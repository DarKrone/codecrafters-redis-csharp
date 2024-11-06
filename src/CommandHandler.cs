using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    internal class CommandHandler
    {
        public static void HandleCommandArray(Socket socket, string[] commands)
        {
            int pointer = 0;
            while (pointer < commands.Length)
            {
                switch (commands[pointer++])
                {
                    case "ECHO":
                        pointer++;
                        EchoCommand(socket ,commands[pointer]);
                        break;
                    case "PING":
                        PingCommand(socket);
                        break;
                }
            }
        }

        private static void EchoCommand(Socket socket, string echoText)
        {
            string msg = Resp.MakeBulkString(echoText);
            Console.WriteLine($"Sending echo message - {msg}");
            socket.SendAsync(Encoding.UTF8.GetBytes(msg));
        }

        private static void PingCommand(Socket socket)
        {
            string msg = Resp.MakeSimpleString("PONG");
            Console.WriteLine($"Sending pong message - {msg}");
            socket.SendAsync(Encoding.UTF8.GetBytes(msg));
        }
    }
}
