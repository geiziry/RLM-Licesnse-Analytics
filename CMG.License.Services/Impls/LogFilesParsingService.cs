using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CMG.License.Services.Impls
{
    public class LogFilesParsingService : ILogFilesParsingService
    {
        private readonly IActorRefFactory actorSystem;

        public LogFilesParsingService(IActorRefFactory actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public async Task<LogFile> ParseLogFileEventsAsync(LogFile logFile)
        {
            if (!logFile.Exists())
                return null;

            var lines = File.ReadAllLines(logFile.Path);

            var startLine = lines.FirstOrDefault(l =>
                        l.StartsWith(LogEvents.START.ToString()));

            logFile.ParseStart(startLine);
            logFile.InitializeProgress(lines);
            var t = new List<bool>();
            await Source.From(lines)
                .SelectAsyncUnordered(int.MaxValue, logFile.ParseLine)
                .RunWith(Sink.ForEach<bool>(x=>t.Add(x)), actorSystem.Materializer());
            return logFile;
        }
    }
}