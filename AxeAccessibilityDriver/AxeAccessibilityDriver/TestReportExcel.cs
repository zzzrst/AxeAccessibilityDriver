using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AxeAccessibilityDriver
{
    class TestReportExcel
    {
        Dictionary<string,Dictionary<ColumnNames, string>> ExcelData;

        string ProjectName;
        string ProjectUrl;
        string Date;
        string fileLocation;

        public void WriteToExcel()
        {
            IWorkbook workbook = null;

            ISheet sheet = null;

            using (FileStream FS = new FileStream(fileLocation, FileMode.Open, FileAccess.ReadWrite))
            {

            };
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
