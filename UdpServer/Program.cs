using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = GetLocalIP();
            var server = new Server(ip, 7890, new Dictionary<int, string>
            { {7891,ip }, {7892,ip }, {7893,ip }
            });
            server.Start();
            Console.WriteLine("UDP服务器已经启动！");
            Console.ReadKey();
        }

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception("获取本机IP出错:" + ex.Message);
            }
        }
    }

    class Server
    {
        private Dictionary<int, string> _clients = new Dictionary<int, string>();

        private UdpClient _server;
        public Server(string ip, int port, Dictionary<int, string> clients)
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _server = new UdpClient(ipPoint);
            _clients = clients;
        }

        public void Start()
        {
            var t = new Thread(SendMessage);
            t.Start();
        }

        private void SendMessage()
        {
            while (true)
            {
                if (_clients.Count > 0)
                {
                    foreach (var port in _clients.Keys)
                    {
                        Console.WriteLine("广播消息已经发送！");
                        var msg = "服务器时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        var buffers = Encoding.UTF8.GetBytes(msg);
                        //广播发送消息
                        _server.Send(buffers, buffers.Length, new IPEndPoint(IPAddress.Parse(_clients[port]), port));
                    }
                    Thread.Sleep(5000);
                }

            }

        }
    }
}
