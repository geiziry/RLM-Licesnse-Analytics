using CMG.License.Shared.DataTypes;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFilesParsingService
    {
        void ParseLogFileEvents(ref LogFile logFile);
    }
}