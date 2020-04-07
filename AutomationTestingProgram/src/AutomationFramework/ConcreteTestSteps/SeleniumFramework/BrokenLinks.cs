// <copyright file="BrokenLinks.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// This test step to print broken links..
    /// </summary>
    public class BrokenLinks : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Broken Links";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            bool passed = true;

            // get all links
            List<string> urlList = InformationObject.TestAutomationDriver.GetAllLinksURL();

            string message = string.Empty;
            if (urlList.Count != 0)
            {
                // attempt to click on link
                foreach (string url in urlList)
                {
                    HttpWebResponse response = null;
                    HttpStatusCode statusCode;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch (WebException we)
                    {
                        response = (HttpWebResponse)we.Response;
                    }

                    statusCode = response.StatusCode;
                    if ((int)statusCode >= 400)
                    {
                        passed = false;
                        message += $"Found broken link '{url}' with status code of '{(int)statusCode}'.";
                    }
                }
            }

            this.TestStepStatus.Actual = passed ? "No broken links were found" : "Something Went wrong.";
            this.TestStepStatus.FriendlyErrorMessage = message;
            this.TestStepStatus.RunSuccessful = passed;
        }
    }
}
