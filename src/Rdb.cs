﻿using System;
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
                hexString = BitConverter.ToString(buffer).Replace("-", "");
            }

            int tableSize = Convert.ToInt32($"0x{hexString.Substring(hexString.IndexOf("FB") + 2, 2)}", 16); //size of table
            Console.WriteLine($"Table size - {tableSize}");
            int expiryTableSize = Convert.ToInt32($"0x{hexString.Substring(hexString.IndexOf("FB") + 4, 2)}", 16); //size of expiry table
            Console.WriteLine($"Table expiry size - {expiryTableSize}");

            string dbHexString = hexString[(hexString.IndexOf("FB") + 6)..];
            for (int i = 0; i < tableSize; i++)
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
                            string hexSeconds = dbHexString.Substring(0, 16);
                            hexSeconds = BigEndToLitEndHex(hexSeconds);
                            ulong milliseconds = ulong.Parse(hexSeconds, System.Globalization.NumberStyles.HexNumber);
                            dbHexString = dbHexString[16..];
                            ReadKeyValue(ref dbHexString, milliseconds);
                            break;
                        }
                    case "FD":
                        {
                            dbHexString = dbHexString[2..];
                            string hexSeconds = dbHexString.Substring(0, 8);
                            hexSeconds = BigEndToLitEndHex(hexSeconds);
                            ulong milliseconds = ulong.Parse(hexSeconds, System.Globalization.NumberStyles.HexNumber);
                            dbHexString = dbHexString[8..];
                            ReadKeyValue(ref dbHexString, milliseconds);
                            break;
                        }
                }
            }
        }
 
        private string BigEndToLitEndHex(string hex)
        {
            string[] hexArray = new string[hex.Length / 2];
            for(int i = 0; i < hexArray.Length; i++)
            {
                hexArray[i] = hex[i * 2].ToString() + hex[i * 2 + 1].ToString();
            }
            Array.Reverse(hexArray);
            hex = String.Join("", hexArray);
            return hex;
        }

        private void ReadKeyValue(ref string dbHexString, ulong milliseconds = 0)
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

            Console.WriteLine($"Key - {key}");
            Console.WriteLine($"Value - {value}");
            Console.WriteLine($"Expiry - {milliseconds}");

            if (milliseconds > 0)
            {
                Storage.Instance.AddToStorageWithExpiryTimeStamp(key, value, milliseconds);
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
