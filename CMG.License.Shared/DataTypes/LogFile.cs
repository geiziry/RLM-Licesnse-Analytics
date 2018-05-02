using System.Collections.Generic;
using System.IO;

namespace CMG.License.Shared.DataTypes
{
    public enum CheckIn
    {
        why, product, version, user, host, isv_def, count, cur_use, cur_resuse, server_handle, mm_dd, time
    }
    public enum CheckOut
    {
        product, version, pool, user, host, isv_def, count, cur_use, cur_resuse, server_handle, share_handle, process_id, project, requested_product, requested_version, mm_dd, time
    }
    public enum Deny
    {
        product, version, user, host, isv_def, count, why, last_attempt, pid, mm_dd, time
    }
    public enum Start
    {
        server_name, date, time
    }
    public enum LogEvents { IN,OUT,DENY,START}

    public class LogFile
    {
        private readonly string filePath;

        public LogFile(string filePath)
        {
            this.filePath = filePath;
            CheckIns = new List<List<string>>();
            CheckOuts = new List<List<string>>();
            Denials = new List<List<string>>();
        }

        public bool Exists()
        {
            return File.Exists(filePath);
        }
        public string Path { get { return filePath; }}
        public List<List<string>> CheckIns { get; set; }
        public List<List<string>> CheckOuts { get; set; }
        public List<List<string>> Denials { get; set; }
        public List<string> Start { get; set; }
    }
}