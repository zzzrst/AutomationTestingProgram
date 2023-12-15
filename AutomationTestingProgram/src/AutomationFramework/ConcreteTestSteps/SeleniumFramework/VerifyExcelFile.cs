// <copyright file="VerifyExcelFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class VerifyExcelFile : TestStep
    {
        private string resultFilePath = string.Empty;

        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyExcelFile";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // get object identifier and arguments
            string expectedFilePath = this.Arguments["object"];
            string actualFilePath = this.Arguments["value"];
            string comment = this.Arguments["comment"];

            // parse comment to get coordinates of bounding box
            List<string> valueSeperated = comment.Split(';').ToList();
            if (valueSeperated.Count != 2)
            {
                Logger.Info("Was not formatted properly. Must be SheetName;x1,y1:x2,y2");
            }

            string sheetName = valueSeperated[0];
            int topX = int.Parse(valueSeperated[1].Split(':')[0].Split(',')[0]) - 1;
            int topY = int.Parse(valueSeperated[1].Split(':')[0].Split(',')[1]) - 1;
            int botX = int.Parse(valueSeperated[1].Split(':')[1].Split(',')[0]) - 1;
            int botY = int.Parse(valueSeperated[1].Split(':')[1].Split(',')[1]) - 1;

            bool passed = this.CompareExcel(expectedFilePath, actualFilePath, sheetName, topX, topY, botX, botY);
            this.TestStepStatus.Actual = passed ? "Both files were the same" : "There were differences!. Please find the result file on the desktop";

            // CR: Attach excel files that were compared. Attach resulting file if there were differences.
            InformationObject.TestSetData.AddAttachment(expectedFilePath);
            InformationObject.TestSetData.AddAttachment(actualFilePath);
            if (!passed)
            {
                InformationObject.TestSetData.AddAttachment(this.resultFilePath);
            }

            this.TestStepStatus.RunSuccessful = passed;
        }

        /// <summary> Open an Excel file (xls or xlsx) and convert it into a DataTable.
        /// THE FIRST ROW MUST CONTAIN THE NAMES OF THE FIELDS. </summary>
        /// <param name="filePath"> The filepath to the expected excel. </param>
        /// <param name="filePath2"> The filepath to the actual excel. </param>
        /// <param name="sheetName"> The name of the sheet to compare. If incorrect, will use index 0.</param>
        /// <param name="topX"> The top x coordinate. </param>
        /// <param name="topY"> The top y coordinate. </param>
        /// <param name="botX"> The bottom x coordinate. </param>
        /// <param name="botY"> The bottom y coordinate. </param>
        private bool CompareExcel(string filePath, string filePath2, string sheetName, int topX, int topY, int botX, int botY)
        {
            bool difference = false;

            filePath = this.VerifyAndConvert(filePath);
            filePath2 = this.VerifyAndConvert(filePath2);
            this.resultFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"//CompareExcelResult_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.xlsx";

            using (FileStream expectedFS = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream actualFS = new FileStream(filePath2, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream resultFS = new FileStream(this.resultFilePath, FileMode.Create, FileAccess.Write))
                    {
                        // set up workbooks and sheets to use.
                        // open both XLS and XLSX
                        IWorkbook expectedWorkbook = WorkbookFactory.Create(expectedFS);
                        IWorkbook actualWorkbook = WorkbookFactory.Create(actualFS);
                        IWorkbook resultWorkbook = new XSSFWorkbook();

                        // set sheet. If null, use the first sheet.
                        ISheet expectedSheet = expectedWorkbook.GetSheet(sheetName) ?? expectedWorkbook.GetSheetAt(0);
                        ISheet actualSheet = actualWorkbook.GetSheet(sheetName) ?? actualWorkbook.GetSheetAt(0);
                        ISheet resultSheet = resultWorkbook.CreateSheet();

                        // create cell style for result file.
                        ICellStyle resultCellStyle = resultWorkbook.CreateCellStyle();
                        resultCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                        resultCellStyle.FillPattern = FillPattern.SolidForeground;

                        // Error if our range is greater than our worksheet last row number
                        int lastRow = botX;
                        if (botX > expectedSheet.LastRowNum || botX > actualSheet.LastRowNum)
                        {
                            lastRow = Math.Min(expectedSheet.LastRowNum, actualSheet.LastRowNum);
                            Logger.Info($"The range is not valid. Provided last row is {botX + 1} while sheet only has {lastRow + 1}");
                        }

                        // loop from topX to lastRow
                        for (int rowIndex = topX; rowIndex <= lastRow; rowIndex++)
                        {
                            IRow expectedRow = expectedSheet.GetRow(rowIndex);
                            IRow actualRow = actualSheet.GetRow(rowIndex);
                            IRow resultRow = resultSheet.CreateRow(rowIndex);

                            // null is when the row only contains empty cells
                            if (expectedRow == null)
                            {
                                if (actualRow != null)
                                {
                                    Logger.Info($"Expected row is empty, but actualRow isn't. We found {actualRow}.");
                                }
                            }
                            else
                            {
                                // set up for column index.
                                int lastColIndex = botY;
                                if (lastColIndex > expectedRow.Cells.Count || lastColIndex > actualRow.Cells.Count)
                                {
                                    lastColIndex = Math.Min(expectedRow.Cells.Count, actualRow.Cells.Count);
                                }

                                for (int colIndex = topY; colIndex <= lastColIndex; colIndex++)
                                {
                                    ICell expectedCell = expectedRow.GetCell(colIndex);
                                    ICell actualCell = actualRow.GetCell(colIndex);
                                    ICell resultCell = resultRow.CreateCell(colIndex);

                                    string expectedValue = expectedCell == null ? string.Empty : expectedCell.ToString();
                                    string actualValue = actualCell == null ? string.Empty : actualCell.ToString();

                                    // if both are null, then expetedCell will be equal to actual cell
                                    if (expectedValue != actualValue)
                                    {
                                        string cellValue = $"Expected {expectedValue} but found {actualValue} at ({rowIndex + 1},{colIndex + 1})";
                                        resultCell.CellStyle = resultCellStyle;
                                        resultCell.SetCellValue(cellValue);
                                        Logger.Info(cellValue);
                                        difference = true;
                                    }
                                    else
                                    {
                                        resultCell.SetCellValue($"{expectedCell}");
                                    }
                                }
                            }
                        }

                        resultWorkbook.Write(resultFS);

                        if (!difference)
                        {
                            File.Delete(this.resultFilePath);
                        }
                    }
                }
            }

            return !difference;
        }

        /// <summary>
        /// Verifies that the file exists and convert to xlsx if needed.
        /// </summary>
        /// <param name="filePath">the path to the file.</param>
        private string VerifyAndConvert(string filePath)
        {
            // check if file was found / not.
            if (!File.Exists(filePath))
            {
                throw new Exception($"File {filePath} was not found.");
            }

            // Convert if provided file has .csv extension.
            if (Path.GetExtension(filePath) == ".csv")
            {
                string excelFilePath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".xlsx";
                this.ConvertCSVtoXLSX(excelFilePath, filePath);
                filePath = excelFilePath;
            }

            return filePath;
        }

        /// <summary>
        /// Converts CSV to .xlsx file.
        /// </summary>
        /// <param name="excelFileName"> Name of the file. </param>
        /// <param name="csvFilePath"> Path to the CSV File. </param>
        private void ConvertCSVtoXLSX(string excelFileName, string csvFilePath)
        {
            using (FileStream resultFS = new FileStream(excelFileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamReader csvFS = new StreamReader(csvFilePath))
                {
                    IWorkbook excel = new XSSFWorkbook();
                    ISheet sheet = excel.CreateSheet();

                    string lineA;

                    int rowCount = 0;
                    while ((lineA = csvFS.ReadLine()) != null)
                    {
                        var values = lineA.Split(',').ToList();

                        IRow row = sheet.CreateRow(rowCount);
                        int colCount = 0;
                        foreach (string val in values)
                        {
                            row.CreateCell(colCount).SetCellValue(val);
                            colCount++;
                        }

                        rowCount++;
                    }

                    excel.Write(resultFS);
                }
            }
        }
    }
}
