using CMG.License.Shared.DataTypes;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFilesParsingService
    {
        LogFile ParseLogFileEvents(LogFile logFile);
    }
}