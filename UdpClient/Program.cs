using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpClienter
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = GetLocalIP();

            var port = new Random().Next(7891, 7893);
            var client = new Client(ip, port, ip,7890);
            client.Start();
            Console.WriteLine("UDP接收远程消息已经启动！");
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

    class Client
    {
        private UdpClient _client;

        private IPEndPoint _remoteIpPoint;
        public Client(string ip, int port, string remoteIp,int remotePort)
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _client = new UdpClient(ipPoint);

            _remoteIpPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
        }

        public void Start()
        {
            var t = new Thread(ReceiveMessage);
            t.Start();
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                var buffers=_client.Receive(ref _remoteIpPoint);
                var msg=Encoding.UTF8.GetString(buffers);
                Console.WriteLine(msg);
                Thread.Sleep(5000);
            }

        }
    }
}
