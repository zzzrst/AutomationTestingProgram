// <copyright file="ConvertPDF.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using AutomationTestinProgram.Helper;
    using ComparePDF;
    using NPOI.HPSF;

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

            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"]);
                string pdfFileToConvert_Expected = $"{this.Arguments["object"]}";
                string pdfFileToConvert_Actual = this.Arguments["value"];

                pdfFileToConvert_Expected = FilePathResolver.Resolve(pdfFileToConvert_Expected);
                pdfFileToConvert_Actual = FilePathResolver.Resolve(pdfFileToConvert_Actual);

                InformationObject.TestSetData.AddAttachment(pdfFileToConvert_Expected);
                InformationObject.TestSetData.AddAttachment(pdfFileToConvert_Actual);

                PDFToolWrapper.RunPDFToText(pdfFileToConvert_Expected, $@"{path}\expected.txt", Logger.GetLog4NetLogger());
                this.TestStepStatus.Actual = $@"Converted {pdfFileToConvert_Expected} to {path}\expected.txt.";
                Logger.Info(Logger.Tab(4) + this.TestStepStatus.Actual);

                PDFToolWrapper.RunPDFToText(pdfFileToConvert_Actual, $@"{path}\actual.txt", Logger.GetLog4NetLogger());
                string secondMessage = $@"Converted {pdfFileToConvert_Actual} to {path}\actual.txt.";
                this.TestStepStatus.Actual += secondMessage;

                Logger.Info(Logger.Tab(4) + secondMessage);
            }
            catch (Exception e)
            {
                Logger.Info("Convert PDF FAILED");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Converting PDF";
                this.HandleException(e);
            }
        }
    }
}
