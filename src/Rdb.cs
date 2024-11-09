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

        private Dictionary<string, string> config = new Dictionary<string, string>();

        public string GetConfigValueByKey(string key)
        {
            return config[key];
        }

        public void SetConfig(string key, string value)
        {
            config[key] = value;
        }

        public async void ReadDb()
        {
            using (FileStream fstream = File.OpenRead($"{config["dir"]}/{config["dbfilename"]}"))
            { 
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string hexString = BitConverter.ToString(buffer);
                Console.WriteLine(hexString);
            }
        }
    }
}
