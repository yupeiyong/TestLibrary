using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace TestProject.Stream
{

    public class Server
    {

        //端口
        private const int webPort = 80;

        //默认接收缓存大小
        private byte[] receiveBufferBytes = new byte[4096];

        //需要获取网页的url
        private string webPageURL;


        public Server(string webPageUrl)
        {
            webPageURL = webPageUrl;
        }


        /// <summary>
        ///     从该网页上获取数据
        /// </summary>
        public void FetchWebPageData()
        {
            if (!string.IsNullOrEmpty(webPageURL))
                FetchWebPageData(webPageURL);
        }


        /// <summary>
        ///     从该网页上获取数据
        /// </summary>
        /// <param name="webPageURL">网页url</param>
        private void FetchWebPageData(string webPageURL)
        {
            //通过url获取主机信息
            var iphe = Dns.GetHostEntry(GetHostNameBystrUrl(webPageURL));
            Console.WriteLine("远程服务器名： {0}", iphe.HostName);

            //通过主机信息获取其IP
            var address = iphe.AddressList;
            var ipep = new IPEndPoint(address[0], 80);

            //实例化一个socket用于接收网页数据
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //连接
            socket.Connect(ipep);
            if (socket.Connected)
            {
                //发送头文件，这样才能下载网页数据
                socket.Send(Encoding.ASCII.GetBytes(GetHeader(webPageURL)));
            }
            else
            {
                return;
            }

            //接收头一批数据
            var count = socket.Receive(receiveBufferBytes);

            //转化成string 
            var getString = Encoding.Default.GetString(receiveBufferBytes);

            //创建文件流
            var fs = new FileStream(@"d:\\Test.html", FileMode.OpenOrCreate);

            //创建缓存流
            var bs = new BufferedStream(fs);
            using (fs)
            {
                using (bs)
                {
                    var finalContent = Encoding.Default.GetBytes(getString.ToCharArray());

                    //将头一批数据写入本地硬盘
                    bs.Write(finalContent, 0, finalContent.Length);

                    //循环通过socket接收数据
                    while (count > 0)
                    {
                        count = socket.Receive(receiveBufferBytes, receiveBufferBytes.Length, SocketFlags.None);

                        //直接将获取到的byte数据写入本地硬盘
                        bs.Write(receiveBufferBytes, 0, receiveBufferBytes.Length);
                        Console.WriteLine(Encoding.Default.GetString(receiveBufferBytes));
                    }
                    bs.Flush();
                    fs.Flush();
                    bs.Close();
                    fs.Close();
                }
            }
        }


        /// <summary>
        ///     得到header
        /// </summary>
        /// <param name="url">网页url</param>
        /// <returns>header字符串</returns>
        private string GetHeader(string webPageurl)
        {
            return "GET " + GetRelativeUrlBystrUrl(webPageurl) + " HTTP/1.1\r\nHost: "
                   + GetHostNameBystrUrl(webPageurl) + "\r\nConnection: Close\r\n\r\n";
        }


        /// <summary>
        ///     得到相对路径
        /// </summary>
        /// <param name="strUrl">网页url</param>
        /// <returns></returns>
        private string GetRelativeUrlBystrUrl(string strUrl)
        {
            var iIndex = strUrl.IndexOf(@"//");
            if (iIndex <= 0)
                return "/";
            var strTemp = strUrl.Substring(iIndex + 2);
            iIndex = strTemp.IndexOf(@"/");
            if (iIndex > 0)
                return strTemp.Substring(iIndex);
            return "/";
        }


        /// <summary>
        ///     根据Url得到host
        /// </summary>
        /// <param name="strUrl">网页url</param>
        /// <returns></returns>
        private string GetHostNameBystrUrl(string strUrl)
        {
            var iIndex = strUrl.IndexOf(@"//");
            if (iIndex <= 0)
                return "";
            var strTemp = strUrl.Substring(iIndex + 2);
            iIndex = strTemp.IndexOf(@"/");
            if (iIndex > 0)
                return strTemp.Substring(0, iIndex);
            return strTemp;
        }

    }

}