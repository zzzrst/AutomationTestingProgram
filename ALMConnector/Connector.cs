// <copyright file="Connector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using TDAPIOLELib;

    /// <summary>
    /// Class to represent a connector that establishes a connection to the ALM server.
    /// </summary>
    public class Connector
    {
        /// <summary>
        /// Defines the ALMUID.
        /// </summary>
        private static readonly string ALMUID = "CY_CYCLE_ID";

        /// <summary>
        /// Defines the connection.
        /// </summary>
        private TDConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connector"/> class.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <param name="password">The password<see cref="string"/>.</param>
        /// <param name="domain">The domain<see cref="string"/>.</param>
        /// <param name="project">The project<see cref="string"/>.</param>
        public Connector(string username, string password, string domain, string project)
        {
            // authentication credentials
            this.Username = username;
            this.Password = password;

            // domain and project strings
            this.Domain = domain;
            this.Project = project;

            // URL to connect to
            this.AlmUrl = ConfigurationManager.AppSettings["ALMURL"];

            // attempt to connect to server
            try
            {
                this.Connected = this.ConnectToServer();
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                throw new Exception("Please register your HP ALM Client and Check your config file!");
            }
        }

        /// <summary>
        /// Gets or sets the Domain
        /// Defines the domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the TestSet.
        /// </summary>
        public TestSetInstance TestSet { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        private string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether defines the connected.
        /// </summary>
        private bool Connected { get; set; } = false;

        /// <summary>
        /// Gets or sets the almUrl.
        /// </summary>
        private string AlmUrl { get; set; }

        /// <summary>
        /// The IsConnected.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsConnected()
        {
            return this.connection.ProjectConnected && this.connection.Connected && this.connection.LoggedIn;
        }

        /// <summary>
        /// The DisconnectFromServer.
        /// </summary>
        public void DisconnectFromServer()
        {
            // Console.WriteLine("Logging Out...");
            this.connection.Logout();

            // Console.WriteLine("Disconnecting...");
            this.connection.ReleaseConnection();
        }

        /// <summary>
        /// The ConnectToServer.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ConnectToServer()
        {
            // initialize new TDConnection
            this.connection = new TDConnection();
            bool connected = false; // initializes returning result

            // conn.InitConnectionEx(almurl);
            // Console.WriteLine("Checking Server Compatibility: " + connection.InitAndCheckServerCompatibility(almUrl));
            this.connection.InitAndCheckServerCompatibility(this.AlmUrl);

            // Authentication
            // Console.WriteLine("Logging in...");
            try
            {
                this.connection.Login(this.Username, this.Password);
            }
            catch (Exception)
            {
                Console.WriteLine("Login Failed");
                return false;
            }

            // Connect to a project
            // Console.WriteLine("Connecting to Project...");
            try
            {
                this.connection.Connect(this.Domain, this.Project);
            }
            catch (Exception)
            {
                Console.WriteLine("Connecting to Project Failed");
                return false;
            }

            // check connected status
            connected = this.connection.ProjectConnected && this.connection.Connected && this.connection.LoggedIn;

            // Console.WriteLine("Connected: " + connected);
            return connected;
        }

        /// <summary>
        /// The SetTestSetByPath.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="TestSetInstance"/>.</returns>
        public TestSetInstance SetTestSetByPath(string path)
        {
            // trim off the testname from the end of the path
            string[] trimmed = this.TrimTestName(path);

            // get list of the testcaseinstatnces from the path of the folder
            List<TestSetInstance> list = this.FindListOfTestSetsByPath(trimmed[0]);

            // loop through each testcaseinstance in the list
            foreach (TestSetInstance testSetInstance in list)
            {
                // get the folder path of of this test case
                string testfolderpath = testSetInstance.GetFolderPath();

                // if folder path matches and name matches then set res to this testcase
                if (testfolderpath == trimmed[0].TrimEnd('\\') && testSetInstance.Name == trimmed[1])
                {
                    this.TestSet = testSetInstance;
                }
            }

            if (this.TestSet == null)
            {
                throw new CannotFindTestSet(CannotFindTestSet.ErrorMsg + path);
            }

            return this.TestSet;
        }

        /// <summary>
        /// The GetAllTestSetByPath.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="TestSetInstance"/>.</returns>
        public List<string> GetAllTestSetByPath(string path)
        {
            List<string> res = new List<string>();

            TestSetTreeManager tree = this.connection.TestSetTreeManager as TestSetTreeManager;
            try
            {
                TestSetFolder tsFolder = tree.NodeByPath[path] as TestSetFolder;
                List tsList = tsFolder.FindTestSets(string.Empty);
                foreach (TestSet test in tsList)
                {
                    res.Add($"{test.ID}");
                }
            }
            catch (Exception)
            {
            }

            return res;
        }

        /// <summary>
        /// The SetTestSetByUID.
        /// </summary>
        /// <param name="uID">The UID<see cref="int"/>.</param>
        /// <returns>The <see cref="TestSetInstance"/>.</returns>
        public TestSetInstance SetTestSetByUID(int uID)
        {
            // create a new filter filtering out by UID
            TestSetFactory factory = this.connection.TestSetFactory as TestSetFactory;
            TDFilter filter = factory.Filter as TDFilter;
            try
            {
                filter[ALMUID] = uID.ToString();
            }
            catch (Exception)
            {
            }

            List list = filter.NewList(); // filters out testset by UID

            if (list.Count == 1)
            {
                this.TestSet = new TestSetInstance(list[1] as TestSet, this.Username);
            }
            else
            {
                throw new CannotFindTestSet(CannotFindTestSet.ErrorMsg + uID);
            }

            return this.TestSet;
        }

        /// <summary>
        /// Sends the Test Set Execution Report.
        /// </summary>
        public void SendExecutionReport()
        {
            if (this.TestSet.Finished == null)
            {
                this.TestSet.Finished = DateTime.Now;
            }

            TestSetExecutionSummaryReport tsExecSummaryReport = new TestSetExecutionSummaryReport()
            {
                TestSet = this.TestSet,
            };
            string htmlReport = tsExecSummaryReport.GenerateHTMLReport();

            string status = "Passed";

            if (this.TestSet.TotalBlocked > 0)
            {
                status = "Blocked";
            }

            if (this.TestSet.TotalFailed > 0)
            {
                status = "Failed";
            }

            this.SendEmail(this.TestSet.EmailList, $"{this.TestSet.Name} Execution Summary [{status}]", htmlReport, this.TestSet.Attachments.ToArray(), "HTML");

            if (status == "Failed")
            {
                this.SendEmail(this.TestSet.FailedEmailList, $"{this.TestSet.Name} Execution Summary [{status}]", htmlReport, this.TestSet.Attachments.ToArray(), "HTML");
            }
        }

        /// <summary>
        /// The SendEmail.ai.
        /// </summary>
        /// <param name="aLMUser">The ALMUser(s) to send the email to.<see cref="string"/>.</param>
        /// <param name="emailSubject">The subject of the email<see cref="string"/>.</param>
        /// <param name="emailBody">The body of the email<see cref="string"/>.</param>
        /// <param name="serverFileNames">The file name on the server for the attachment<see cref="T:object[]"/>.</param>
        /// <param name="emailType">Either "HTML" or "Text" to determine the format of the email body<see cref="string"/>.</param>
        public void SendEmail(string aLMUser, string emailSubject, string emailBody, object[] serverFileNames, string emailType)
        {
            try
            {
                if (serverFileNames.Length == 0)
                {
                    this.connection.SendMail(aLMUser, string.Empty, emailSubject, emailBody, null, emailType);
                }
                else
                {
                    this.connection.SendMailEx(aLMUser, string.Empty, emailSubject, emailBody, (int)TDMAIL_FLAGS.TDMAIL_ATTACHMENT, serverFileNames, emailType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// The FindListOfTestSetsByPath.
        /// </summary>
        /// <param name="testLabPath">The testLabPath<see cref="string"/>.</param>
        /// <returns>The <see cref="List{ALMTestSetInstance}"/>.</returns>
        private List<TestSetInstance> FindListOfTestSetsByPath(string testLabPath)
        {
            List<TestSetInstance> res = new List<TestSetInstance>(); // create a new list of ALMTestSetInstances
            TestSetTreeManager tree = this.connection.TestSetTreeManager as TestSetTreeManager; // gets tree set mananger form QC Connection
            try
            {
                TestSetFolder tsFolder = tree.NodeByPath[testLabPath] as TestSetFolder; // gets test set folder from the testLabPath
                List tsList = tsFolder.FindTestSets(string.Empty); // gets list of test sets in the folder
                foreach (TestSet test in tsList)
                {
                    res.Add(new TestSetInstance(test, this.Username));
                }
            }
            catch (Exception)
            {
            }

            return res; // return list of ALMTestCaseInstance
        }

        /// <summary>
        /// The TrimTestName.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        /// <returns>The <see cref="T:string[]"/>.</returns>
        private string[] TrimTestName(string path)
        {
            string[] res = new string[2];
            string[] dir = path.Split('\\');
            res[1] = dir[dir.Length - 1];
            dir[dir.Length - 1] = null;
            res[0] = string.Join("\\", dir);
            return res;
        }
    }
}
