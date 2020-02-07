using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AxeAccessibilityDriver.Axe
{
    class TestReportExcel
    {
        Dictionary<string,Dictionary<ColumnNames, string>> ExcelData;

        string ProjectName;
        string ProjectUrl;
        string Date;

        public void WriteToExcel()
        {
            IWorkbook workbook = null;
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
