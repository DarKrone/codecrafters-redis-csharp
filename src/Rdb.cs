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
            { //52-45-44-49-53-30-30-31-31-EF-BF-BD-09-72-65-64-69-73-2D-76-65-72-05-37-2E-32-2E-30-EF-BF-BD-0A-72-65-64-69-73-2D-62-69-74-73-EF-BF-BD-40-EF-BF-BD-00-EF-BF-BD-01-00-00-04-70-65-61-72-09-62-6C-75-65-62-65-72-72-79-EF-BF-BD-EF-BF-BD-4A-D0-90-79-EF-BF-BD-EF-BF-BD-0A
                string text = await reader.ReadToEndAsync();
                byte[] bstring = Encoding.UTF8.GetBytes(text);
                foreach (byte b in bstring)
                {
                    Console.Write(b + "-");
                }
            }
        }
    }
}
