// <copyright file="ChooseCollectionOld.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class ChooseCollectionOld : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Choose Collection Old";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            throw new NotImplementedException();
            /*string obj = this.TestObject.Object;
            string property = this.TestObject.Attribute;
            string collection = this.DbDriver.QuerySpecialChars(this.Alm.AlmEnvironment, obj).ToString();

            // find and click collection drop-down list
            Test_Object objCollectionDdl = new Test_Object("Collection DDL", "acc_name", "Choose a collection activate");
            objCollectionDdl.AddAttribute("tag", "span");
            ClickWebElement clkCollectionDdl = new ClickWebElement(objCollectionDdl, this.Timeout);
            clkCollectionDdl.Alm = this.Alm;
            clkCollectionDdl.BrowserDriver = this.BrowserDriver;
            clkCollectionDdl.DbDriver = this.DbDriver;
            if ((this.TestStatus = clkCollectionDdl.Execute()).Pass)
            {
                // find and click collection item
                Test_Object objCollectionLnk = new Test_Object("Element to select from DDL", property, collection);
                objCollectionLnk.AddAttribute("tag", "div");
                ClickLink clkCollectionLnk = new ClickLink(objCollectionLnk, this.Timeout);
                clkCollectionLnk.Alm = clkCollectionDdl.Alm;
                clkCollectionLnk.BrowserDriver = clkCollectionDdl.BrowserDriver;
                clkCollectionLnk.DbDriver = clkCollectionDdl.DbDriver;
                this.TestStatus = clkCollectionLnk.Execute();
            }*/
        }
    }
}
