// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Main program for ALMConnector.
    /// </summary>
    public class Program
    {/*
        /// <summary>
        /// Main method for alm connector.
        /// </summary>
        /// <param name="args">{folder path} {alm user}. </param>
        internal static void Main(string[] args)
        {
            string username = ConfigurationManager.AppSettings["username"].ToString();
            string password = ConfigurationManager.AppSettings["password"].ToString();
            string domain = ConfigurationManager.AppSettings["domain"].ToString();
            string project = ConfigurationManager.AppSettings["project"].ToString();
            Connector c = new Connector(username, password, domain, project);

            Console.WriteLine($"We are going to attempt to set the responsible tester to {args[1]} for all test sets under path {args[0]}");

            List<string> testSets = c.GetAllTestSetByPath(args[0]);
            if (testSets.Count == 0)
            {
                Console.WriteLine($"No tests were found with the provided path {args[0]}");
                return;
            }

            Console.WriteLine($"Total # of test sets found were: {testSets.Count}");

            foreach (string testSetID in testSets)
            {
                TestSetInstance tsi = c.SetTestSetByUID(int.Parse(testSetID));
                bool cont = true;
                int count = 0;
                while (cont)
                {
                    tsi.SetCurrentTestCaseField("Responsible Tester", args[1]);
                    cont = tsi.MoveToNextTestCase();
                    count++;
                }

                Console.WriteLine($"  A total of {count} test cases inside {tsi.Name} has been updated.");
            }
        }*/
    }
}
