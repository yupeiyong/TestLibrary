using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TcpClient
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            Console.Title = "tcp client";
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(IPAddress.Parse(GetLocalIP()), 5600); //NO.1 
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] 连接服务器成功...");
            new Thread(new ParameterizedThreadStart(ReceiveThread)).Start(client); //NO. 
            string input = "";
            string[] heads = { "1", "2", "3", "4" };
            Console.WriteLine("输入字符串发送:");
            while ((input = Console.ReadLine()) != "quit") //NO.3 
            {
                string[] inputs = input.Split(' ');
                if (inputs.Length >= 2)
                {
                    if (heads.Contains(inputs[0])) //NO.4 
                    {
                        Send(client, int.Parse(inputs[0]), input.Replace(inputs[0] + " ", "")); //NO.5 
                    }
                    else
                    {
                        Console.WriteLine("输入错误!");
                    }
                }
                else
                {
                    Console.WriteLine("输入错误!");
                }
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="server"></param>
        /// <param name="msgType"></param>
        /// <param name="message"></param>
        private static void Send(Socket server, int msgType, string message)
        {
            var buffers = Encoding.Unicode.GetBytes(message);
            var sendBuffers = new byte[5 + buffers.Count()];
            using (var writer = new BinaryWriter(new MemoryStream(sendBuffers)))
            {
                writer.Write((byte)msgType);
                writer.Write(buffers.Length);
                writer.Write(buffers);
            }
            server.Send(sendBuffers);
        }


        private static void ReceiveThread(object socket)
        {
            var clientSocket = socket as Socket;
            if (clientSocket == null) return;

            var endPoint = clientSocket.RemoteEndPoint as IPEndPoint;
            if (endPoint == null) return;

            var remote_ip = endPoint.Address.ToString(); //NO.2 
            var remote_port = endPoint.Port; //NO.3

            var alreadyReceiveBuffers = new List<byte>();
            var receiveBuffers = new byte[256];
            var readLength = 0;
            do
            {
                readLength = clientSocket.Receive(receiveBuffers);
                var tmp = new byte[readLength];
                Buffer.BlockCopy(receiveBuffers, 0, tmp, 0, readLength);
                alreadyReceiveBuffers.AddRange(tmp);
                int msg_type = 0;
                string msg = null;
                while (ResolveInfos(ref msg_type, ref msg, alreadyReceiveBuffers)) //NO.5 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] 来自服务器(" + remote_ip + ":" + remote_port + ")的处理结果"); //NO.6
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" 处理类型:" + GetMsgType(msg_type));
                    Console.WriteLine(" 处理结果:" + msg);
                    Console.WriteLine();
                    Console.WriteLine("输入字符串发送:");
                }

            } while (readLength != 0);
        }
        /// <summary>
        ///     解析内容
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="message"></param>
        /// <param name="buffers"></param>
        private static bool ResolveInfos(ref int msgType, ref string message, List<byte> buffers)
        {
            if (buffers != null && buffers.Count > 5)
            {
                using (var reader = new BinaryReader(new MemoryStream(buffers.ToArray())))
                {
                    var header = reader.ReadByte();
                    var length = reader.ReadInt32();
                    var content = reader.ReadBytes(length);
                    if (buffers.Count - length >= 5)
                    {
                        msgType = header;
                        message = Encoding.Unicode.GetString(content);
                        buffers.RemoveRange(0, 5 + length);
                        return true;
                    }
                    msgType = 0;
                    message = null;
                    return false;
                }
            }
            msgType = 0;
            message = null;
            return false;
        }
        private static string GetMsgType(int msgType)
        {
            var msg = "";
            switch (msgType)
            {
                case 1: //NO.1 
                    {
                        msg = "转大写";
                        break;
                    }
                case 2: //NO.2 
                    {
                        msg = "转小写";
                        break;
                    }
                case 3: //NO.3 
                    {
                        msg = "替换百分号";
                        break;
                    }
                case 4: //NO.4 
                    {
                        msg = "替换百分号为空格";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return msg;
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
                Console.WriteLine("获取本机IP出错:" + ex.Message);
                return "";
            }
        }
    }

}
