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
        /// The data to write to the excel sheet.
        /// </summary>
        public Dictionary<string, List<string>> ExcelData;

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string ProjectName;

        /// <summary>
        /// The url of the project.
        /// </summary>
        public string ProjectUrl;

        /// <summary>
        /// The date this was modified.
        /// </summary>
        public string Date;

        /// <summary>
        /// The location to save the file to.
        /// </summary>
        public string FileLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestReportExcel"/> class.
        /// Creates a new Excel report.
        /// </summary>
        public TestReportExcel()
        {
            this.ExcelData = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Writes the aoda results to the excel file.
        /// </summary>
        public void WriteToExcel()
        {
            IWorkbook workbook = null;

            ISheet sheet = null;

            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\AODA_Template.xlsx";
            string resultFilePath = this.FileLocation;

            using (FileStream templateFS = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(templateFS);
            }

            // Define styles
            ICellStyle passStyle = workbook.CreateCellStyle();
            passStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
            passStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle failStyle = workbook.CreateCellStyle();
            failStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            failStyle.FillPattern = FillPattern.SolidForeground;

            // get the sheet to modify.
            sheet = workbook.GetSheet(ResourceHelper.GetString("SheetCheckList"));

            foreach (string key in this.ExcelData.Keys)
            {
                int rowId = this.FindIdWithValue(key, sheet);
                int colIndex = 2;

                foreach (string col in this.ExcelData[key])
                {
                    IRow row = sheet.GetRow(rowId);
                    ICell cell = row.GetCell(colIndex);
                    cell.SetCellValue(col);
                    colIndex++;
                }
            }


            // write to output.
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
                workbook.Close();
            }
        }

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
