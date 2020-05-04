// <copyright file="VerifyWebTableContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class VerifyWebTableContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify WebTable Content";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string filePath = this.Arguments["value"];
            List<List<string>> expected = new List<List<string>>();
            if (File.Exists(filePath))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        expected.Add(line.Split(',').ToList());
                    }
                }

                List<List<string>> tableArray = this.GetTable();
                List<int> mapping = new List<int>();

                // find the columns we are interested in
                if (expected.Count >= 2)
                {
                    bool foundAllColumns = true;
                    foreach (string header in expected[0])
                    {
                        string headerElement = header.Trim();
                        if (tableArray[0].Contains(headerElement))
                        {
                            int index = tableArray[0].IndexOf(headerElement);
                            mapping.Add(index);
                            this.TestStepStatus.Actual += $"\nFound {headerElement} in column {index}.";
                        }
                        else
                        {
                            this.TestStepStatus.Actual += $"\nCould not find the following {headerElement} in the web table.";
                            foundAllColumns = false;
                            break;
                        }
                    }

                    // check each row in expected
                    if (foundAllColumns)
                    {
                        int rowIndex = 1;
                        while (rowIndex < expected.Count)
                        {
                            // find it in the table
                            foreach (List<string> row in tableArray)
                            {
                                bool foundRow = true;
                                foreach (int colIndex in mapping)
                                {
                                    foundRow = foundRow && (row[colIndex] == expected[rowIndex][colIndex].Trim());
                                }

                                this.TestStepStatus.RunSuccessful = foundRow;
                                if (foundRow)
                                {
                                    this.TestStepStatus.Actual += $"\n Successfully found row {rowIndex} in web table";
                                    rowIndex++;
                                    break;
                                }
                            }

                            if (!this.TestStepStatus.RunSuccessful)
                            {
                                this.TestStepStatus.Actual += $"\n Could not find row {rowIndex} in web table";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    this.TestStepStatus.Actual = "Expected table didn't have more than the header row.";
                }
            }
            else
            {
                this.TestStepStatus.Actual = $"Could not find file at {filePath}";
            }
        }

        /// <summary>
        ///  Changes the web table into a 2D array.
        /// </summary>
        /// <returns>The 2D array of the table in string representation.</returns>
        private List<List<string>> GetTable()
        {
            string baseXPath = this.XPathBuilder();

            string tableHeaderpath = baseXPath + "/thead/tr/th";
            string tableRowPath = baseXPath + "/tbody/tr";

            int numOfRows = this.FindAllByXPath(tableRowPath).Count;

            List<List<string>> result = new List<List<string>>();
            List<string> header = new List<string>();

            foreach (IWebElement e in this.FindAllByXPath(tableHeaderpath))
            {
                header.Add(e.Text);
            }

            result.Add(header);

            for (int rowNum = 1; rowNum < numOfRows + 1; rowNum++)
            {
                List<string> row = new List<string>();
                string rowXPath = $"{tableRowPath}[{rowNum}]/td";
                List<IWebElement> elements = this.FindAllByXPath(rowXPath) ?? new List<IWebElement>();
                foreach (IWebElement e in elements)
                {
                    row.Add(e.Text);
                }

                result.Add(row);
            }

            return result;
        }

        /// <summary>
        /// The FindAllByXPath.
        /// </summary>
        /// <param name="xPath">The XPath<see cref="string"/>.</param>
        /// <returns>The <see cref="List{IWebElement}"/>.</returns>
        private List<IWebElement> FindAllByXPath(string xPath)
        {
            return ((TestingDriver.SeleniumDriver)InformationObject.TestAutomationDriver).WebDriver.FindElements(By.XPath(xPath)).ToList();
        }
    }
}
