using System;

namespace CMG.License.Shared.DataTypes
{
    public struct CheckOutDto
    {
        public string Product { get; set; }
        public string Version { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public int Count { get; set; }
        public int CurrentInUse { get; set; }
        public string ServerHandle { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public struct CheckInDto
    {
        public string Product { get; set; }
        public string Version { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public int Count { get; set; }
        public int CurrentInUse { get; set; }
        public string ServerHandle { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public struct DenyDto
    {
        public string Product { get; set; }
        public string Version { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public int Count { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public struct ProductDto
    {
        public string Name { get; set; }
        public int InstalledCount { get; set; }
    }

    public struct StartDto
    {
        public string ServerName { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
