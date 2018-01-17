using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TcpServer
{

    internal class Program1
    {

        private static void Main1(string[] args)
        {

            Console.Title = "tcp server";
            const int localPort = 5600;
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, localPort)); //NO.1 
            server.Listen(10); //No.2 
            Console.WriteLine("Server Start Listening...(Port:" + localPort + ")");
            while (true) //accept loop NO.3 
            {
                var proxySocket = server.Accept(); //NO.4 
                new Thread(Receive_th).Start(proxySocket); //NO. 
                var ipEndPoint = proxySocket.RemoteEndPoint as IPEndPoint;
                if (ipEndPoint != null)
                {
                    var remoteIp = ipEndPoint.Address.ToString();
                    var remotePort = ipEndPoint.Port;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] 远程主机(" + remoteIp + ":" + remotePort + ")连入"); //NO.6
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }

        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msgType"></param>
        /// <param name="message"></param>
        private static void Send1(Socket socket, int msgType, string message)
        {
            var msgBuffer = Encoding.Unicode.GetBytes(message);
            var sendBuffer = new byte[1 + 4 + msgBuffer.Count()];
            using (var bw = new BinaryWriter(new MemoryStream(sendBuffer)))
            {
                bw.Write((byte)msgType);
                bw.Write(msgBuffer.Length);
                bw.Write(msgBuffer);
                socket.Send(sendBuffer);
            }
        }



        //private static bool ResolveInfos11(ref int msg_type, ref string msg, List<byte> bytes_container)
        //{
        //    if (bytes_container.Count > 5) //NO.1 
        //    {
        //        using (BinaryReader br = new BinaryReader(new MemoryStream(bytes_container.ToArray())))
        //        {
        //            byte head = br.ReadByte();
        //            int length = br.ReadInt32();
        //            if (bytes_container.Count - 5 >= length) //NO.2 
        //            {
        //                msg = Encoding.Unicode.GetString(br.ReadBytes(length)); //NO.3 
        //                msg_type = (int)head; //NO.4 
        //                bytes_container.RemoveRange(0, 5 + length); //NO.5 
        //                return true;
        //            }
        //            else
        //            {
        //                msg_type = 0;
        //                msg = null;
        //                return false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        msg_type = 0;
        //        msg = null;
        //        return false;
        //    }
        //}


        private static bool ResolveInfos(ref int msgType, ref string msg, List<byte> bytesContainer)
        {
            if (bytesContainer != null && bytesContainer.Count > 5)
            {
                using (var reader = new BinaryReader(new MemoryStream(bytesContainer.ToArray())))
                {
                    var head = reader.ReadByte();
                    var length = reader.ReadInt32();
                    if (bytesContainer.Count - 5 >= length)
                    {
                        msgType = (int)head;
                        msg = Encoding.Unicode.GetString(reader.ReadBytes(length));
                        bytesContainer.RemoveRange(0, 5 + length);
                        return true;
                    }
                    else
                    {
                        msgType = 0;
                        msg = null;
                        return false;
                    }
                }
            }
            else
            {
                msgType = 0;
                msg = null;
                return false;
            }
        }


        private static void DealAsk(Socket proxy_socket, int msg_type, string msg)
        {
            string msg_dealed = msg;
            switch (msg_type)
            {
                case 1: //NO.1 
                    {
                        msg_dealed = msg_dealed.ToUpper();
                        break;
                    }
                case 2: //NO.2 
                    {
                        msg_dealed = msg_dealed.ToLower();
                        break;
                    }
                case 3: //NO.3 
                    {
                        msg_dealed = msg_dealed.Replace("%", "");
                        break;
                    }
                case 4: //NO.4 
                    {
                        msg_dealed = msg_dealed.Replace("%", " ");
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            Send1(proxy_socket, msg_type, msg_dealed); //NO.
        }

        /// <summary>
        /// 接收线程
        /// </summary>
        /// <param name="obj"></param>
        private static void Receive_th(object obj) //NO.1
        {
            var proxySocket = obj as Socket;
            if (proxySocket == null) return;
            var ipEndPoint = proxySocket.RemoteEndPoint as IPEndPoint;
            if (ipEndPoint != null)
            {
                var remoteIp = ipEndPoint.Address.ToString(); //NO.2 
                var remotePort = ipEndPoint.Port; //NO.
                var bytesAlreadyRecv = new List<byte>();
                var bytesToRecv = new byte[256];
                int length_to_recv = 0;
                do //NO.4 
                {
                    try
                    {
                        //每次接收256个字节
                        length_to_recv = proxySocket.Receive(bytesToRecv);
                        byte[] tmp = new byte[length_to_recv];
                        Buffer.BlockCopy(bytesToRecv, 0, tmp, 0, length_to_recv);
                        bytesAlreadyRecv.AddRange(tmp);
                        int msg_type = 0;
                        string msg = null;
                        while (ResolveInfos(ref msg_type, ref msg, bytesAlreadyRecv)) //NO.5 
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] 来自(" + remoteIp + ":" + remotePort + ")的请求");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(" 请求类型:" + GetMsgType(msg_type));
                            Console.WriteLine(" 数据内容:" + msg);
                            Console.WriteLine();
                            DealAsk(proxySocket, msg_type, msg); //deal data NO.6
                        }
                    }
                    catch
                    {
                        proxySocket.Close();
                    }
                } while (length_to_recv != 0);
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


    }

}
