using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    internal class Rdb
    {
        public static readonly Rdb Instance = new Rdb();

        private Dictionary<string, string> config = new Dictionary<string, string>()
        {
            ["dir"] = "/tmp/redis-files",
            ["dbfilename"] = "dump.rdb",
        };

        public string GetConfigValueByKey(string key)
        {
            return config[key];
        }
    }
}
