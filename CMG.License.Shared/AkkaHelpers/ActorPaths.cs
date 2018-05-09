namespace CMG.License.Shared.AkkaHelpers
{
    public static class ActorPaths
    {
        public static readonly ActorMetaData OpenLogFileCoordinatorActor = new ActorMetaData("OpenLogFileCoordinatorActor");
        public static readonly ActorMetaData ProgressTellerActor = new ActorMetaData("ProgressTellerActor", OpenLogFileCoordinatorActor);
        public static readonly ActorMetaData LogFileReportGeneratorActor = new ActorMetaData("LogFileReportGeneratorActor", OpenLogFileCoordinatorActor);
    }
}