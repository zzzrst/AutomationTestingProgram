// <copyright file="ComparePDF.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using ComparePDF;

    /// <summary>
    /// This test step to compare pdfs.
    /// </summary>
    public class ComparePdf : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ComparePDF";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string pdf1 = this.Arguments["object"];
            string pdf2 = this.Arguments["value"];
            PDFComparer pDFComparer = new PDFComparer(pdf1, pdf2, Logger.GetLog4NetLogger());
            this.TestStepStatus.RunSuccessful = pDFComparer.ComparePDFByHash();
        }
    }
}
