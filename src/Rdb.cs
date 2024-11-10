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
        public static readonly Rdb Instance = new();

        private readonly Dictionary<string, string> config = [];

        public string GetConfigValueByKey(string key)
        {
            return config[key];
        }

        public void SetConfig(string key, string value)
        {
            config[key] = value;
        }

        public void ReadDb()
        {
            string hexString;
            using (FileStream fstream = File.OpenRead($"{config["dir"]}/{config["dbfilename"]}"))
            {//52-45-44-49-53-30-30-31-31-
             //FA-
             //0A-72-65-64-69-73-2D-62-69-74-73-C0-40-FA-09-72-65-64-69-73-2D-76-65-72-05-37-2E-32-2E-30-
             //FE-00-
             //FB-
             //01-
             //00-
             //00-
             //09-70-69-6E-65-61-70-70-6C-65-
             //0A-73-74-72-61-77-62-65-72-72-79-
             //FF-
             //79-CF-C9-00-1A-09-2C-CB-0A
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                hexString = BitConverter.ToString(buffer).Replace("-", "");
            }

            int n = Convert.ToInt32(hexString.Substring(hexString.IndexOf("FB") + 2, 2), 16); //size of table

            string dbHexString = hexString[(hexString.IndexOf("FB") + 6)..];
            for (int i = 0; i < n; i++)
            {
                string indicator = dbHexString.Substring(0, 2);
                switch (indicator)
                {
                    case "00":
                        dbHexString = dbHexString[2..];
                        int length = Convert.ToInt32(FromHexToString(dbHexString.Substring(0, 2)), 16);
                        dbHexString = dbHexString[2..];
                        string key = FromHexToString(dbHexString.Substring(0, length * 2));
                        dbHexString = dbHexString[(length * 2)..];
                        length = Convert.ToInt32(FromHexToString(dbHexString.Substring(0, 2)), 16);
                        dbHexString = dbHexString[2..];
                        string value = FromHexToString(dbHexString.Substring(0, length * 2));
                        dbHexString = dbHexString[(length * 2)..];

                        Console.WriteLine(key);
                        Console.WriteLine(value);

                        Storage.Instance.AddToData(key, value);
                        break;
                }
            }
        }

        private static string FromHexToString(string hexString)
        {
            byte[] raw = new byte[hexString.Length / 2];
            for(int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.UTF8.GetString(raw);
        }
    }
}
