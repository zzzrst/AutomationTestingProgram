// <copyright file="SharePointDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using Microsoft.SharePoint.Client;
    using System.Security;
    using NPOI.OpenXmlFormats.Spreadsheet;

    /// NOTE: This class does not work. See the SeleniumFramework class for more details on previous implementation. 
    /// After extensive testing, I (Victor), could not get CSOM library to work with authenticating into the SP online site. 
    /// Perhaps it was do to the domain, user/pass, etc, but also apparently the CSOM library is no longer suggested. 
    /// Microsoft wants to use the PnP.Framework and this is something that we should look towards using going forward. 
    //using PnP.Framework;

    /// <summary>
    /// Class to interact with SharePoint.
    /// </summary>
    public static class SharepointGetter
    {
        /// <summary>
        /// Gets the build number from the sharepoint list.
        /// </summary>
        /// <param name="environment">Name of the environment.</param>
        /// <returns>String for the build number.</returns>
        public static string GetBuildNumber(string environment)
        {
            // warn that the program does not work right now. 
            Logger.Warn("Get Build Number in Sharepoint Getter is not implemented");

            string mappingName = "BUILD NUMBER";

            DatabaseStepData db = new DatabaseStepData("");

            List<string> sPInfo = db.GetEnvironmentSharePointMapping(environment, mappingName);

            // query for creds
            string username = db.GetGlobalVariableValue("WINDOW ACCOUNT USERNAME");
            string password = db.GetGlobalVariableValue("WINDOW ACCOUNT PASSWORD");


            string collection = "";
            //ListItemCollection collection = GetSharePointListItem(sPInfo[0], sPInfo[1], sPInfo[2], sPInfo[3], username, password);
            collection = GetSharePointListItem(sPInfo[0], sPInfo[1], sPInfo[2], sPInfo[3], username, password);


            return collection; // Select(collection, sPInfo[4]);
        }

        /// <summary>
        /// Helper function to get all the items in the targeted SharePoint List that whose anchor column has the same anchor column value.
        /// </summary>
        /// <param name="targetRootSPSite">The root site of the SharePoint list.</param>
        /// <param name="targetSPList">The title of the SharePoint list.</param>
        /// <param name="targetAnchorColumnFieldRef">The field reference of the anchor column of the SharePoint List.</param>
        /// <param name="targetAnchorColumnValue">The value of the anchor column that items have to be equal to.</param>
        /// <param name="username"> The username used to connect to SharePoint.</param>
        /// <param name="password"> The password used to connect to SharePoint.</param>
        /// <returns>All items that match the provided value in the anchor column.</returns>
        private static string GetSharePointListItem(string targetRootSPSite, string targetSPList, string targetAnchorColumnFieldRef, string targetAnchorColumnValue, string username, string password)
        {
            CamlQuery query = new CamlQuery();

            string domain = username.Split('\\')[0];
            string user = username.Split('\\')[1];

            //string site = "https://ontariogov.sharepoint.com/sites/CSC-Intranet/DDSB/AA/BuildsInfrast/Lists/Environments/";
            string site = "https://ontariogov.sharepoint.com/sites/CSC-Intranet/";

            ClientContext sharePointContext = new ClientContext(site)
            {
                Credentials = new NetworkCredential(user, password, domain),
            };

            //NetworkCredential myCred = new NetworkCredential(username, password, domain);

            //CredentialCache myCache = new CredentialCache();

            //ClientContext sharePointContext = new ClientContext("https://ontariogov.sharepoint.com/sites/CSC-Intranet/DDSB/AA/BuildsInfrast/Lists/Environments/Standard.aspx");

            //List sharePointList = context.Web.Lists.GetByTitle(targetSPList);

            //query.ViewXml = $"<View><Query><Where><Eq>" +
            //                  $"<FieldRef Name='{targetAnchorColumnFieldRef}'/>" +
            //                  $"<Value Type='Text'>{targetAnchorColumnValue}</Value>" +
            //                $"</Eq></Where></Query></View>";

            //ListItemCollection listItems = sharePointList.GetItems(query);
            Web web1 = sharePointContext.Web;

            try
            {

                List sharePointList = sharePointContext.Web.Lists.GetByTitle(targetSPList);

                query.ViewXml = $"<View><Query><Where><Eq>" +
                                  $"<FieldRef Name='{targetAnchorColumnFieldRef}'/>" +
                                  $"<Value Type='Text'>{targetAnchorColumnValue}</Value>" +
                                $"</Eq></Where></Query></View>";

                sharePointContext.Load(web1);
                sharePointContext.ExecuteQuery();

            }
            catch (Exception e)
            {
                Logger.Info("Error with SP query: " + e);
            }

            return web1.Title;
        }


        //private static void ConnectSPCSOM(string url, string user, string password)
        //{
        //    using (ClientContext context = new ClientContext(url))
        //    {
        //        context.Credentials = new System.Net.NetworkCredential(user, password);
        //        context.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(MixedAuthRequestMethod);
        //        try
        //        {
        //            Web web2 = context.Web;
        //            Console.WriteLine("Loading web...");
        //            context.Load(web2);
        //            context.ExecuteQuery();
        //            Console.WriteLine(web2.Title);
        //            Console.WriteLine(web2.Url);
        //        }
        //        catch (Exception exception)
        //        {
        //            Console.WriteLine("Exception of type" + exception.GetType() + "caught.");
        //            Console.WriteLine(exception.Message);
        //            Console.WriteLine(exception.StackTrace);
        //        }

        //        Web web = context.Web;

        //        context.Load(web);
        //        context.ExecuteQuery();
        //    }
        //}

        //private static void MixedAuthRequestMethod(object sender, WebRequestEventArgs e)
        //{
        //    try
        //    {
        //        e.WebRequestExecutor.RequestHeaders.Remove("X-FORMS_BASED_AUTH_ACCEPTED");
        //        e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine("Exception of type" + exception.GetType() + "caught.");
        //        Console.WriteLine(exception.Message);
        //        Console.WriteLine(exception.StackTrace);
        //    }
        //}

        ///// <summary>
        ///// Returns the value of the target column in the list provided.
        ///// </summary>
        ///// <param name="sharePointListItems">The list of items inside the SharePoint List.</param>
        ///// <param name="targetColumnFieldRef">The field reference for the target column.</param>
        ///// <returns>The value of the target column.</returns>
        //private static string Select(ListItemCollection sharePointListItems, string targetColumnFieldRef)
        //{
        //    List<string> results = new List<string>();

        //    foreach (ListItem row in sharePointListItems)
        //    {
        //        results.Add($"{row[targetColumnFieldRef]}");
        //    }

        //    return string.Join("|", results.ToArray());
        //}
    }
}
