using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    internal class Resp
    {
        public static string[] ParseMessage(string data)
        {
            List<string> result = new List<string>();
            Console.WriteLine(data);
            int n = int.Parse(data.Substring(1, data.IndexOf(@"\r\n") - 1));

            //Console.WriteLine(n);
            data = data.Substring(data.IndexOf(@"\r\n") + 4);
            //Console.WriteLine(data);

            for(int i = 0 ; i < n; i++)
            {
                result.Add(ParseBulkString(ref data));
            }
            return result.ToArray();
        }

        private static string ParseBulkString(ref string data)
        {
            int n = int.Parse(data.Substring(1, data.IndexOf("\\r\\n") - 1));
            data = data.Substring(data.IndexOf(@"\r\n") + 4);
            //Console.WriteLine(data);
            string text = data.Substring(0, n);
            data = data.Substring(data.IndexOf(@"\r\n") + 4);
            //Console.WriteLine(data);

            return text;
        }

        public static string MakeBulkString(string data)
        {
            return $"${data.ToString()?.Length}\\r\\n{data.ToString()}\\r\\n";
        }

        public static string MakeSimpleString(string data)
        {
            return $"+{data.ToString()}\\r\\n";
        }
    }
}
