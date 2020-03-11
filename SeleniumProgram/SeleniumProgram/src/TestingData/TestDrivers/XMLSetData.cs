// <copyright file="XMLSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.Helper;
    using AutomationTestingProgram.TestAutomationDriver;
    using AutomationTestSetFramework;

    /// <summary>
    /// The XML Driver to get data from an xml.
    /// </summary>
    public class XMLSetData : ITestSetData
    {
        /// <summary>
        /// The stack to read/excecute for the test set.
        /// </summary>
        private readonly Stack<XmlNode> testStack = new Stack<XmlNode>();

        /// <summary>
        /// Determines if the current stack layer for test set should execute.
        /// </summary>
        private readonly Stack<bool> performStack = new Stack<bool>();

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "XML";

        /// <summary>
        /// Gets or sets the information for the test set.
        /// </summary>
        private XmlNode TestFlow { get; set; }

        /// <summary>
        /// Gets or sets the xml file containing the XML Data.
        /// </summary>
        private XmlDocument XMLDataFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the xml file containing the test set/case/steps.
        /// </summary>
        private XmlDocument XMLDocObj { get; set; } = null;

        /// <inheritdoc/>
        public void SetUp()
        {
            if (File.Exists(this.TestArgs))
            {
                this.XMLDocObj = new XmlDocument();
                this.XMLDocObj.Load(this.TestArgs);
                this.TestFlow = this.XMLDocObj.GetElementsByTagName("TestCaseFlow")[0];
            }
            else
            {
                Logger.Error("XML File could not be found!");
            }
        }

        /// <inheritdoc/>
        public void SetUpTestSet()
        {
            this.AddNodesToStack(this.TestFlow);
        }

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            return this.testStack.Count > 0;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            ITestCase testCase = this.RunIfTestCaseLayer();
            if (testCase == null)
            {
                throw new Exception("Missing Test case");
            }

            return testCase;
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
        private ITestCase RunIfTestCaseLayer(bool performAction = true)
        {
            ITestCase testCase = null;
            XmlNode currentNode;

            while (this.testStack.Count > 0 && testCase == null)
            {
                currentNode = this.testStack.Pop();
                performAction = this.performStack.Pop();

                if (currentNode.Name == "If")
                {
                    this.RunThenElseLayer(currentNode, performAction);
                }
                else if (currentNode.Name == "RunTestCase")
                {
                    testCase = this.FindTestCase(XMLHelper.ReplaceIfToken(currentNode.InnerText, this.XMLDataFile), performAction);
                }
                else
                {
                    Logger.Warn($"We currently do not deal with this: {currentNode.Name}");
                }
            }

            return testCase;
        }

        /// <summary>
        /// Finds the given test case.
        /// </summary>
        /// <param name="testCaseID">ID to find the testcase to run.</param>
        /// <param name="performAction"> Perfoms the action. </param>
        /// <returns> 0 if pass. >=1 if fail.</returns>
        private ITestCase FindTestCase(string testCaseID, bool performAction = true)
        {
            ITestCase testCase = InformationObject.TestCaseData.SetUpTestCase(testCaseID, performAction);
            return testCase;
        }

        /// <summary>
        /// This function parses the if flow and starts executing. Used in both Test Set and Test Case.
        /// </summary>
        /// <param name="ifXMLNode"> XML Node that has the if block. </param>
        /// <param name="performAction"> Perfoms the action. </param>
        private void RunThenElseLayer(XmlNode ifXMLNode, bool performAction = true)
        {
            bool ifCondition = false;

            // we check condition if we have to perfom this action.
            if (performAction)
            {
                string elementXPath = XMLHelper.ReplaceIfToken(ifXMLNode.Attributes["elementXPath"].Value, this.XMLDataFile);
                string condition = ifXMLNode.Attributes["condition"].Value;

                ITestAutomationDriver.ElementState state = condition == "EXIST" ? ITestAutomationDriver.ElementState.Visible : ITestAutomationDriver.ElementState.Invisible;

                ifCondition = InformationObject.TestAutomationDriver.CheckForElementState(elementXPath, state);
            }

            // inside the testCaseFlow, you can only have either RunTestCase element or an If element.
            foreach (XmlNode ifSection in ifXMLNode.ChildNodes)
            {
                switch (ifSection.Name)
                {
                    case "Then":
                        // we run this test case only if performAction is true, and the condition for the element has passed.
                        this.AddNodesToStack(ifSection, performAction && ifCondition);
                        break;
                    case "ElseIf":
                        // we check the condition if performAction is true and the previous if condition was false.
                        // we can only run the test case if performAction is true, previous if condition was false, and the current if condition is true.
                        bool secondIfCondition = false;

                        if (performAction && !ifCondition)
                        {
                            string elementXPath = XMLHelper.ReplaceIfToken(ifXMLNode.Attributes["elementXPath"].Value, this.XMLDataFile);
                            string condition = ifSection.Attributes["condition"].Value;

                            ITestAutomationDriver.ElementState state = condition == "EXIST" ? ITestAutomationDriver.ElementState.Visible : ITestAutomationDriver.ElementState.Invisible;

                            secondIfCondition = InformationObject.TestAutomationDriver.CheckForElementState(elementXPath, state);
                        }

                        this.AddNodesToStack(ifSection, performAction && !ifCondition && secondIfCondition);

                        // update ifCondition to reflect if elseIf was run
                        ifCondition = !ifCondition && secondIfCondition;
                        break;
                    case "Else":
                        // at this point, we only run this action if performAction is true and the previous ifCondition was false.
                        this.AddNodesToStack(ifSection, performAction && !ifCondition);
                        break;
                    default:
                        Logger.Warn($"We currently do not deal with this. {ifSection.Name}");
                        break;
                }
            }
        }
    }
}
