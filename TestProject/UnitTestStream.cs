using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class UnitTestStream
    {

        [TestMethod]
        public void TestMethodStream()
        {
            byte[] buffer = null;

            string testString = "Stream!Hello world";
            char[] readCharArray = null;
            byte[] readBuffer = null;
            string readString = string.Empty;

            //关于MemoryStream 我会在后续章节详细阐述
            using (MemoryStream stream = new MemoryStream())
            {
                Console.WriteLine("初始字符串为：{0}", testString);

                //如果该流可写
                if (stream.CanWrite)
                {
                    //首先我们尝试将testString写入流中
                    //关于Encoding我会在另一篇文章中详细说明，暂且通过它实现string->byte[]的转换
                    buffer = Encoding.Default.GetBytes(testString);

                    //我们从该数组的第一个位置开始写，长度为3，写完之后 stream中便有了数据
                    //对于新手来说很难理解的就是数据是什么时候写入到流中，在冗长的项目代码面前，我碰见过很
                    //多新手都会有这种经历，我希望能够用如此简单的代码让新手或者老手们在温故下基础
                    stream.Write(buffer, 0, 3);

                    Console.WriteLine("现在Stream.Postion在第{0}位置", stream.Position + 1);

                    //从刚才结束的位置（当前位置）往后移3位，到第7位
                    long newPositionInStream = stream.CanSeek ? stream.Seek(3, SeekOrigin.Current) : 0;

                    Console.WriteLine("重新定位后Stream.Postion在第{0}位置", newPositionInStream + 1);
                    if (newPositionInStream < buffer.Length)
                    {
                        //将从新位置（第7位）一直写到buffer的末尾，注意下stream已经写入了3个数据“Str”
                        stream.Write(buffer, (int) newPositionInStream, buffer.Length - (int) newPositionInStream);
                    }

                    //写完后将stream的Position属性设置成0，开始读流中的数据
                    stream.Position = 0;

                    // 设置一个空的盒子来接收流中的数据，长度根据stream的长度来决定
                    readBuffer = new byte[stream.Length];

                    //设置stream总的读取数量 ，
                    //注意！这时候流已经把数据读到了readBuffer中
                    int count = stream.CanRead ? stream.Read(readBuffer, 0, readBuffer.Length) : 0;

                    //由于刚开始时我们使用加密Encoding的方式,所以我们必须解密将readBuffer转化成Char数组，这样才能重新拼接成string

                    //首先通过流读出的readBuffer的数据求出从相应Char的数量
                    int charCount = Encoding.Default.GetCharCount(readBuffer, 0, count);

                    //通过该Char的数量 设定一个新的readCharArray数组
                    readCharArray = new char[charCount];

                    //Encoding 类的强悍之处就是不仅包含加密的方法，甚至将解密者都能创建出来（GetDecoder()），
                    //解密者便会将readCharArray填充（通过GetChars方法，把readBuffer 逐个转化将byte转化成char，并且按一致顺序填充到readCharArray中）
                    Encoding.Default.GetDecoder().GetChars(readBuffer, 0, count, readCharArray, 0);
                    for (int i = 0; i < readCharArray.Length; i++)
                    {
                        readString += readCharArray[i];
                    }
                    Console.WriteLine("读取的字符串为：{0}", readString);
                }

                stream.Close();
            }
        }

    }
}
