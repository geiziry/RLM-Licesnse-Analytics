﻿namespace CMG.License.Shared.AkkaHelpers
{
    public static class ActorPaths
    {
        public static readonly ActorMetaData OpenLogFileCoordinatorActor = new ActorMetaData("OpenLogFileCoordinatorActor");
        public static readonly ActorMetaData ProgressTellerActor = new ActorMetaData("ProgressTellerActor", OpenLogFileCoordinatorActor);
        public static readonly ActorMetaData LogFileReportGeneratorActor = new ActorMetaData("LogFileReportGeneratorActor", OpenLogFileCoordinatorActor);
        public static readonly ActorMetaData LogFilesParsingActor = new ActorMetaData("LogFilesParsingActor", LogFileReportGeneratorActor);
        public static readonly ActorMetaData logFilesExcelProviderActor = new ActorMetaData("logFilesExcelProviderActor", LogFileReportGeneratorActor);
        public static readonly ActorMetaData getCheckInsActor = new ActorMetaData("getCheckInsActor", LogFileReportGeneratorActor);
    }
}