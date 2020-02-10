using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AxeAccessibilityDriver
{
    public class TestReportExcel
    {
        /// <summary>
        /// The data to write to the excel sheet
        /// </summary>
        public Dictionary<string, Dictionary<ColumnNames, string>> ExcelData;

        public string ProjectName;
        public string ProjectUrl;
        public string Date;
        public string fileLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestReportExcel"/> class.
        /// Creates a new Excel report.
        /// </summary>
        public TestReportExcel()
        {
            this.ExcelData = new Dictionary<string, Dictionary<ColumnNames, string>>();
        }

        /// <summary>
        /// The names of the Columns.
        /// </summary>
        public enum ColumnNames
        {
            /// <summary>
            /// Id of the AODA.
            /// </summary>
            Id,

            /// <summary>
            /// Usualy AA or A.
            /// </summary>
            Level,

            /// <summary>
            /// Pass, Fail or N/A.
            /// </summary>
            Criteria,

            /// <summary>
            /// Any comments provided.
            /// </summary>
            Comments,
        }

        private enum Criteria
        {
            Pass,
            Fail,
            NA,
        }

        /// <summary>
        /// Writes the aoda results to the excel file.
        /// </summary>
        public void WriteToExcel()
        {
            IWorkbook workbook = null;

            ISheet sheet = null;

            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\AODA_Template.xlsx";
            string resultFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"//AODA_Result_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.xlsx";

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
            sheet = workbook.GetSheet("WCAG 2.0 Compliance Checklist");

            IRow row = sheet.GetRow(13 - 1);
            ICell cell = row.GetCell(4 - 1);

            cell.SetCellValue("Pass");

            //write to output.
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
                workbook.Close();
            }
        }
    }
}
