// <copyright file="TestSetXml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    using AutomationTestSetFramework;
    using SeleniumPerfXML.Implementations.Loggers_and_Reporters;
    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Implementation of the ITestSet Class.
    /// </summary>
    public class TestSetXml : ITestSet
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
        public string Name { get; set; } = "Test Set";

        /// <inheritdoc/>
        public int TotalTestCases
        {
            get => this.TestCaseFlow.ChildNodes.Count;
            set => this.TotalTestCases = this.TestCaseFlow.ChildNodes.Count;
        }

        /// <inheritdoc/>
        public ITestSetStatus TestSetStatus { get; set; }

        /// <inheritdoc/>
        public int CurrTestCaseNumber { get; set; } = -1;

        /// <inheritdoc/>
        public IMethodBoundaryAspect.FlowBehavior OnExceptionFlowBehavior { get; set; }

        /// <summary>
        /// Gets or sets the information for the test set.
        /// </summary>
        public XmlNode TestCaseFlow { get; set; }

        /// <summary>
        /// Gets or sets the reporter.
        /// </summary>
        public IReporter Reporter { get; set; }

        /// <summary>
        /// Gets or sets the seleniumDriver to use.
        /// </summary>
        public SeleniumDriver Driver { get; set; }

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            return this.testStack.Count > 0;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            TestCaseXml testCase = null;

            testCase = this.IfRunTestCaseLayer();

            if (testCase == null)
            {
                throw new Exception("Missing Test case");
            }

            return testCase;
        }

        /// <inheritdoc/>
        public void HandleException(Exception e)
        {
            this.TestSetStatus.ErrorStack = e.StackTrace;
            this.TestSetStatus.FriendlyErrorMessage = e.Message;
            this.TestSetStatus.RunSuccessful = false;
            this.ShouldExecuteVariable = false;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (this.TestSetStatus == null)
            {
                this.TestSetStatus = new TestSetStatus()
                {
                    StartTime = DateTime.UtcNow,
                };
            }

            this.AddNodesToStack(this.TestCaseFlow);
        }

        /// <inheritdoc/>
        public bool ShouldExecute()
        {
            return this.ShouldExecuteVariable;
        }

        /// <inheritdoc/>
        public void TearDown()
        {
            this.TestSetStatus.EndTime = DateTime.UtcNow;

            this.Reporter.AddTestSetStatus(this.TestSetStatus);

            ITestSetLogger log = new TestSetLogger();
            log.Log(this);
        }

        /// <inheritdoc/>
        public void UpdateTestSetStatus(ITestCaseStatus testCaseStatus)
        {
            if (testCaseStatus.RunSuccessful == false)
            {
                this.TestSetStatus.RunSuccessful = false;
            }

            this.Reporter.AddTestCaseStatus(testCaseStatus);
        }

        /// <summary>
        /// Adds all the child nodes in the outer most layer to the stack in reverse order.
        /// </summary>
        /// <param name="currentNode">the layer to add.</param>
        /// <param name="performAction">Whether or not the nodes should execute their test cases.</param>
        private void AddNodesToStack(XmlNode currentNode, bool performAction = true)
        {
            for (int i = currentNode.ChildNodes.Count - 1; i >= 0; i--)
            {
                this.testStack.Push(currentNode.ChildNodes[i]);
                this.performStack.Push(performAction);
            }
        }

        /// <summary>
        /// Reads the layer which contains Ifs and RunTestCase and returns the next test case.
        /// </summary>
        /// <param name="performAction">Tells whether or not to perform the action for the test case.</param>
        /// <returns>The next test case.</returns>
        private TestCaseXml IfRunTestCaseLayer(bool performAction = true)
        {
            TestCaseXml testCase = null;
            XmlNode currentNode;

            while (this.testStack.Count > 0 && testCase == null)
            {
                currentNode = this.testStack.Pop();
                performAction = this.performStack.Pop();

                if (currentNode.Name == "If")
                {
                    this.RunIfTestCase(currentNode, performAction);
                }
                else if (currentNode.Name == "RunTestCase")
                {
                    testCase = this.FindTestCase(XMLInformation.ReplaceIfToken(currentNode.InnerText), performAction);
                }
                else
                {
                    Logger.Warn($"We currently do not deal with this: {currentNode.Name}");
                }
            }

            return testCase;
        }

        /// <summary>
        /// Runs the test case based on the provided ID.
        /// </summary>
        /// <param name="testCaseID">ID to find the testcase to run.</param>
        /// <param name="performAction"> Perfoms the action. </param>
        /// <returns> 0 if pass. >=1 if fail.</returns>
        private TestCaseXml FindTestCase(string testCaseID, bool performAction = true)
        {
            TestCaseXml testCase = null;

            // get the list of testcases
            XmlNode testCases = XMLInformation.XMLDocObj.GetElementsByTagName("TestCases")[0];

            // Find the appropriate testcase;
            foreach (XmlNode node in testCases.ChildNodes)
            {
                if (node.Name == "TestCase" && XMLInformation.ReplaceIfToken(node.Attributes["id"].Value) == testCaseID)
                {
                    int repeat = 1;
                    string name = "TestCase";
                    if (XMLInformation.RespectRepeatFor && node.Attributes["repeatFor"] != null)
                    {
                        repeat = int.Parse(node.Attributes["repeatFor"].Value);

                        // repeat = repeat > 1 ? 1 : -1;
                    }

                    if (node.Attributes["name"] != null)
                    {
                        name = node.Attributes["name"].Value;
                    }

                    testCase = new TestCaseXml()
                    {
                        Name = name,
                        TestCaseInfo = node,
                        ShouldExecuteAmountOfTimes = repeat,
                        ShouldExecuteVariable = performAction,
                        Reporter = this.Reporter,
                        Driver = this.Driver,
                        TestCaseNumber = this.CurrTestCaseNumber,
                    };

                    this.CurrTestCaseNumber += 1;

                    return testCase;
                }
            }

            Logger.Warn($"Sorry, we didn't find a test case that matched the provided ID: {testCaseID}");
            return testCase;
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
    }
}
