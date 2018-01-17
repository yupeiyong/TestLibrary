using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace TcpServer
{

    internal class Program
    {

        public static void Main(string[] args)
        {
            //声明一个socket
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            const int port = 5600;
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(20);
            Console.WriteLine("服务端开始侦听...(端口:" + port + ")");
            do
            {
                var proxySocket = server.Accept();

                //线程接收消息
                new Thread(ReceiveThread).Start(proxySocket);
                var remotePoint = proxySocket.RemoteEndPoint as IPEndPoint;
                if (remotePoint != null)
                {
                    var remoteIp = remotePoint.Address.ToString();
                    var remotePort = remotePoint.Port;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] 远程主机(" + remoteIp + ":" + remotePort + ")连入"); //NO.6
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            } while (true);
            Console.ReadKey();
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
                writer.Write((byte) msgType);
                writer.Write(buffers.Length);
                writer.Write(buffers);
            }
            server.Send(sendBuffers);
        }


        /// <summary>
        ///     接收线程
        /// </summary>
        /// <param name="proxySocket"></param>
        private static void ReceiveThread(object proxySocket)
        {
            var proxy = proxySocket as Socket;
            if (proxy == null)
                return;

            var ipEndPoint = proxy.RemoteEndPoint as IPEndPoint;
            var remoteIp = ipEndPoint.Address.ToString();
            var remotePort = ipEndPoint.Port;
            if (ipEndPoint != null)
            {
                var alreadyReceiveBuffers = new List<byte>();
                var receiveBuffers = new byte[256];
                var readLength = 0;
                do
                {
                    try
                    {
                        readLength = proxy.Receive(receiveBuffers);
                        var tmpBuffers = new byte[readLength];
                        Buffer.BlockCopy(receiveBuffers, 0, tmpBuffers, 0, readLength);
                        alreadyReceiveBuffers.AddRange(tmpBuffers);
                        var msgType = 0;
                        var message = string.Empty;
                        while (ResolveInfos(ref msgType, ref message, alreadyReceiveBuffers))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] 来自(" + remoteIp + ":" + remotePort + ")的请求");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(" 请求类型:" + GetMsgType(msgType));
                            Console.WriteLine(" 数据内容:" + message);
                            Console.WriteLine();
                            var msg = ConvertMessage(msgType, message);

                            //发送转换后的字符串
                            Send(proxy, msgType, msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        proxy.Close();
                    }
                } while (readLength != 0);
            }
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


        /// <summary>
        ///     转换字符串内容
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ConvertMessage(int msgType, string message)
        {
            var msg = string.Empty;
            switch (msgType)
            {
                case 1:
                    msg = message.ToUpper();
                    break;
                case 2:
                    msg = message.ToLower();
                    break;
                case 3:
                    msg = message.Replace("%", "");
                    break;
                case 4:
                    msg = message.Replace("%", " ");
                    break;
            }
            return msg;
        }

    }

}