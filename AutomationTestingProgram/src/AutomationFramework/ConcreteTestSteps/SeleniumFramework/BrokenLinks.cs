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

            // get all links
            List<string> urlList = InformationObject.TestAutomationDriver.GetAllLinksURL();

            if (urlList.Count != 0)
            {
                // attempt to click on link
                foreach (string url in urlList)
                {
                    HttpWebResponse response;
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
                        this.TestStepStatus.RunSuccessful = false;
                        this.TestStepStatus.FriendlyErrorMessage += $"Found broken link '{url}' with status code of '{(int)statusCode}'.";
                    }
                }
            }

            this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? "No broken links were found" : "Something Went wrong.";
        }
    }
}
