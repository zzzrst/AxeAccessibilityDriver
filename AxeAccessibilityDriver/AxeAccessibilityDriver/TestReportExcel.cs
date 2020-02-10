using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AxeAccessibilityDriver
{
    public class TestReportExcel
    {
        Dictionary<string,Dictionary<ColumnNames, string>> ExcelData;

        public string ProjectName;
        public string ProjectUrl;
        public string Date;
        public string fileLocation;

        public void WriteToExcel()
        {
            IWorkbook workbook = null;

            ISheet sheet = null;

            string resultFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"//AODA_Result_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.xlsx";

            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook = new XSSFWorkbook();
                sheet = workbook.CreateSheet();

                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;

                IRow row = sheet.CreateRow(0);
                ICell cell = row.CreateCell(0);

                cell.CellStyle = cellStyle;
                cell.SetCellValue("Hi there");

                workbook.Write(fileStream);
            }
        }

        public enum ColumnNames
        {
            Id,
            Level,
            Criteria,
            Comments
        }
    }
}
