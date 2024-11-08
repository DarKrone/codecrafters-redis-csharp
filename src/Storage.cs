using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    class Storage
    {
        public static readonly Storage Instance = new Storage();

        private Dictionary<string, string> data = new Dictionary<string, string>();

        public void AddToData(string key, string value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public bool TryGetFromDataByKey(string key, out string value)
        {
            if (data.ContainsKey(key))
            {
                value = data[key];
                return true;
            }
            else
            {
                value = "";
                return false;
            }
        }

        public void RemoveFromData(string key)
        {
            if (!data.ContainsKey(key))
            {
                return;
            }
            else
            {
                data.Remove(key);
            }
        }

        public void ClearAllData()
        {
            data.Clear();
        }

        public Dictionary<string, string> GetAllData()
        {
            return data;
        }
    }
}
