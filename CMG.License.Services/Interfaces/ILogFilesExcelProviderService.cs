using CMG.License.Shared.DataTypes;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFilesExcelProviderService
    {
        void FillXlsxTemplate(LogFile logFile, string excelTemplate);
    }
}