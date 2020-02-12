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
        public string FileLocation { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\AODA_Result.xlsx";

        /// <summary>
        /// Writes the aoda results to the excel file.
        /// </summary>
        public void WriteToExcel()
        {
            IWorkbook workbook = null;

            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\AODA_Template.xlsx";
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
            ISheet sheet = null;

            // Define styles
            ICellStyle passStyle = workbook.CreateCellStyle();
            passStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BrightGreen.Index;
            passStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle failStyle = workbook.CreateCellStyle();
            failStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            failStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle commentStyle = workbook.CreateCellStyle();
            commentStyle.WrapText = true;

            // get the checklist sheet to modify.
            sheet = workbook.GetSheet(ResourceHelper.GetString("SheetCheckList"));

            foreach (string key in this.ExcelData.Keys)
            {
                int rowId = this.FindIdWithValue(key, sheet);
                int colIndex = 3;

                if (rowId >= 0)
                {
                    foreach (string col in this.ExcelData[key])
                    {
                        // if this is the comment column
                        if (colIndex == 3 + int.Parse(ResourceHelper.GetString("CommentColumn")))
                        {
                            // only put comments on rows that fail.
                            if (this.ExcelData[key][int.Parse(ResourceHelper.GetString("CriteriaColumn"))].Equals("Fail"))
                            {
                                sheet.GetRow(rowId).GetCell(colIndex).CellStyle = commentStyle;
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

        private void UpdateIssueSheet(IWorkbook workbook)
        {
            ISheet sheet = null;

            // set the date
            string date = DateTime.Now.ToString("dd/MM/yyyy");

            // Define styles
            ICellStyle passStyle = workbook.CreateCellStyle();
            passStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
            passStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle failStyle = workbook.CreateCellStyle();
            failStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            failStyle.FillPattern = FillPattern.SolidForeground;

            // get the checklist sheet to modify.
            sheet = workbook.GetSheet(ResourceHelper.GetString("SheetIssueLog"));

            // get all the criterion options.
            List<string> criterionOptions = new List<string>();
            for (int i = 1; i < 38; i++)
            {
                criterionOptions.Add(sheet.GetRow(i).GetCell(14).ToString());
            }

            for (int x = 0; x < this.IssueList.Count; x++)
            {
                IssueLog issueLog = this.IssueList[x];
                IRow row = sheet.GetRow(3 + x);
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

        private int FindIdWithValue(string key, ISheet sheet)
        {
            Console.WriteLine("_______");
            Console.WriteLine(key);
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
