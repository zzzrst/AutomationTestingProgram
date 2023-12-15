// <copyright file="DatabaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALM_Migrator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using Spire.Xls;
    using DatabaseConnector;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The base implementation of the Database Data.
    /// </summary>
    public class DatabaseData
    {

        //private string Collection { get; set; } = "0";

        //private string Release { get; set; } = "0";
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseData"/> class.
        /// Base constructor for the database Data.
        /// </summary>
        /// <param name="args">args to be passed in.</param>
        public DatabaseData()
        {
            this.TestDBName = ConfigurationManager.AppSettings["DBTestCaseDatabase"].ToString();
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }


        /// <inheritdoc/>
        public Worksheet worksheet { get; set; }


        /// <inheritdoc/>
        public Worksheet setDescWorksheet { get; set; }


        /// <inheritdoc/>
        public Workbook workbook { get; set; }

        /// <inheritdoc/>
        //public string Name { get; } = "Database";

        /// <inheritdoc/>
        private int CellRow { get; set; } = 1;


        /// <inheritdoc/>
        private int SetArgsRow { get; set; } = 1;

        private int testCaseNum = 1;


        /// <inheritdoc/>
        //public int CellColumn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name of the test case db.
        /// </summary>
        public string TestDBName { get; set; }

        /// <summary>
        /// Gets or sets connection established to test database.
        /// </summary>
        public OracleDatabase TestDB { get; set; }

        public void initializeWorkSheetDB()
        {
            this.workbook = new Workbook();
            this.workbook.Worksheets.Clear();
            this.worksheet = workbook.Worksheets.Add("Database Data");
            this.setDescWorksheet = workbook.Worksheets.Add("Test Set Information");

            string[] databaseHeader = new string[] {"TESTCASENAME", "TESTSTEPDESCRIPTION", "STEPNUM", "ACTIONONOBJECT", "OBJECT", "VALUE", "COMMENTS", "RELEASE", "LOCAL_ATTEMPTS",
                "LOCAL_TIMEOUT", "CONTROL", "COLLECTION", "TESTSTEPTYPE", "GOTOSTEP" };

            string[] setDescHeader = new string[] { "Test Suite", "Test Set Description from ALM", "Test Cases (In Execution Order)", "Order Number" };

            // insert header to file
            worksheet.InsertArray(databaseHeader, this.CellRow, 1, false);
            setDescWorksheet.InsertArray(setDescHeader, this.CellRow, 1, false);

            this.CellRow += 1;
            this.SetArgsRow += 1;
        }

        public void resetTestCaseNum()
        {
            this.setDescWorksheet.Range[this.SetArgsRow - this.testCaseNum + 1, 1, this.SetArgsRow - 1, 1].Merge();
            this.setDescWorksheet.Range[this.SetArgsRow - this.testCaseNum + 1, 2, this.SetArgsRow - 1, 2].Merge();

            this.testCaseNum = 1;
        }

        public void writeTestSetDetails(string TestSetName, string TestSetDescription)
        {
            this.setDescWorksheet.Range[this.SetArgsRow, 1].Text = TestSetName;
            //this.setDescWorksheet.Range[this.SetArgsRow, 2].Text = TestSetDescription;

            string stripedText = StripHTML(TestSetDescription);

            this.setDescWorksheet.Range[this.SetArgsRow, 2].Text = stripedText;

            //RichText richText = this.setDescWorksheet.Range[this.SetArgsRow, 2].RichText;

            // since description is rich text, try to convert to rich text format
            //richText.Text = TestSetDescription;
        }

        /// <summary>
        /// Gets or sets name of the environment.
        /// </summary>
        //protected string Environment { get; set; }

        /// <summary>
        /// connects the given database and returns it.
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <returns>The same database.</returns>
        public OracleDatabase ConnectToDatabase(OracleDatabase database)
        {
            if (database == null || !database.IsConnected())
            {
                int count = 0;
                int maxTries = 2;

                // trys 2 times
                while (count < maxTries)
                {
                    string host = ConfigurationManager.AppSettings["DBHost"].ToString();
                    string port = ConfigurationManager.AppSettings["DBPort"].ToString();
                    string serviceName = ConfigurationManager.AppSettings["DBServiceName"].ToString();
                    string userID = ConfigurationManager.AppSettings["DBUserId"].ToString();
                    string password = ConfigurationManager.AppSettings["DBPassword"].ToString();

                    Console.WriteLine($"Attempting to connect to host:{host}, port:{port}, service:{serviceName}, user:{userID}, password:{password}");

                    database = new OracleDatabase(host, port, serviceName, userID, password);
                    database.Connect();
                    if (database.IsConnected())
                    {
                        Console.WriteLine($"Connected to database: {serviceName}");
                        break;
                    }

                    count++;
                }

                if (count > maxTries)
                {
                    Console.WriteLine($"Failed to Connected to database");
                }
            }

            return database;
        }


        /// <summary>
        /// Queries a test case from the test database given the testcase name, collection, and release.
        /// </summary>
        /// <param name="testcase">Name of the testcase.</param>
        /// <returns>A test case from the test database.</returns>
        public List<List<object>> QueryTestCase(string testcase, string collection, string release)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);

            // write the name of the test set
            this.setDescWorksheet.Range[this.SetArgsRow, 3].Text = testcase;
            // write the test set number, starting at 1
            this.setDescWorksheet.Range[this.SetArgsRow, 4].Text = this.testCaseNum.ToString();
            this.SetArgsRow += 1;
            this.testCaseNum += 1;

            // note that this query will take whatever ALM supplies. the second query takes latest. PFAAM latest is 23.6 but 25 is in the DBw which causes errors
            string query = $"SELECT T.TESTCASE, T.TESTSTEPDESCRIPTION, T.STEPNUM, T.ACTIONONOBJECT, T.OBJECT, T.VALUE, T.COMMENTS, T.RELEASE, T.LOCAL_ATTEMPTS, T.LOCAL_TIMEOUT, T.CONTROL, T.COLLECTION, T.TEST_STEP_TYPE_ID, T.GOTOSTEP FROM {this.TestDBName} T WHERE T.TESTCASE = '{testcase}' AND T.COLLECTION = '{collection}' AND T.RELEASE = '{release}' ORDER BY T.STEPNUM";
            //string query = $"SELECT t.TESTCASE, t.TESTSTEPDESCRIPTION, t.STEPNUM, t.ACTIONONOBJECT, t.OBJECT, t.VALUE, t.COMMENTS, t.RELEASE, t.LOCAL_ATTEMPTS, t.LOCAL_TIMEOUT, t.CONTROL, t.COLLECTION, t.TEST_STEP_TYPE_ID, t.GOTOSTEP FROM {this.TestDBName} t JOIN (SELECT testcase, MAX(release) AS latest_release FROM {this.TestDBName} WHERE collection LIKE '{this.Collection}' AND testcase LIKE '{testcase}'  AND release LIKE '%' GROUP BY testcase) latest ON t.testcase = latest.testcase AND t.release = latest.latest_release ORDER BY t.testcase, t.stepnum, t.release DESC";

            Console.WriteLine("Querying the following: [" + query + "]");
            var result = this.TestDB.ExecuteQuery(query);
            this.TestDB.Disconnect();
            Console.WriteLine("Closed connection to database.\n");
            if (result == null || result.Count == 0)
            {
                Console.WriteLine("Test Case Not Found in Database");
                //throw new Exception("Database Test Case Not Found");
            }
            return result;
        }

        /// <summary>
        /// Queries a test case from the test database given the testcase name, collection, and release.
        /// </summary>
        /// <param name="testcase">Name of the testcase.</param>
        /// <returns>A test case from the test database.</returns>
        public List<List<object>> QueryForLatestDistinct(string colName)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);
            //string query = $"SELECT T.TESTCASE, T.TESTSTEPDESCRIPTION, T.STEPNUM, T.ACTIONONOBJECT, T.OBJECT, T.VALUE, T.COMMENTS, T.RELEASE, T.LOCAL_ATTEMPTS, T.LOCAL_TIMEOUT, T.CONTROL, T.COLLECTION, T.TEST_STEP_TYPE_ID, T.GOTOSTEP FROM {this.TestDBName} T WHERE T.TESTCASE = '{testcase}' AND T.COLLECTION = '{this.Collection}' AND T.RELEASE = '{this.Release}' ORDER BY T.STEPNUM";
            string query = $"SELECT DISTINCT t.{colName} FROM QA_AUTOMATION.TESTCASE t JOIN (SELECT testcase, MAX(release) AS latest_release FROM QA_AUTOMATION.TESTCASE WHERE collection LIKE '%' AND testcase LIKE '%'  AND release LIKE '%' GROUP BY testcase) latest ON t.testcase = latest.testcase AND t.release = latest.latest_release ORDER BY t.{colName}";
            Console.WriteLine("Querying the following: [" + query + "]");

            var result = this.TestDB.ExecuteQuery(query);
            this.TestDB.Disconnect();
            Console.WriteLine("Closed connection to database.\n");
            if (result == null || result.Count == 0)
            {
                Console.WriteLine("No results found in Database");
                //throw new Exception("Database Test Case Not Found");
            }
            return result;
        }


        /// <summary>
        /// The A Test Step.
        /// </summary>
        /// <param name="row">The row<see cref="T:List{object}"/>.</param>
        /// <returns>The <see cref=""/>.</returns>
        public void WriteTestStepsToExcel(List<object> row)
        {
            // input testCaseName
            //worksheet.Range[this.CellRow, 1].Value = testCaseName;

            // start at the second column and input the rest of the data
            for(int i = 0; i < row.Count; i++)
            {
                // iterate through the for loop and input the data 
                // note that we are saving the value as a Text format
                worksheet.Range[this.CellRow, i+1].Text = row[i].ToString();
            }
            // autofit range of columns
            worksheet.AllocatedRange.AutoFitColumns();

            this.CellRow += 1; 
        }

        // save to file
        public void SaveToFile(string testCaseName)
        {
            // save file to excel
            //string fileName = $"C:\\TEMP\\excel_db_export_{testCaseName}_{this.Collection}_{this.Release}.xlsx";
            string fileName = $"C:\\TEMP\\{testCaseName}.xlsx";
            try
            {
                this.workbook.SaveToFile(fileName, ExcelVersion.Version2016);
            }
            catch (IOException)
            {
                Console.WriteLine("Warning! File is currently open by another process. Cannot overwrite file: " + testCaseName);
                return;
            }
            //System.Diagnostics.Process.Start(fileName);
            Console.WriteLine("Finished running SaveToFile()");
        }

        // strip html 
        public string StripHTML(string HTMLText)
        {
            var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(HTMLText, "");
        }

    }
}
