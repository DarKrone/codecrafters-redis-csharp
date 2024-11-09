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
            using (StreamReader reader = new StreamReader($"{config["dir"]}/{config["dbfilename"]}"))
            {
                string text = await reader.ReadToEndAsync();
                byte[] bstring = Encoding.UTF8.GetBytes(text);
                string hexString = Convert.ToHexString(bstring);
                Console.WriteLine(text);
            }
        }
    }
}
