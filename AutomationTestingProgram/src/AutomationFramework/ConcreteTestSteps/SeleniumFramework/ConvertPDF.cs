// <copyright file="ConvertPDF.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using ComparePDF;

    /// <summary>
    /// This test step to compare pdfs.
    /// </summary>
    public class ConvertPDF : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ConvertPDF";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string pdfFileToConvert_Expected = $"{this.Arguments["object"]}";
            string pdfFileToConvert_Actual = this.Arguments["value"];

            PDFToolWrapper.RunPDFToText(pdfFileToConvert_Expected, @"C:\Temp\expected.txt", Logger.GetLog4NetLogger());
            this.TestStepStatus.Actual = $@"Converted {pdfFileToConvert_Expected} to C:\Temp\expected.txt.";
            Logger.Info(Logger.Tab(4) + this.TestStepStatus.Actual);

            PDFToolWrapper.RunPDFToText(pdfFileToConvert_Expected, @"C:\Temp\actual.txt", Logger.GetLog4NetLogger());
            string secondMessage = $@"Converted {pdfFileToConvert_Actual} to C:\Temp\actual.txt.";
            this.TestStepStatus.Actual += secondMessage;

            Logger.Info(Logger.Tab(4) + secondMessage);
        }
    }
}
