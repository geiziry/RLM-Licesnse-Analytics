using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
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

            #region MyRegion
            //var delimeter = ByteString.FromString("\r\n");
            //var getLines =
            //    Flow.Create<ByteString>()
            //    .Via(Framing.Delimiter(delimeter, maximumFrameLength: 256, allowTruncation: true))
            //    .Select(bytes => bytes.ToString());

            //var source = FileIO.FromFile(new FileInfo(logFile.Path))
            //                    .Via(getLines)
            //                    .MapMaterializedValue(_ => NotUsed.Instance);

            //var parseFlow = Flow.Create<string>().Select(logFile.ParseLine);
            //var sink = Sink.Ignore<Task<bool>>();


            //await source
            //    .Via(parseFlow)
            //    .ToMaterialized(sink, Keep.Right)
            //    .Run(actorSystem.Materializer());

            #endregion

            var lines = File.ReadAllLines(logFile.Path);
            var startLine = lines.FirstOrDefault(l =>
                        l.StartsWith(LogEvents.START.ToString()));
            logFile.ParseStart(startLine);
            logFile.InitializeProgress(lines);
            await Source.From(lines)
                .SelectAsyncUnordered(3, logFile.ParseLine)
                .RunWith(Sink.Ignore<bool>(), actorSystem.Materializer());
            return logFile;
        }
    }
}