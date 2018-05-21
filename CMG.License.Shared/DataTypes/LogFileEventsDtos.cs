using System;

namespace CMG.License.Shared.DataTypes
{
    public struct CheckInDto
    {
        public int Count { get; set; }
        public int CurrentInUse { get; set; }
        public string Host { get; set; }
        public string Product { get; set; }
        public string ServerHandle { get; set; }
        public DateTime TimeStamp { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
    }

    public struct CheckOutDto
    {
        public int Count { get; set; }
        public int CurrentInUse { get; set; }
        public string Host { get; set; }
        public string Product { get; set; }
        public string ServerHandle { get; set; }
        public DateTime TimeStamp { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
    }
    public struct DenyDto
    {
        public int Count { get; set; }
        public string Host { get; set; }
        public string Product { get; set; }
        public DateTime TimeStamp { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
    }

    public struct ProductDto
    {
        public int InstalledCount { get; set; }
        public string Name { get; set; }
    }

    public struct StartDto
    {
        public string ServerName { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public struct ShutdownDto
    {
        public DateTime TimeStamp { get; set; }
    }

    #region enums

    public enum LogEvents { PRODUCT, IN, OUT, DENY, START, INUSE,SHUTDOWN}

    public static class CheckIn
    {
        public const int why = 1, product = 2, version = 3, user = 4,
            host = 5, isv_def = 6, count = 7, cur_use = 8, cur_resuse = 9,
            server_handle = 10, mm_dd = 11, time = 12;
    }

    public static class CheckOut
    {
        public const int product = 1, version = 2, pool = 3, user = 4, host = 5, isv_def = 6,
            count = 7, cur_use = 8, cur_resuse = 9, server_handle = 10, share_handle = 11,
            process_id = 12, project = 13, requested_product = 14, requested_version = 15, mm_dd = 16, time = 17;
    }

    public static class Deny
    {
        public const int product = 1, version = 2, user = 3, host = 4, isv_def = 5, count = 6,
            why = 7, last_attempt = 8, mm_dd = 9, time = 10;
    }

    public static class InUse
    {
        public const int product = 1, version = 2, pool = 3, user = 4, host = 5, isv_def = 6, count = 7, server_handle = 8, share_handle = 9, process_id = 10, mm_dd = 11, time = 12;
    }

    public static class Product
    {
        public const int name = 1, version = 2, pool = 3, count = 4, reservations = 5,
            soft_limit = 6, hostid = 7, contract = 8, customer = 9, issuer = 10, line_item = 11,
            options = 12, share = 13, max_share = 14, type = 15, named_user_count = 16,
            meter_type = 17, meter_counter = 18, meter_initial_decrement = 19, meter_period = 20, meter_period_decrement = 21;
    }

    public static class Start
    {
        public const int server_name = 1, date = 2, time = 3;
    }

    public static class Shutdown
    {
        public const int user = 1, hostid = 2, mm_dd = 3, time = 4;
    }
    #endregion enums
}