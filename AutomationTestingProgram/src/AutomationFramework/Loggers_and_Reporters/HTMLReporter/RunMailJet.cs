using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Mailjet.Client.TransactionalEmails;
using System.IO;

namespace AutomationTestingProgram
{
    public class RunMailJet
    {
        //private static List<string> SendToEmails { get; set; };
        //private static string FromEmail { get; set; };
        //private static string HTMLContent { get; set; };

        public static void CreateEmail(string html_content, List<string> emails)
        {
            // TODO: blocking call, try to use async Main and async eventhandlers in WinForms/WPF
            //HTMLContent = html_content;
            //SendToEmails = toEmails;

            RunAsync(html_content, emails).Wait();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        static async Task RunAsync(string html_content, List<string> emails)
           {
            Console.WriteLine("Sending Email");

            MailjetClient client = new MailjetClient(
                System.Configuration.ConfigurationManager.AppSettings["MJ_APIKEY_PUBLIC"], 
                System.Configuration.ConfigurationManager.AppSettings["MJ_APIKEY_PRIVATE"]);

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };

            string contactSendFrom = System.Configuration.ConfigurationManager.AppSettings["EMAIL_SENDER"];

            string testSetName = InformationObject.TestSetName;
            string buildNumber = InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.BuildNumber);
            string envVar = InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.Environment);

            //// attachment will contain more details
            //Attachment newAttachment = new Attachment($"{testSetName.Replace(" ", string.Empty)}_execution.html", contentType: "text/html", html_content);

            // content in the email will be shorter than the content in the pdf
            string refactoredContent = DeleteUnusedEmailContent(html_content);

            // construct your email with builder
            var email = new TransactionalEmailBuilder();
            email.WithFrom(new SendContact(contactSendFrom));
            email.WithSubject($"Test Execution from DevOps {testSetName} {envVar} {buildNumber}");
            email.WithHtmlPart("<h1>Latest Execution</h1>\n");
            email.WithHtmlPart(refactoredContent);
            //email.WithAttachment(newAttachment);

            // append each email item to the email
            foreach (var emailItem in emails)
            {
                email.WithTo(new SendContact(emailItem));
            }

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(email.Build());

            Console.WriteLine("Email sent: " + response.Messages[0].Status);

        }

        // helper function that will parse through the HTML file for the flag "IF_EMAIL_THEN_DELETE" and delete the lines of the file
        static string DeleteUnusedEmailContent(string originalEmailHTML)
        {
            // create a while loop that will iterate through the entire html
            string search = "IF_EMAIL_THEN_DELETE";

            int movingIndex = 0;
            int prevIndex = 0;

            int countFound = 0;

            // as long as there are still search elements remaining, index into
            while (originalEmailHTML.Contains(search) && movingIndex != -1)
            {
                // if the count is divisible by 2, then delete
                if (countFound % 2 == 0)
                {
                    // delete between the two values.
                    // Since we know that they these two values must end between comments, there is no need to delete the comment
                    int delAmt = movingIndex + search.Length - prevIndex;
                    originalEmailHTML = originalEmailHTML.Remove(prevIndex, delAmt);

                    //Logger.Info("Moving index: " + movingIndex);
                    //Logger.Info("Previous index: " + prevIndex);
                    //Logger.Info("Delete index: " + delAmt);
                    movingIndex = prevIndex;

                    // movingIndex -= delAmt;
                }
                else
                {
                    // if we will pass, since we are only going to delete between two values
                }

                // set the previous to the current
                prevIndex = movingIndex;

                // if the moving index is greater than the size of the string, then don't attempt
                if (movingIndex > originalEmailHTML.Length)
                {
                    break;
                }
                else
                {
                    // found index of the search
                    movingIndex = originalEmailHTML.IndexOf(search, movingIndex + 1);
                    countFound += 1;
                }
            }

            //File.WriteAllText("C:\\TEMP\\output_test.html", originalEmailHTML);

            return originalEmailHTML;
        }
    }
}