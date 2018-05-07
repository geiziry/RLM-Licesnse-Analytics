using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMG.License.Services.Impls
{
    public class LogFilesExcelProviderService : ILogFilesExcelProviderService
    {
        public void FillXlsxTemplate(List<LogRptDto> ReportRows, string excelTemplatePath)
        {
            var xlsFileInfo = new FileInfo(@"C:\Users\mgeiziry\Desktop\test.xlsx");
            //if (!xlsFileInfo.Exists)
            //    return;

            using (var xlsFile=new ExcelPackage(xlsFileInfo))
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
                rowDataSheet.Cells[2, 10, dataRange.End.Row, 10].Style.Numberformat.Format = "hh:mm:ss";
                rowDataSheet.Cells[2, 11, dataRange.End.Row, 11].Style.Numberformat.Format = "hh:mm:ss";

                dataRange.AutoFitColumns();

                var wsPivot = xlsFile.Workbook.Worksheets.Add("PivotSimple");
                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange, "testPivotTbl");

                //Add CheckOut Time in Rows field
                pivotTable.RowFields.Add(pivotTable.Fields[1]);
                //Add Product in Columns field
                pivotTable.ColumnFields.Add(pivotTable.Fields[3]);
                
                var dataField = pivotTable.DataFields.Add(pivotTable.Fields[8]);
                dataField.Function = OfficeOpenXml.Table.PivotTable.DataFieldFunctions.Max;
                pivotTable.DataOnRows = true;
                xlsFile.Save();
            }
        }
    }
}