using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Table.PivotTable;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CMG.License.Services.Impls
{
    public class LogFilesExcelProviderService : ILogFilesExcelProviderService
    {
        public void GenerateDenialsSheet(List<LogRptDto> ReportRows, ExcelPackage xlsFile)
        {
            throw new System.NotImplementedException();
        }

        public void GenerateRawDataSheet(List<LogRptDto> ReportRows, ExcelPackage xlsFile)
        {
            var rowDataSheet = xlsFile.Workbook.Worksheets.Add("Raw Data2");
            var dataRange = rowDataSheet.Cells["A1"].LoadFromCollection(
                from r in ReportRows
                orderby r.OutTime
                select r,
                true, OfficeOpenXml.Table.TableStyles.Light13);
            //Format table
            rowDataSheet.Cells[2, 1, dataRange.End.Row, 1].Style.Numberformat.Format = "mm/dd/yy hh:mm";
            rowDataSheet.Cells[2, 2, dataRange.End.Row, 2].Style.Numberformat.Format = "mm/dd/yy hh:mm";
            rowDataSheet.Cells[2, 3, dataRange.End.Row, 3].Style.Numberformat.Format = "mm/dd/yy hh:mm";
            rowDataSheet.Cells[2, 10, dataRange.End.Row, 10].Style.Numberformat.Format = "[h]:mm:ss";
            rowDataSheet.Cells[2, 11, dataRange.End.Row, 11].Style.Numberformat.Format = "[h]:mm:ss";

            dataRange.AutoFitColumns();

            var wsPivot = xlsFile.Workbook.Worksheets.Add("PivotSimple");
            var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange, "testPivotTbl");

            //Add CheckOut Time in Rows field
            pivotTable.RowFields.Add(pivotTable.Fields[1]).AddDateGrouping(eDateGroupBy.Days);
            //Add Product in Columns field
            pivotTable.ColumnFields.Add(pivotTable.Fields[3]);
            //Add Current in use as dataField
            var dataField = pivotTable.DataFields.Add(pivotTable.Fields[8]);
            dataField.Function = DataFieldFunctions.Max;
            //Add pivot chart
            var chart = wsPivot.Drawings.AddChart("PivotChart", eChartType.ColumnClustered, pivotTable);
            chart.Style = eChartStyle.Style8;
            chart.SetPosition(1, 0, 4, 0);
            chart.SetSize(600, 400);
        }
    }
}