using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniWebServer
{
    class Program
    {

        #region  自定义服务器1
        static void Main1(string[] args)
        {
            IPAddress localIP = IPAddress.Loopback;
            IPEndPoint endPoint = new IPEndPoint(localIP, 8010);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint); //NO.1
            server.Listen(10);
            Console.WriteLine("开始监听，端口号：{0}", endPoint.Port);
            server.BeginAccept(new AsyncCallback(OnAccept1), server); //NO.2 asynchronous accept
            Console.Read();
        }

        static void OnAccept1(IAsyncResult ar)
        {
            Socket server = ar.AsyncState as Socket;
            Socket proxy_socket = server.EndAccept(ar); //NO.1 get proxy socket
            Console.WriteLine(proxy_socket.RemoteEndPoint);
            byte[] bytes_to_recv = new byte[4096];
            int length_to_recv = proxy_socket.Receive(bytes_to_recv); //NO.2 receive request data
            string received_string = Encoding.UTF8.GetString(bytes_to_recv, 0, length_to_recv);
            Console.WriteLine(received_string); //NO.3
            string statusLine = "";
            string responseContent = "";
            string responseHeader = "";
            byte[] statusLine_to_bytes;
            byte[] responseContent_to_bytes;
            byte[] responseHeader_to_bytes;
            string[] items = received_string.Split(new string[] { "\r\n" }, StringSplitOptions.None); //items[0]like "GET / HTTP/1.1" NO.4 resolve the request string
            if (items[0].Contains("Sleep")) //NO.5
            {
                Thread.Sleep(1000 * 10);
                statusLine = "HTTP/1.1 200 OK\r\n";
                statusLine_to_bytes = Encoding.UTF8.GetBytes(statusLine);
                responseContent = "<html><head><title>Sleeping WebPage</title></head><body><h2>Sleeping 10 seconds,Hello Microsoft .NET<h2></body></html>";
                responseContent_to_bytes = Encoding.UTF8.GetBytes(responseContent);
                responseHeader =
               string.Format("Content-Type:text/html;charset=UTF-8\r\nContent-Length:{0}\r\n",
               responseContent_to_bytes.Length);
                responseHeader_to_bytes = Encoding.UTF8.GetBytes(responseHeader);
            }
            else //NO.6
            {
                statusLine = "HTTP/1.1 200 OK\r\n";
                statusLine_to_bytes = Encoding.UTF8.GetBytes(statusLine);
                responseContent = "<html><head><title>Normal Web Page</title></head><body><h2>Hello Microsoft .NET<h2></body></html>";
                responseContent_to_bytes = Encoding.UTF8.GetBytes(responseContent);
                responseHeader =
               string.Format("Content-Type:text/html;charset=UTF-8\r\nContent-Length:{0}\r\n",
               responseContent_to_bytes.Length);
                responseHeader_to_bytes = Encoding.UTF8.GetBytes(responseHeader);
            }
            proxy_socket.Send(statusLine_to_bytes); //NO.7
            proxy_socket.Send(responseHeader_to_bytes); //NO.8
            proxy_socket.Send(new byte[] { (byte)'\r', (byte)'\n' }); //NO.9
            proxy_socket.Send(responseContent_to_bytes); //NO.10
            proxy_socket.Close(); //NO.11
            server.BeginAccept(new AsyncCallback(OnAccept1), server); //start the next accept NO.12
        }

        #endregion

        #region 自定义服务器2
        static void Main2(string[] args)
        {
            var ip = IPAddress.Loopback;
            var endPoint = new IPEndPoint(ip, 8081);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);

            server.Listen(10);
            Console.WriteLine("开始监听，端口号：{0}", endPoint.Port);
            server.BeginAccept(new AsyncCallback(OnAccept2), server);
            Console.Read();
        }

        static void OnAccept2(IAsyncResult ar)
        {
            var server = ar.AsyncState as Socket;
            var proxySocket = server.EndAccept(ar);
            Console.WriteLine(proxySocket.RemoteEndPoint);
            var receiveBuffers = new byte[4096];
            var receiveLength = proxySocket.Receive(receiveBuffers);
            var receiveString = Encoding.UTF8.GetString(receiveBuffers, 0, receiveLength);
            Console.WriteLine(receiveString);
            var receiveItems = receiveString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            var contents = string.Empty;
            if (receiveItems[0].ToLower().Contains("sleep"))
            {
                contents = "<html><head><title>Sleeping WebPage</title></head><body><h2>Sleeping 10 seconds,Hello Microsoft .NET<h2></body></html>";
                Thread.Sleep(1000 * 10);
            }
            else
            {
                contents = "<html><head><title>Normal Web Page</title></head><body><h2>Hello Microsoft .NET<h2></body></html>";
            }
            var contentBuffers = Encoding.UTF8.GetBytes(contents);

            var statusContent = "HTTP/1.1 200 OK\r\n";
            var statusBuffers = Encoding.UTF8.GetBytes(statusContent);

            var headContent = string.Format("Content-Type:text/html;charset=UTF-8\r\nContent-Length:{0}\r\n", contentBuffers.Length);
            var headBuffers = Encoding.UTF8.GetBytes(headContent);
            proxySocket.Send(statusBuffers);
            proxySocket.Send(headBuffers);
            proxySocket.Send(new byte[] { (byte)'\r', (byte)'\n' }); //NO.9

            proxySocket.Send(contentBuffers);
            proxySocket.Close();
            server.BeginAccept(new AsyncCallback(OnAccept2), server);
        }


        #endregion

        #region 自定义服务器3
        static void Main(string[] args)
        {
            MyWebServer MWS = new MyWebServer();
            Console.Read();
        }

        class MyWebServer
        {
            private TcpListener myListener;
            private int port = 8899; // 选者任何闲置端口
                                     //开始兼听端口
                                     //同时启动一个兼听进程
            public MyWebServer()
            {
                try
                {
                    //开始兼听端口
                    myListener = new TcpListener(port);
                    myListener.Start();
                    Console.WriteLine("Web Server Running... Press ^C to Stop...");
                    //同时启动一个兼听进程 ''StartListen''
                    Thread th = new Thread(new ThreadStart(StartListen));
                    th.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("兼听端口时发生错误 :" + e.ToString());
                }
            }
            public void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
            {

                String sBuffer = "";
                if (sMIMEHeader.Length == 0)
                {
                    sMIMEHeader = "text/html"; // 默认 text/html
                }
                sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
                sBuffer = sBuffer + "Server: cx1193719-b\r\n";
                sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
                sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
                sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";
                Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
                SendToBrowser(bSendData, ref mySocket);
                Console.WriteLine("Total Bytes : " + iTotBytes.ToString());
            }
            public void SendToBrowser(String sData, ref Socket mySocket)
            {
                SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
            }

            public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
            {
                int numBytes = 0;

                try
                {
                    if (mySocket.Connected)
                    {
                        if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                            Console.WriteLine("Socket Error cannot Send Packet");
                        else
                        {
                            Console.WriteLine("No. of bytes send {0}", numBytes);
                        }
                    }
                    else
                        Console.WriteLine("连接失败....");
                }
                catch (Exception e)
                {
                    Console.WriteLine("发生错误 : {0} ", e);

                }
            }
            public void StartListen()
            {

                int iStartPos = 0;
                String sRequest;
                String sDirName;
                String sRequestedFile;
                String sErrorMessage;
                String sLocalDir;
                /////////////////////////////////////注意设定你自己的虚拟目录/////////////////////////////////////
                String sMyWebServerRoot = @"C:\\Cassini\"; //设置你的虚拟目录
                                                           //////////////////////////
                String sFormattedMessage = "";
                String sResponse = "";


                while (true)
                {
                    //接受新连接
                    Socket mySocket = myListener.AcceptSocket();

                    Console.WriteLine("Socket Type " + mySocket.SocketType);
                    if (mySocket.Connected)
                    {
                        Console.WriteLine("\nClient Connected!!\n==================\nCLient IP {0}\n", mySocket.RemoteEndPoint);

                        Byte[] bReceive = new Byte[1024];
                        int i = mySocket.Receive(bReceive, bReceive.Length, 0);


                        //转换成字符串类型
                        string sBuffer = Encoding.ASCII.GetString(bReceive);


                        //只处理"get"请求类型
                        if (sBuffer.Substring(0, 3) != "GET")
                        {
                            Console.WriteLine("只处理get请求类型..");
                            mySocket.Close();
                            return;
                        }

                        // 查找 "HTTP" 的位置
                        iStartPos = sBuffer.IndexOf("HTTP", 1);


                        string sHttpVersion = sBuffer.Substring(iStartPos, 8);


                        // 得到请求类型和文件目录文件名
                        sRequest = sBuffer.Substring(0, iStartPos - 1);

                        sRequest= sRequest.Replace("\\","/");


                        //如果结尾不是文件名也不是以"/"结尾则加"/"
                        if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                        {
                            sRequest = sRequest + "/";
                        }


                        //得带请求文件名
                        iStartPos = sRequest.LastIndexOf("/") + 1;
                        sRequestedFile = sRequest.Substring(iStartPos);


                        //得到请求文件目录
                        sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/") - 3);


                        //获取虚拟目录物理路径
                        sLocalDir = sMyWebServerRoot;

                        Console.WriteLine("请求文件目录 : " + sLocalDir);

                        if (sLocalDir.Length == 0)
                        {
                            sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);
                            mySocket.Close();
                            continue;
                        }


                        if (sRequestedFile.Length == 0)
                        {
                            // 取得请求文件名
                            sRequestedFile = "index.html";
                        }


                        /////////////////////////////////////////////////////////////////////
                        // 取得请求文件类型（设定为text/html）
                        /////////////////////////////////////////////////////////////////////

                        String sMimeType = "text/html";

                        string sPhysicalFilePath = sLocalDir + sRequestedFile;
                        Console.WriteLine("请求文件: " + sPhysicalFilePath);


                        if (File.Exists(sPhysicalFilePath) == false)
                        {

                            sErrorMessage = "<script language='javascript'>alert('你好呀，我不是IIS服务器!');</script>";
                            //SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            //SendToBrowser(sErrorMessage, ref mySocket);
                            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(sErrorMessage);
                            SendHeader(sHttpVersion, sMimeType, bytes.Length, " 200 OK", ref mySocket);
                            SendToBrowser(bytes, ref mySocket);
                            //Console.WriteLine(sFormattedMessage);
                        }
                        else
                        {
                            int iTotBytes = 0;

                            sResponse = "";

                            FileStream fs = new FileStream(sPhysicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                            BinaryReader reader = new BinaryReader(fs);
                            byte[] bytes = new byte[fs.Length];
                            int read;
                            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);

                                iTotBytes = iTotBytes + read;

                            }
                            reader.Close();
                            fs.Close();

                            SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                            SendToBrowser(bytes, ref mySocket);
                            //mySocket.Send(bytes, bytes.Length,0);

                        }
                        mySocket.Close();
                    }
                }
            }
        }
        #endregion
    }
}
