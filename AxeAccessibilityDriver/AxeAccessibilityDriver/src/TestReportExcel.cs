// <copyright file="TestReportExcel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;

    /// <summary>
    /// The excel test reporter object.
    /// </summary>
    public class TestReportExcel
    {
        /// <summary>
        /// The number of critera Failed.
        /// </summary>
        private int criteriaFailed = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestReportExcel"/> class.
        /// Creates a new Excel report.
        /// </summary>
        public TestReportExcel()
        {
            this.ExcelData = new Dictionary<string, List<string>>();
            this.IssueList = new List<IssueLog>();
        }

        /// <summary>
        /// Gets or sets the data to write to the excel sheet.
        /// </summary>
        public Dictionary<string, List<string>> ExcelData { get; set; }

        /// <summary>
        /// Gets or sets the AODA defects.
        /// </summary>
        public List<IssueLog> IssueList { get; set; }

        /// <summary>
        /// Gets or sets the list of all urls tested.
        /// </summary>
        public List<string> UrlList { get; set; }

        /// <summary>
        /// Gets or sets name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the url of the project.
        /// </summary>
        public string ProjectUrl { get; set; }

        /// <summary>
        /// Gets or sets the date this was modified.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the location to save the file to.
        /// </summary>
        public string FileLocation { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\WATR_Result.xlsx";

        /// <summary>
        /// Writes the aoda results to the excel file.
        /// </summary>
        public void WriteToExcel()
        {
            IWorkbook workbook = null;

            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\WATR_Template.xlsx";

            string resultFilePath = this.FileLocation;

            using (FileStream templateFS = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(templateFS);
            }

            this.UpdateChecklistSheet(workbook);
            this.UpdateIssueSheet(workbook);
            this.UpdateSummarySheet(workbook);

            // write to output.
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
                workbook.Close();
            }
        }

        private void UpdateSummarySheet(IWorkbook workbook)
        {
            double totalCriterias = double.Parse(ResourceHelper.GetString("TOTAL_CRITERIA"));
            int progressRow = int.Parse(ResourceHelper.GetString("SUMMARY_PROGRESS_ROW"));
            int progressCell = int.Parse(ResourceHelper.GetString("SUMMARY_PROGRESS_CELL"));

            // get the summary sheet to modify.
            ISheet sheet = workbook.GetSheet(ResourceHelper.GetString("SheetSummary"));

            // Set the progress
            int progress = (int)Math.Round((totalCriterias - this.criteriaFailed) / totalCriterias * 100);
            sheet.GetRow(progressRow).GetCell(progressCell).SetCellValue(progress.ToString() + "%");

            // update the summary
            int summaryRow = int.Parse(ResourceHelper.GetString("SUMMARY_SUMMARY"));
            int summaryCel = 0;
            string summary = $"There are total of {this.criteriaFailed} out of the box issues remaining, however all have workarounds in place in form of conforming alternate version that are freely available. Several fixed defects require retesting and will soon be tested.";
            sheet.GetRow(summaryRow).GetCell(summaryCel).SetCellValue(summary);

            // updates the link tested
            int linkRow = int.Parse(ResourceHelper.GetString("SUMMARY_LINKS"));
            int linkCel = 0;
            string links = string.Empty;
            foreach (string url in this.UrlList)
            {
                // character limit
                if ((links + url + "\r\n").Length < 32000)
                {
                    links += url + "\r\n";
                }
            }

            sheet.GetRow(linkRow).GetCell(linkCel).SetCellValue(links);

            // upadate the date
            // set the date
            int dateRow = int.Parse(ResourceHelper.GetString("SUMMARY_DATE_ROW"));
            int dateCell = int.Parse(ResourceHelper.GetString("SUMMARY_DATE_CELL"));
            string date = $"This evaluation was carried out on {DateTime.Now.ToString("F")}.";
            sheet.GetRow(dateRow).GetCell(dateCell).SetCellValue(date);
        }

        private void UpdateChecklistSheet(IWorkbook workbook)
        {
            int startCol = int.Parse(ResourceHelper.GetString("CHECKLIST_START_COL"));

            // get the checklist sheet to modify.
            ISheet sheet = workbook.GetSheet(ResourceHelper.GetString("SheetCheckList"));

            this.DefineColourFormattingChecklistSheet(sheet);

            foreach (string key in this.ExcelData.Keys)
            {
                int rowId = this.FindIdWithValue(key, sheet);
                int colIndex = startCol;

                if (rowId >= 0)
                {
                    foreach (string col in this.ExcelData[key])
                    {
                        if (colIndex == startCol + int.Parse(ResourceHelper.GetString("CommentColumn")))
                        {
                            // only put comments on rows that fail.
                            if (this.ExcelData[key][int.Parse(ResourceHelper.GetString("CriteriaColumn"))].Equals("Fail"))
                            {
                                sheet.GetRow(rowId).GetCell(colIndex).SetCellValue(col);
                                this.criteriaFailed++;
                            }
                        }
                        else
                        {
                            sheet.GetRow(rowId).GetCell(colIndex).SetCellValue(col);
                        }

                        colIndex++;
                    }
                }
            }

            // update the hidden column that represents the pass/fail
            int hiddenColumn = 7;
            for (int row = 12; row < 56; row++)
            {
                workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateFormulaCell(sheet.GetRow(row).GetCell(hiddenColumn));
            }

            // update the total
            int totalRow = int.Parse(ResourceHelper.GetString("CHECKLIST_TOTAL_ROW"));
            int totalCell = int.Parse(ResourceHelper.GetString("CHECKLIST_TOTAL_CELL"));
            workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateFormulaCell(sheet.GetRow(totalRow).GetCell(totalCell));

            // set the date
            int dateRow = int.Parse(ResourceHelper.GetString("CHECKLIST_DATE_ROW"));
            int dateCell = int.Parse(ResourceHelper.GetString("CHECKLIST_DATE_CELL"));
            sheet.GetRow(dateRow).GetCell(dateCell).SetCellValue(DateTime.Now.ToString());
        }

        /// <summary>
        /// Defines the conditional formatting for the checklist sheet.
        /// </summary>
        /// <param name="sheet">the checklist sheet.</param>
        private void DefineColourFormattingChecklistSheet(ISheet sheet)
        {
            // Define formatting.
            XSSFSheetConditionalFormatting sCF = (XSSFSheetConditionalFormatting)sheet.SheetConditionalFormatting;

            // Fill Green if Passing Score
            XSSFConditionalFormattingRule cfGreen =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, $"\"{ResourceHelper.GetString("CriteriaPass")}\"");
            XSSFPatternFormatting fillGreen = (XSSFPatternFormatting)cfGreen.CreatePatternFormatting();
            fillGreen.FillBackgroundColor = IndexedColors.LightGreen.Index;
            fillGreen.FillPattern = FillPattern.SolidForeground;

            // Fill Red if Failing Score
            XSSFConditionalFormattingRule cfRed =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, $"\"{ResourceHelper.GetString("CriteriaFail")}\"");
            XSSFPatternFormatting fillRed = (XSSFPatternFormatting)cfRed.CreatePatternFormatting();
            fillRed.FillBackgroundColor = IndexedColors.Rose.Index;
            fillRed.FillPattern = FillPattern.SolidForeground;

            // Fill yellow if blank Score
            XSSFConditionalFormattingRule cfYellow =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, "\"\"");
            XSSFPatternFormatting fillYellow = (XSSFPatternFormatting)cfYellow.CreatePatternFormatting();
            fillYellow.FillBackgroundColor = IndexedColors.LightYellow.Index;
            fillYellow.FillPattern = FillPattern.SolidForeground;

            // Fill yellow if not applicable too
            XSSFConditionalFormattingRule cfYellow2 =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, $"\"{ResourceHelper.GetString("CriteriaNA")}\"");
            XSSFPatternFormatting fillYellow2 = (XSSFPatternFormatting)cfYellow2.CreatePatternFormatting();
            fillYellow2.FillBackgroundColor = IndexedColors.LightYellow.Index;
            fillYellow2.FillPattern = FillPattern.SolidForeground;

            // Fill blue if need manual testing
            XSSFConditionalFormattingRule cfBlue =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, $"\"{ResourceHelper.GetString("CriteriaNeedManual")}\"");
            XSSFPatternFormatting fillBlue = (XSSFPatternFormatting)cfBlue.CreatePatternFormatting();
            fillBlue.FillBackgroundColor = IndexedColors.Aqua.Index;
            fillBlue.FillPattern = FillPattern.SolidForeground;

            // this is in the 'Meets Criteria' Column
            CellRangeAddress[] cfRange =
            {
                CellRangeAddress.ValueOf("D13:D26"), CellRangeAddress.ValueOf("D29:D40"),
                CellRangeAddress.ValueOf("D43:D52"), CellRangeAddress.ValueOf("D55:D56"),
            };

            // You can't add 4 at once
            sCF.AddConditionalFormatting(cfRange, new XSSFConditionalFormattingRule[] { cfRed, cfGreen, cfYellow2, });
            sCF.AddConditionalFormatting(cfRange, new XSSFConditionalFormattingRule[] { cfYellow, cfBlue });

            // fill in the success criteria score

            // Fill Green if Passing Score
            cfGreen =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.LessThanOrEqual, "0");
            fillGreen = (XSSFPatternFormatting)cfGreen.CreatePatternFormatting();
            fillGreen.FillBackgroundColor = IndexedColors.LightGreen.Index;
            fillGreen.FillPattern = FillPattern.SolidForeground;

            // Fill Red if Failing Score
            cfRed =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.GreaterThan, "0");
            fillRed = (XSSFPatternFormatting)cfRed.CreatePatternFormatting();
            fillRed.FillBackgroundColor = IndexedColors.Rose.Index;
            fillRed.FillPattern = FillPattern.SolidForeground;

            sCF.AddConditionalFormatting(
                new CellRangeAddress[] { CellRangeAddress.ValueOf("D63") },
                new XSSFConditionalFormattingRule[] { cfRed, cfGreen, cfYellow });
        }

        /// <summary>
        /// Updates the issue sheet.
        /// </summary>
        /// <param name="workbook">The workbook containing the issue sheet.</param>
        private void UpdateIssueSheet(IWorkbook workbook)
        {
            // get the checklist sheet to modify.
            ISheet sheet = workbook.GetSheet(ResourceHelper.GetString("SheetIssueLog"));

            this.DefineColourFormattingIssueSheet(sheet);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 12;

            ICellStyle hidden = workbook.CreateCellStyle();
            hidden.IsHidden = true;

            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.SetFont(font);

            // set the date
            string date = DateTime.Now.ToString("yyyy/MMMM/dd");

            // get all the criterion options.
            List<string> criterionOptions = new List<string>();
            int totalCriteria = int.Parse(ResourceHelper.GetString("TOTAL_CRITERIA"));
            int hiddenCell = int.Parse(ResourceHelper.GetString("ISSUE_HIDDEN_CRITERA_NAME_CELL"));
            for (int i = 1; i < totalCriteria; i++)
            {
                criterionOptions.Add(sheet.GetRow(i).GetCell(hiddenCell).ToString());
            }

            // print out all the issues
            for (int x = 0; x < this.IssueList.Count; x++)
            {
                IssueLog issueLog = this.IssueList[x];
                IRow row;

                // create a new row. skips the first 3 rows
                row = sheet.CreateRow(3 + x);
                for (int r = 0; r < 8; r++)
                {
                    row.CreateCell(r).CellStyle = style;
                }

                row.GetCell(0).SetCellValue(x + 1);
                row.GetCell(1).SetCellValue(date);
                row.GetCell(2).SetCellValue(issueLog.Url);

                // If it is null, it usualy means best practices.
                if (issueLog.Criterion != null)
                {
                    row.GetCell(3).SetCellValue(criterionOptions.Find(s => s.Contains(issueLog.Criterion)));
                }

                row.GetCell(4).SetCellValue(issueLog.Description);
                row.GetCell(5).SetCellValue(issueLog.Impact);
                row.GetCell(6).SetCellValue("Current");
                row.GetCell(7).SetCellValue("To be Determined");
            }
        }

        /// <summary>
        /// Defines the conditional formatting for the issue sheet.
        /// </summary>
        /// <param name="sheet">the issue sheet.</param>
        private void DefineColourFormattingIssueSheet(ISheet sheet)
        {
            // Define formatting.
            XSSFSheetConditionalFormatting sCF = (XSSFSheetConditionalFormatting)sheet.SheetConditionalFormatting;

            // Fill Red if High
            XSSFConditionalFormattingRule cfRed =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, "\"High\"");
            XSSFPatternFormatting fillRed = (XSSFPatternFormatting)cfRed.CreatePatternFormatting();
            fillRed.FillBackgroundColor = IndexedColors.Red.Index;
            fillRed.FillPattern = FillPattern.SolidForeground;

            // Fill Orange if Medium
            XSSFConditionalFormattingRule cfOrange =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, "\"Medium\"");
            XSSFPatternFormatting fillOrange = (XSSFPatternFormatting)cfOrange.CreatePatternFormatting();
            fillOrange.FillBackgroundColor = IndexedColors.Gold.Index;
            fillOrange.FillPattern = FillPattern.SolidForeground;

            // Fill yellow if low
            XSSFConditionalFormattingRule cfYellow =
                (XSSFConditionalFormattingRule)sCF.CreateConditionalFormattingRule(ComparisonOperator.Equal, "\"Low\"");
            XSSFPatternFormatting fillYellow = (XSSFPatternFormatting)cfYellow.CreatePatternFormatting();
            fillYellow.FillBackgroundColor = IndexedColors.LightYellow.Index;
            fillYellow.FillPattern = FillPattern.SolidForeground;

            // this is in the row for impact
            CellRangeAddress[] cfRange =
            {
                CellRangeAddress.ValueOf($"F4:F{4 + this.IssueList.Count}"),
            };

            sCF.AddConditionalFormatting(cfRange, new XSSFConditionalFormattingRule[] { cfRed, cfOrange, cfYellow });
        }

        /// <summary>
        /// In the Checklist sheet, it finds the row index who's Criteria id is equal to the key.
        /// </summary>
        /// <param name="key">the id to find.</param>
        /// <param name="sheet">Checklist Sheet.</param>
        /// <returns>The row index.</returns>
        private int FindIdWithValue(string key, ISheet sheet)
        {
            int id = -1;
            for (int rowIndex = 12; rowIndex < 56; rowIndex++)
            {
                string cellValue = sheet.GetRow(rowIndex).GetCell(0).ToString();
                if (key.Equals(cellValue))
                {
                    id = rowIndex;
                }
            }

            return id;
        }
    }
}