// <copyright file="TestCaseXml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestSetFramework;
    using SeleniumPerfXML.Implementations.Loggers_and_Reporters;
    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Implementation of the testCase class.
    /// </summary>
    public class TestCase : ITestCase
    {
        private readonly Stack<XmlNode> testStack = new Stack<XmlNode>();

        /// <summary>
        /// Determines if the current stack layer should execute.
        /// </summary>
        private readonly Stack<bool> performStack = new Stack<bool>();

        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public string Name { get; set; } = "Test Case";

        /// <inheritdoc/>
        public int TestCaseNumber { get; set; }

        /// <inheritdoc/>
        public int TotalTestSteps
        {
            get => this.TestCaseInfo.ChildNodes.Count;
            set => this.TotalTestSteps = this.TestCaseInfo.ChildNodes.Count;
        }

        /// <inheritdoc/>
        public ITestCaseStatus TestCaseStatus { get; set; }

        /// <inheritdoc/>
        public int CurrTestStepNumber { get; set; } = 0;

        /// <inheritdoc/>
        public IMethodBoundaryAspect.FlowBehavior OnExceptionFlowBehavior { get; set; }

        /// <summary>
        /// Gets or sets the ammount of times this should be ran.
        /// </summary>
        public int ShouldExecuteAmountOfTimes { get; set; } = 1;

        /// <summary>
        /// Gets or sets the ammount of times the test case has ran.
        /// </summary>
        public int ExecuteCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the information for the test case.
        /// </summary>
        public XmlNode TestCaseInfo { get; set; }

        /// <summary>
        /// Gets or sets the reporter.
        /// </summary>
        public IReporter Reporter { get; set; }

        /// <summary>
        /// Gets or sets the seleniumDriver to use.
        /// </summary>
        public SeleniumDriver Driver { get; set; }

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            return this.testStack.Count > 0 || this.ShouldExecuteAmountOfTimes > this.ExecuteCount + 1;
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            ITestStep testStep = null;

            // reached end of loop, check if should loop again.
            if (this.testStack.Count == 0 && this.ShouldExecuteAmountOfTimes > this.ExecuteCount)
            {
                this.AddNodesToStack(this.TestCaseInfo);
                this.ExecuteCount += 1;
            }

            testStep = this.IfRunTestStepLayer();

            return testStep;
        }

        /// <inheritdoc/>
        public void HandleException(Exception e)
        {
            this.ExecuteCount -= 1;
            this.TestCaseStatus.ErrorStack = e.StackTrace;
            this.TestCaseStatus.FriendlyErrorMessage = e.Message;
            this.TestCaseStatus.RunSuccessful = false;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (this.TestCaseStatus == null)
            {
                this.TestCaseStatus = new TestCaseStatus()
                {
                    StartTime = DateTime.UtcNow,
                    TestCaseNumber = this.TestCaseNumber,
                };
            }

            if (this.testStack.Count <= 0)
            {
                this.AddNodesToStack(this.TestCaseInfo);
            }
        }

        /// <inheritdoc/>
        public bool ShouldExecute()
        {
            return this.ShouldExecuteVariable;
        }

        /// <inheritdoc/>
        public void TearDown()
        {
            this.TestCaseStatus.EndTime = DateTime.UtcNow;
            if (!this.ShouldExecuteVariable)
            {
                this.TestCaseStatus.Actual = "Did not run.";
            }

            ITestCaseLogger log = new TestCaseLogger();
            log.Log(this);
        }

        /// <inheritdoc/>
        public void UpdateTestCaseStatus(ITestStepStatus testStepStatus)
        {
            if (testStepStatus.RunSuccessful == false)
            {
                this.TestCaseStatus.RunSuccessful = false;
                this.TestCaseStatus.FriendlyErrorMessage = "Something went wrong with a test step";
            }

            this.Reporter.AddTestStepStatusToTestCase(testStepStatus, this.TestCaseStatus);
        }

        private void AddNodesToStack(XmlNode currentNode, bool performAction = true)
        {
            for (int i = currentNode.ChildNodes.Count - 1; i >= 0; i--)
            {
                this.testStack.Push(currentNode.ChildNodes[i]);
                this.performStack.Push(performAction);
            }
        }

        private ITestStep IfRunTestStepLayer(bool performAction = true)
        {
            TestStepXml testStep = null;
            XmlNode currentNode;

            while (this.testStack.Count > 0 && testStep == null)
            {
                currentNode = this.testStack.Pop();
                performAction = this.performStack.Pop();

                if (currentNode.Name == "If")
                {
                    this.RunIfTestCase(currentNode, performAction);
                }
                else if (currentNode.Name == "RunTestStep")
                {
                    testStep = this.FindTestStep(XMLInformation.ReplaceIfToken(currentNode.InnerText), performAction);
                }
                else
                {
                    Logger.Warn($"We currently do not deal with this: {currentNode.Name}");
                }
            }

            return testStep;
        }

        /// <summary>
        /// This function parses the if test case flow and starts executing.
        /// </summary>
        /// <param name="ifXMLNode"> XML Node that has the if block. </param>
        /// <param name="performAction"> Perfoms the action. </param>
        private void RunIfTestCase(XmlNode ifXMLNode, bool performAction = true)
        {
            bool ifCondition = false;

            // we check condition if we have to perfom this action.
            if (performAction)
            {
                string elementXPath = XMLInformation.ReplaceIfToken(ifXMLNode.Attributes["elementXPath"].Value);
                string condition = ifXMLNode.Attributes["condition"].Value;

                SeleniumDriver.ElementState state = condition == "EXIST" ? SeleniumDriver.ElementState.Visible : SeleniumDriver.ElementState.Invisible;

                ifCondition = this.Driver.CheckForElementState(elementXPath, state);
            }

            // inside the testCaseFlow, you can only have either RunTestCase element or an If element.
            foreach (XmlNode ifSection in ifXMLNode.ChildNodes)
            {
                if (ifSection.Name == "Then")
                {
                    // we run this test case only if performAction is true, and the condition for the element has passed.
                    this.AddNodesToStack(ifSection, performAction && ifCondition);
                }
                else if (ifSection.Name == "ElseIf")
                {
                    // we check the condition if performAction is true and the previous if condition was false.
                    // we can only run the test case if performAction is true, previous if condition was false, and the current if condition is true.
                    bool secondIfCondition = false;

                    if (performAction && !ifCondition)
                    {
                        string elementXPath = XMLInformation.ReplaceIfToken(ifXMLNode.Attributes["elementXPath"].Value);
                        string condition = ifSection.Attributes["condition"].Value;

                        SeleniumDriver.ElementState state = condition == "EXIST" ? SeleniumDriver.ElementState.Visible : SeleniumDriver.ElementState.Invisible;

                        secondIfCondition = this.Driver.CheckForElementState(elementXPath, state);
                    }

                    this.AddNodesToStack(ifSection, performAction && !ifCondition && secondIfCondition);

                    // update ifCondition to reflect if elseIf was run
                    ifCondition = !ifCondition && secondIfCondition;
                }
                else if (ifSection.Name == "Else")
                {
                    // at this point, we only run this action if performAction is true and the previous ifCondition was false.
                    this.AddNodesToStack(ifSection, performAction && !ifCondition);
                }
                else
                {
                    Logger.Warn($"We currently do not deal with this. {ifSection.Name}");
                }
            }
        }

        /// <summary>
        /// This function will go through the list of steps and run the appropriate test step if found.
        /// </summary>
        /// <param name="testStepID"> The ID of the test step to run. </param>
        /// <param name="performAction"> Perfoms the action. </param>
        /// <returns>0 if pass. >=1 if fail.</returns>
        private TestStepXml FindTestStep(string testStepID, bool performAction = true)
        {
            TestStepXml testStep = null;

            // get the list of testSteps
            XmlNode testSteps = XMLInformation.XMLDocObj.GetElementsByTagName("TestSteps")[0];

            // Find the appropriate test steps
            foreach (XmlNode innerNode in testSteps.ChildNodes)
            {
                if (innerNode.Name != "#comment" && XMLInformation.ReplaceIfToken(innerNode.Attributes["id"].Value) == testStepID)
                {
                    testStep = this.BuildTestStep(innerNode, performAction);
                    return testStep;
                }
            }

            Logger.Warn($"Sorry, we didn't find a test step that matched the provided ID: {testStepID}");
            return testStep;
        }

        private TestStepXml BuildTestStep(XmlNode testStepNode, bool performAction = true)
        {
            TestStepXml testStep = null;
            string name = XMLInformation.ReplaceIfToken(testStepNode.Attributes["name"].Value);

            // initial value is respectRunAODAFlag
            // if we respect the flag, and it is not found, then default value is false.
            bool runAODA = XMLInformation.RespectRunAODAFlag;
            if (runAODA)
            {
                if (testStepNode.Attributes["runAODA"] != null)
                {
                    runAODA = bool.Parse(testStepNode.Attributes["runAODA"].Value);
                }
                else
                {
                    runAODA = false;
                }
            }

            // populate runAODAPageName. Deault is Not provided.
            string runAODAPageName = "Not provided.";
            if (runAODA)
            {
                if (testStepNode.Attributes["runAODAPageName"] != null)
                {
                    runAODAPageName = XMLInformation.ReplaceIfToken(testStepNode.Attributes["runAODAPageName"].Value);
                }
            }

            // log is true by default.
            bool log = true;
            if (testStepNode.Attributes["log"] != null)
            {
                log = bool.Parse(testStepNode.Attributes["log"].Value);
            }

            Logger.Debug($"Test step '{name}': runAODA->{runAODA} runAODAPageName->{runAODAPageName} log->{log}");

            testStep = ReflectiveGetter.GetEnumerableOfType<TestStepXml>()
                .Find(x => x.Name.Equals(testStepNode.Name));

            if (testStep == null)
            {
                Logger.Error($"Was not able to find the provided test action '{testStepNode}'.");
            }
            else
            {
                string namePrepender = this.ExecuteCount > 0 ? $"{this.ExecuteCount}.{this.CurrTestStepNumber} " : $"";

                for (int index = 0; index < testStepNode.Attributes.Count; index++)
                {
                    testStepNode.Attributes[index].InnerText = XMLInformation.ReplaceIfToken(testStepNode.Attributes[index].InnerText);
                }

                testStep.Name = name;
                testStep.ShouldLog = log;
                testStep.TestStepInfo = testStepNode;
                testStep.ShouldExecuteVariable = performAction && this.ShouldExecuteVariable;
                testStep.RunAODA = runAODA;
                testStep.RunAODAPageName = runAODAPageName;
                testStep.Driver = this.Driver;
                testStep.Reporter = this.Reporter;
                testStep.TestStepNumber = this.CurrTestStepNumber + (this.ExecuteCount * this.TotalTestSteps);
            }

            return testStep;
        }
    }
}
