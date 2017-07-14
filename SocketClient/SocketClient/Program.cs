using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace SocketClient
{

    internal class Program
    {

        /// <summary>
        ///     客户端
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var filename = @"F:\素材库\图片\建筑\a8a4d24f6a30d055008b7dfe3fc1c0b5.jpg";
            var ip = "127.0.0.1";
            var socket = new SocketClient();
            socket.SendFile(filename, ip, 81);
            Console.ReadKey();
        }


        private static void SendImageToServer(string imgURl)
        {
            if (!File.Exists(imgURl)) return;

            //创建一个文件流打开图片
            var fs = File.Open(imgURl, FileMode.Open);

            //声明一个byte数组接受图片byte信息
            var fileBytes = new byte[fs.Length];
            using (fs)
            {
                //将图片byte信息读入byte数组中
                fs.Read(fileBytes, 0, fileBytes.Length);
                fs.Close();
            }

            //找到服务器的IP地址
            var address = IPAddress.Parse("127.0.0.1");

            //创建TcpClient对象实现与服务器的连接
            var client = new TcpClient();

            //连接服务器
            client.Connect(address, 81);
            using (client)
            {
                //连接完服务器后便在客户端和服务端之间产生一个流的通道
                var ns = client.GetStream();
                using (ns)
                {
                    //通过此通道将图片数据写入网络流，传向服务器端接收
                    ns.Write(fileBytes, 0, fileBytes.Length);
                }
            }
        }

    }

    public class SocketClient
    {

        public void SendFile(string fileName, string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            byte[] fileBytes;
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var fileLength = fs.Length;
                fileBytes = new byte[fileLength];
                fs.Read(fileBytes, 0, fileBytes.Length);
                fs.Flush();
                fs.Close();
            }
            var client = new TcpClient();
            try
            {
                client.Connect(ipAddress, port);
                using (var ns = client.GetStream())
                {
                    ns.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("连接时发生错误" + ex.Message, ex);
            }
        }

    }

}