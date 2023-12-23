// <copyright file="AxeDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    //using Deque.AxeCore.Selenium;
    //using Deque.AxeCore.Commons;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using Selenium.Axe;

    /// <summary>
    /// This is the driver to deal with Axe.core.
    /// </summary>
    public class AxeDriver : IAccessibilityChecker
    {
        private const string TALLIEDRESULT = "TalliedResult.csv";
        private const string RULEPAGESUMMARY = "RulePageSummary.csv";
        private const string AODAEXCELREPORT = "WATRReport.xlsx";

        /// <summary>
        /// Result Type -> { Rule ID -> {Page URL -> HTML, Target, Data, Related Nodes} }.
        /// </summary>
        private Dictionary<string, Dictionary<string, Dictionary<string, HashSet<RuleNodeInformation>>>> results;

        /// <summary>
        /// Page URL -> Provided Page Title, Browser Page Title.
        /// </summary>
        private Dictionary<string, PageInformation> pageInfo;

        /// <summary>
        /// Rule ID -> Description, Rule Tag, Impact, Help, Help URL.
        /// </summary>
        private Dictionary<string, RuleInformation> ruleInfo;

        /// <summary>
        /// [Page URL, Provided Page Title, Browser Page Title, Passes, Violations, Incomplete, Inapplicable].
        /// </summary>
        private List<string> pageSummary;

        /// <summary>
        /// Logger to use when logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxeDriver"/> class.
        /// </summary>
        /// <param name="driver">driver to use.</param>
        public AxeDriver(IWebDriver driver)
            : this(driver, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxeDriver"/> class.
        /// </summary>
        /// <param name="driver">driver to use.</param>
        /// <param name="logger">Logger to use.</param>
        public AxeDriver(IWebDriver driver, ILogger logger)
        {
            this.logger = logger;
            this.Driver = driver;
            this.results = new Dictionary<string, Dictionary<string, Dictionary<string, HashSet<RuleNodeInformation>>>>();
            this.pageInfo = new Dictionary<string, PageInformation>();
            this.ruleInfo = new Dictionary<string, RuleInformation>();
            this.pageSummary = new List<string>()
            {
                "Page URL, Provided Page Title, Browser Page Title, Passes, Violations, Incomplete, Inapplicable",
            };
        }

        /// <inheritdoc/>
        public IWebDriver Driver { get; set; }

        /// <summary>
        /// This captures the AODA result for this webpage.
        /// </summary>
        /// <param name="providedPageTitle"> Title of the page. </param>
        public void CaptureResult(string providedPageTitle)
        {
            this.logger?.LogInformation($"Capturing Result for {providedPageTitle}");

            // Here we are maximizing and making the page full screen, which we don't necessarility want
            // this.Driver.Manage().Window.FullScreen();
            AxeResult results = this.Driver.Analyze();

            // check if there is any error. If there is, write it out
            // Console.WriteLine("Axe violations: " + results.Violations.ToString());

            // map page information
            if (!this.pageInfo.ContainsKey(results.Url))
            {
                this.pageInfo[results.Url] = new PageInformation()
                {
                    BrowserPageTitle = this.Driver.Title,
                    ProvidedPageTitle = providedPageTitle,
                };

                // adds it to the excel sheet too
            }

            // map each axe result
            this.MapAxeResult(results.Inapplicable, AxeResultType.INAPPLICABLE, results.Url);
            this.MapAxeResult(results.Incomplete, AxeResultType.INCOMPLETE, results.Url);
            this.MapAxeResult(results.Passes, AxeResultType.PASS, results.Url);
            this.MapAxeResult(results.Violations, AxeResultType.VIOLATIONS, results.Url);

            // populate mapping of each axe result into a nice list
            this.pageSummary.Add(
                string.Format(
                    $"\"{results.Url}\"," +
                    $"\"{providedPageTitle}\"," +
                    $"\"{this.Driver.Title}\"," +
                    $"\"{results.Passes.Count()}\"," +
                    $"\"{results.Violations.Count()}\"," +
                    $"\"{results.Incomplete.Count()}\"," +
                    $"\"{results.Inapplicable.Count()}\""));
            this.logger?.LogInformation($"Capturing Success.");
        }

        /// <summary>
        /// Logs the result for this file.
        /// </summary>
        /// <param name="folderLocation">Location to save all the results.</param>
        public void LogResults(string folderLocation)
        {
            TestReportExcel excelReport = new TestReportExcel()
            {
                FileLocation = folderLocation + "\\" + AODAEXCELREPORT,
                UrlList = this.pageInfo.Keys.ToList(),
            };

            List<string> rulePageSummary = new List<string>()
            {
                "Page URl,Provided Page Title,Browser Page Title,Result Type,Description,Rule Tag,Impact,Help,Help URL,Occurance on Page",
            };
            this.logger?.LogInformation($"Generating Json File.");

            // we are looping through each resultType
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, HashSet<RuleNodeInformation>>>> resultType in this.results)
            {
                // we are now looping through each rule
                foreach (KeyValuePair<string, Dictionary<string, HashSet<RuleNodeInformation>>> ruleID in resultType.Value)
                {
                    // we are now looping through each page
                    foreach (KeyValuePair<string, HashSet<RuleNodeInformation>> pageURL in ruleID.Value)
                    {
                        string currentURL = pageURL.Key;
                        string currentProvidedPageTitle = this.pageInfo[pageURL.Key].ProvidedPageTitle;
                        string currentBrowserPageTitle = this.pageInfo[pageURL.Key].BrowserPageTitle;

                        // get each of the nodeInfo out
                        List<JObject> nodeInfoList = new List<JObject>();
                        foreach (RuleNodeInformation node in pageURL.Value)
                        {
                            JObject nodeInfo = new JObject(
                                new JProperty("Page URL", currentURL),
                                new JProperty("Provided Page Title", currentProvidedPageTitle),
                                new JProperty("Browser Page Title", currentBrowserPageTitle),
                                new JProperty("HTML", node.HTML),
                                new JProperty("Target", node.Target));
                            nodeInfoList.Add(nodeInfo);
                        }

                        // add it into rule Summary
                        JObject ruleSummary = new JObject(
                            new JProperty("Rule ID", ruleID.Key),
                            new JProperty("Result Type", resultType.Key),
                            new JProperty("Description", this.ruleInfo[ruleID.Key].Description),
                            new JProperty("Rule Tag", this.ruleInfo[ruleID.Key].RuleTag),
                            new JProperty("Impact", this.ruleInfo[ruleID.Key].Impact),
                            new JProperty("Help", this.ruleInfo[ruleID.Key].Help),
                            new JProperty("Help URL", this.ruleInfo[ruleID.Key].HelpUrl),
                            new JProperty("Nodes", nodeInfoList));

                        // pass, fail or n/a.
                        string criteriaString = ResourceHelper.GetString(ResourceHelper.GetString(resultType.Key));

                        // add it to the excel data
                        this.WriteToExcelData(excelReport, this.ruleInfo[ruleID.Key].RuleTag, criteriaString, $"{this.ruleInfo[ruleID.Key].Help}");

                        // add it to the excel issue list only if it failed.
                        if (criteriaString.Equals(ResourceHelper.GetString("CriteriaFail")))
                        {
                            List<string> criteriaIds = this.GetCriteriaId(this.ruleInfo[ruleID.Key].RuleTag);
                            foreach (string id in criteriaIds)
                            {
                                excelReport.IssueList.Add(new IssueLog()
                                {
                                    Criterion = id,
                                    Impact = ResourceHelper.GetString($"IssueKey{this.ruleInfo[ruleID.Key].Impact}"),
                                    Description = this.ruleInfo[ruleID.Key].Help,
                                    Url = currentURL,
                                });
                            }
                        }

                        // record occurance on page
                        rulePageSummary.Add(
                            string.Format(
                                $"\"{currentURL}\"," +
                                $"\"{currentProvidedPageTitle}\"," +
                                $"\"{currentBrowserPageTitle}\"," +
                                $"\"{resultType.Key}\"," +
                                $"\"{this.ruleInfo[ruleID.Key].Description}\"," +
                                $"\"{string.Join(" ", this.ruleInfo[ruleID.Key].RuleTag)}\"," +
                                $"\"{this.ruleInfo[ruleID.Key].Impact}\"," +
                                $"\"{this.ruleInfo[ruleID.Key].Help}\"," +
                                $"\"{this.ruleInfo[ruleID.Key].HelpUrl}\"," +
                                $"\"{pageURL.Value.Count.ToString()}\""));

                        // write it to file
                        string fileName = $"{ruleID.Key}_{string.Join("_", this.ruleInfo[ruleID.Key].RuleTag)}.json";

                        string directoryPath = $"{folderLocation}\\Json\\{resultType.Key}";
                        Directory.CreateDirectory(directoryPath);

                        using (StreamWriter file = File.AppendText($"{directoryPath}\\{fileName}"))
                        using (JsonTextWriter writer = new JsonTextWriter(file) { Formatting = Formatting.Indented })
                        {
                            ruleSummary.WriteTo(writer);
                        }
                    }
                }
            }

            this.logger?.LogInformation($"Generating RulePageSummary.csv");

            // populate RulePageSummary.csv
            using (StreamWriter sw = new StreamWriter(folderLocation + RULEPAGESUMMARY))
            {
                foreach (string rulePage in rulePageSummary)
                {
                    sw.WriteLine(rulePage);
                }
            }

            this.logger?.LogInformation($"Generating TalliedResult.csv");

            // write out TalliedResult.csv
            using (StreamWriter sw = new StreamWriter(folderLocation + TALLIEDRESULT))
            {
                foreach (string pageResult in this.pageSummary)
                {
                    sw.WriteLine(pageResult);
                }
            }

            this.logger?.LogInformation($"Generating WATRReport.xlsx");
            excelReport.WriteToExcel();
        }

        /// <summary>
        /// Writes the result to the excel sheet under checklist.
        /// </summary>
        /// <param name="excelReport">The excel sheet.</param>
        /// <param name="ruleTag">List of rules.</param>
        /// <param name="resultString">If it passed or failed.</param>
        /// <param name="comment">any comments that it comes with.</param>
        private void WriteToExcelData(TestReportExcel excelReport, List<string> ruleTag, string resultString, string comment)
        {
            // add it into the excel sheet.
            List<string> ids;
            int criteriaColumn = int.Parse(ResourceHelper.GetString("CriteriaColumn"));
            int commentColumn = int.Parse(ResourceHelper.GetString("CommentColumn"));

            // representation of one row.
            List<string> row = new List<string>();

            // Add Criteria.
            row.Add(resultString);

            // Add Comments.
            row.Add(comment);

            ids = this.GetCriteriaId(ruleTag);

            // add the key.
            foreach (string rowName in ids)
            {
                // If there is an existing data.
                if (excelReport.ExcelData.ContainsKey(rowName))
                {
                    // If any one row is a fail, it will change the result to a fail.
                    if (excelReport.ExcelData[rowName][criteriaColumn] != "Fail")
                    {
                        excelReport.ExcelData[rowName] = row;
                    }
                    else
                    {
                        // if the row is already a fail, add more comments to it only if the same comment does not exist already.
                        if (!excelReport.ExcelData[rowName][commentColumn].Contains(row[commentColumn]))
                        {
                            excelReport.ExcelData[rowName][commentColumn] += $"\r\n{row[commentColumn]}";
                        }
                    }
                }
                else
                {
                    excelReport.ExcelData.Add(rowName, row);
                }
            }
        }

        /// <summary>
        /// Finds the Criteria Id inside the ruleTags.
        /// </summary>
        /// <param name="ruleTag">List of tags.</param>
        /// <returns>the id.</returns>
        private List<string> GetCriteriaId(List<string> ruleTag)
        {
            List<string> tags = new List<string>();
            List<string> ids = new List<string>();
            tags = ruleTag.FindAll(s => s.Contains("wcag") && !s.Contains("1a") && !s.Contains("2a"));

            foreach (string tag in tags)
            {
                string id = tag.Substring(4);
                id = id.Aggregate(string.Empty, (c, i) => c + i + '.');
                id = id.Substring(0, id.Length - 1);
                ids.Add(id);
            }

            return ids;
        }

        /// <summary>
        /// Populate RuleInfo and Results based on AxeResultItems[] passed in.
        /// </summary>
        /// <param name="axeResults">The different results that were found.</param>
        /// <param name="resultType"> The type of result. </param>
        /// <param name="url">The url used for this page.</param>
        private void MapAxeResult(AxeResultItem[] axeResults, string resultType, string url)
        {
            foreach (AxeResultItem resultItem in axeResults)
            {
                // populate RuleInfo
                if (!this.ruleInfo.ContainsKey(resultItem.Id))
                {
                    this.ruleInfo[resultItem.Id] = new RuleInformation()
                    {
                        Description = resultItem.Description,
                        Help = resultItem.Help,
                        HelpUrl = resultItem.HelpUrl,
                        Impact = resultItem.Impact,
                        RuleTag = resultItem.Tags.ToList(),
                    };
                }

                // populate Results
                foreach (AxeResultNode resultNode in resultItem.Nodes)
                {
                    RuleNodeInformation temp = new RuleNodeInformation()
                    {
                        HTML = resultNode.Html,

                        // original
                        // Target = resultNode.Target,

                        Target = resultNode.Target.ToString(),
                    };

                    // add into the results dictionary
                    if (this.results.ContainsKey(resultType))
                    {
                        if (this.results[resultType].ContainsKey(resultItem.Id))
                        {
                            if (this.results[resultType][resultItem.Id].ContainsKey(url))
                            {
                                // add it to the list
                                this.results[resultType][resultItem.Id][url].Add(temp);
                            }
                            else
                            {
                                // adding new rule-node information
                                this.results[resultType][resultItem.Id][url] = new HashSet<RuleNodeInformation>() { temp };
                            }
                        }
                        else
                        {
                            // result Type exists, but ruleID doesn't
                            this.results[resultType][resultItem.Id] = new Dictionary<string, HashSet<RuleNodeInformation>>()
                            {
                                { url, new HashSet<RuleNodeInformation>() { temp } },
                            };
                        }
                    }
                    else
                    {
                        // we have to input new record for result type -> ruleId -> page Url -> {Rule Node Information}
                        this.results[resultType] = new Dictionary<string, Dictionary<string, HashSet<RuleNodeInformation>>>()
                        {
                            {
                                resultItem.Id, new Dictionary<string, HashSet<RuleNodeInformation>>()
                                {
                                    { url, new HashSet<RuleNodeInformation>() { temp } },
                                }
                            },
                        };
                    }
                }
            }
        }
    }
}
