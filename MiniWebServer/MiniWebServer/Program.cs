using System;
using System.Collections.Generic;
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
        static void Main1(string[] args)
        {
            IPAddress localIP = IPAddress.Loopback;
            IPEndPoint endPoint = new IPEndPoint(localIP, 8010);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint); //NO.1
            server.Listen(10);
            Console.WriteLine("开始监听，端口号：{0}", endPoint.Port);
            server.BeginAccept(new AsyncCallback(OnAccept), server); //NO.2 asynchronous accept
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
            server.BeginAccept(new AsyncCallback(OnAccept), server); //start the next accept NO.12
        }


        static void Main(string[] args)
        {
            var ip = IPAddress.Loopback;
            var endPoint = new IPEndPoint(ip, 8081);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);

            server.Listen(10);
            Console.WriteLine("开始监听，端口号：{0}", endPoint.Port);
            server.BeginAccept(new AsyncCallback(OnAccept), server);
            Console.Read();
        }

        static void OnAccept(IAsyncResult ar)
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
            server.BeginAccept(new AsyncCallback(OnAccept), server);
        }
    }
}
