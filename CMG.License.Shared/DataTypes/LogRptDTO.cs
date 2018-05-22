using System;

namespace CMG.License.Shared.DataTypes
{
    public class EpplusIgnoreAttribute : Attribute { }
    public class LogRptDto
    {
        public DateTime RequestTime { get; set; }
        public DateTime OutTime { get; set; }
        public DateTime InTime { get; set; }
        public string Product { get; set; }
        public string Version { get; set; }
        public string UserName { get; set; }
        public string HostName { get; set; }
        public int InstalledCount { get; set; }
        public int InUse { get; set; }
        public TimeSpan WaitTime { get { return OutTime - RequestTime; } }
        public TimeSpan Duration { get { return InTime - OutTime; } }
        [EpplusIgnore]
        public string ServerHandle { get; set; }
    }
}