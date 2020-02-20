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
        /// The value for pass in the excel document.
        /// </summary>
        public const string PASSVALUE = "Pass";

        /// <summary>
        /// The value for fail in the excel doucment.
        /// </summary>
        public const string FAILVALUE = "Fail";

        /// <summary>
        /// The not applicable value.
        /// </summary>
        public const string NOTAPPLICABLEVALUE = "Criteria not applicable";

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

            // write to output.
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
                workbook.Close();
            }
        }

        private void UpdateChecklistSheet(IWorkbook workbook)
        {
            // get the checklist sheet to modify.
            ISheet sheet = workbook.GetSheet(ResourceHelper.GetString("SheetCheckList"));

            this.DefineColourFormattingChecklistSheet(sheet);

            foreach (string key in this.ExcelData.Keys)
            {
                int rowId = this.FindIdWithValue(key, sheet);
                int colIndex = 3;

                if (rowId >= 0)
                {
                    foreach (string col in this.ExcelData[key])
                    {
                        if (colIndex == 3 + int.Parse(ResourceHelper.GetString("CommentColumn")))
                        {
                            // only put comments on rows that fail.
                            if (this.ExcelData[key][int.Parse(ResourceHelper.GetString("CriteriaColumn"))].Equals("Fail"))
                            {
                                sheet.GetRow(rowId).GetCell(colIndex).SetCellValue(col);
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

            // update the total
            workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateFormulaCell(sheet.GetRow(62).GetCell(3));

            // set the date
            sheet.GetRow(3).GetCell(2).SetCellValue(DateTime.Now.ToString());
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

            // this is in the 'Meets Criteria' Column
            CellRangeAddress[] cfRange =
            {
                CellRangeAddress.ValueOf("D13:D26"), CellRangeAddress.ValueOf("D29:D40"),
                CellRangeAddress.ValueOf("D43:D52"), CellRangeAddress.ValueOf("D55:D56"),
            };

            // You can't add 4 at once
            sCF.AddConditionalFormatting(cfRange, new XSSFConditionalFormattingRule[] { cfRed, cfGreen, cfYellow2, });
            sCF.AddConditionalFormatting(cfRange, new XSSFConditionalFormattingRule[] { cfYellow });

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
            for (int i = 1; i < 38; i++)
            {
                criterionOptions.Add(sheet.GetRow(i).GetCell(14).ToString());
            }

            // print out all the issues
            for (int x = 0; x < this.IssueList.Count; x++)
            {
                IssueLog issueLog = this.IssueList[x];
                IRow row;

                // create a new row.
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