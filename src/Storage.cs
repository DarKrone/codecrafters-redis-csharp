using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    class Storage
    {
        public static readonly Storage Instance = new Storage();

        private MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public void AddToData(string key, string value)
        {
            cache.Set(key, value);
        }

        public void AddToStorageWithExpiryTimeStamp(string key, string value, ulong expiry)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(expiry);
            cache.Set(key, value, dateTime);
        }

        public void AddToStorageWithExpiryMilliseconds(string key, string value, ulong expiry)
        {
            cache.Set(key, value, DateTime.Now.AddMilliseconds(expiry));
        }

        public bool TryGetFromDataByKey(string key, out string value)
        {
            string? result;
            if (cache.TryGetValue(key, out result))
            {
                value = result!;
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
            cache.Remove(key);
        }

        public void ClearAllData()
        {
            cache.Clear();
        }

        public string[] GetAllKeys()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field!.GetValue(cache) as ICollection;
            var items = new List<string>();
            if (collection != null)
            foreach (var item in collection)
            {
                var methodInfo = item.GetType().GetProperty("Key");
                var val = methodInfo!.GetValue(item);
                items.Add(val!.ToString()!);
            }
            return items.ToArray();
        }
    }
}
