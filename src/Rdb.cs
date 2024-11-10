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
            if (!config.ContainsKey("dir") && !config.ContainsKey("dbfilename"))
            {
                return;
            }
            if (!File.Exists($"{config["dir"]}/{config["dbfilename"]}"))
            {
                return;
            }
            string hexString;
            using (FileStream fstream = File.OpenRead($"{config["dir"]}/{config["dbfilename"]}"))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                Console.WriteLine(BitConverter.ToString(buffer));
                hexString = BitConverter.ToString(buffer).Replace("-", "");
            }
            int n = Convert.ToInt32($"0x{hexString.Substring(hexString.IndexOf("FB") + 2, 2)}", 16); //size of table
            Console.WriteLine(n);
            string dbHexString = hexString[(hexString.IndexOf("FB") + 6)..];
            for (int i = 0; i < n; i++)
            {
                string indicator = dbHexString.Substring(0, 2);
                switch (indicator)
                {
                    case "00":
                        ReadKeyValue(ref dbHexString);
                        break;
                    case "FC":
                        {
                            dbHexString = dbHexString[2..];
                            int milliseconds = Int32.Parse(dbHexString.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                            dbHexString = dbHexString[8..];
                            Console.WriteLine(dbHexString);
                            ReadKeyValue(ref dbHexString, milliseconds);
                            Console.WriteLine(dbHexString);
                            break;
                        }
                    case "FD":
                        {
                            dbHexString = dbHexString[2..];
                            int milliseconds = Int32.Parse(dbHexString.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                            dbHexString = dbHexString[4..];
                            ReadKeyValue(ref dbHexString, milliseconds);
                            break;
                        }
                }
            }
        }
        //52-45-44-49-53-30-30-31-31-
        //FA-
        //09-72-65-64-69-73-2D-76-65-72-05-37-2E-32-2E-30-
        //FA-0A-72-65-64-69-73-2D-62-69-74-73-C0-40-FE-00-
        //
        //FB-
        //05-
        //05-
        //FC-
        //00-
        //0C-28-8A-C7-01-00-00-00-06-62-61-6E-61-
        //6E-61-09-72-61-73-70-62-65-72-72-79-FC-00-0C-28-8A-C7-01-00-00-00-06-6F-72-61-6E-67-65-09-70-69-6E-65-61-70-70-6C-65-FC-00-0C-28-8A-C7-01-00-00-00-05-67-72-61-70-65-06-62-61-6E-61-6E-61-FC-00-9C-EF-12-7E-01-00-00-00-05-61-70-70-6C-65-0A-73-74-72-61-77-62-65-72-72-79-FC-00-0C-28-8A-C7-01-00-00-00-04-70-65-61-72-05-6D-61-6E-67-6F-FF-E8-32-4B-92-7F-92-89-7C-0A
        private void ReadKeyValue(ref string dbHexString, int milliseconds = -1)
        {
            dbHexString = dbHexString[2..];
            int length = Convert.ToInt32($"0x{dbHexString.Substring(0, 2)}", 16);
            dbHexString = dbHexString[2..];
            string key = FromHexToString(dbHexString.Substring(0, length * 2));
            dbHexString = dbHexString[(length * 2)..];
            length = Convert.ToInt32($"0x{dbHexString.Substring(0, 2)}", 16);
            dbHexString = dbHexString[2..];
            string value = FromHexToString(dbHexString.Substring(0, length * 2));
            dbHexString = dbHexString[(length * 2)..];

            Console.WriteLine(key);
            Console.WriteLine(value);
            Console.WriteLine(milliseconds);

            if (milliseconds > 0)
            {
                Storage.Instance.AddToStorageWithExpiry(key, value, milliseconds);
            }
            else
            {
                Storage.Instance.AddToData(key, value);
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
