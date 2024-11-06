using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        // You can use print statements as follows for debugging, they'll be visible when running tests.
        Console.WriteLine("Logs from your program will appear here!");

        // Uncomment this block to pass the first stage
        TcpListener server = new TcpListener(IPAddress.Any, 6379);
        server.Start();

        while(true)
        {
            Socket clientSocket = server.AcceptSocket(); // wait for client
            Thread connThread = new Thread(() => { HandleConnection(clientSocket); });
            connThread.Start();
        }
    }

    private static void HandleConnection(Socket socket)
    {
        byte[] buffer = new byte[1024];
        while (socket.Connected)
        {
            socket.Receive(buffer);
            socket.SendAsync(Encoding.UTF8.GetBytes("+PONG\r\n"));
        }
    }
}

