using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace SocketServer
{

    /// <summary>
    ///     服务端监听客户端信息，一旦有发送过来的信息，便立即处理
    /// </summary>
    internal class Program
    {

        private static void Main(string[] args)
        {
            var server = new SocketServer();
            server.ListenFile("E:\\abc.jpg", null, 81);
            Console.ReadKey();
        }

    }

    public class SocketServer
    {

        private const int Bufferlength = 200;
        private byte[] _buffer;


        public void ListenFile(string newFilename, string ip, int port)
        {
            _buffer = new byte[Bufferlength];
            var ipAddress = string.IsNullOrWhiteSpace(ip) ? IPAddress.Any : IPAddress.Parse(ip);
            var listen = new TcpListener(ipAddress, port);
            listen.Start();
            while (true)
            {
                Console.WriteLine("等待连接");

                //线程会挂在这里，直到客户端发来连接请求
                using (var client = listen.AcceptTcpClient())
                using (var ns = client.GetStream())
                using (var fs = new FileStream(newFilename, FileMode.OpenOrCreate))
                {
                    Console.WriteLine("已经连接");
                    while (true)
                    {
                        var readCount = ns.Read(_buffer, 0, Bufferlength);
                        if (readCount == 0)
                        {
                            break;
                        }
                        fs.Write(_buffer, 0, readCount);
                    }
                }
            }
        }

    }

}