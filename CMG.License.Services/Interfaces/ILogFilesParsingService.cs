using CMG.License.Shared.DataTypes;
using System.Threading.Tasks;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFilesParsingService
    {
        Task<LogFile> ParseLogFileEventsAsync(LogFile logFile);
    }
}