using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.Stream;


namespace TestProject
{

    [TestClass]
    public class UnitTestStream
    {

        /// <summary>
        ///     测试基础流
        /// </summary>
        [TestMethod]
        public void TestMethodStream()
        {
            byte[] buffer = null;

            var testString = "Stream!Hello world";
            char[] readCharArray = null;
            byte[] readBuffer = null;
            var readString = string.Empty;

            //关于MemoryStream 我会在后续章节详细阐述
            using (var stream = new MemoryStream())
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
                    var newPositionInStream = stream.CanSeek ? stream.Seek(3, SeekOrigin.Current) : 0;

                    Console.WriteLine("重新定位后Stream.Postion在第{0}位置", newPositionInStream + 1);
                    if (newPositionInStream < buffer.Length)
                    {
                        //将从新位置（第7位）一直写到buffer的末尾，注意下stream已经写入了3个数据“Str”
                        stream.Write(buffer, (int)newPositionInStream, buffer.Length - (int)newPositionInStream);
                    }

                    //写完后将stream的Position属性设置成0，开始读流中的数据
                    stream.Position = 0;

                    // 设置一个空的盒子来接收流中的数据，长度根据stream的长度来决定
                    readBuffer = new byte[stream.Length];

                    //设置stream总的读取数量 ，
                    //注意！这时候流已经把数据读到了readBuffer中
                    var count = stream.CanRead ? stream.Read(readBuffer, 0, readBuffer.Length) : 0;

                    //由于刚开始时我们使用加密Encoding的方式,所以我们必须解密将readBuffer转化成Char数组，这样才能重新拼接成string

                    //首先通过流读出的readBuffer的数据求出从相应Char的数量
                    var charCount = Encoding.Default.GetCharCount(readBuffer, 0, count);

                    //通过该Char的数量 设定一个新的readCharArray数组
                    readCharArray = new char[charCount];

                    //Encoding 类的强悍之处就是不仅包含加密的方法，甚至将解密者都能创建出来（GetDecoder()），
                    //解密者便会将readCharArray填充（通过GetChars方法，把readBuffer 逐个转化将byte转化成char，并且按一致顺序填充到readCharArray中）
                    Encoding.Default.GetDecoder().GetChars(readBuffer, 0, count, readCharArray, 0);
                    for (var i = 0; i < readCharArray.Length; i++)
                    {
                        readString += readCharArray[i];
                    }
                    Console.WriteLine("读取的字符串为：{0}", readString);
                }

                stream.Close();
            }
        }


        /// <summary>
        ///     测试流读取
        /// </summary>
        [TestMethod]
        public void TestMethodTextReader()
        {
            var text = "abc\nabc";

            using (TextReader reader = new StringReader(text))
            {
                while (reader.Peek() != -1)
                {
                    Console.WriteLine("Peek = {0}", (char)reader.Peek());
                    Console.WriteLine("Read = {0}", (char)reader.Read());
                }
                reader.Close();
            }

            using (TextReader reader = new StringReader(text))
            {
                var charBuffer = new char[3];
                var data = reader.ReadBlock(charBuffer, 0, 3);
                for (var i = 0; i < charBuffer.Length; i++)
                {
                    Console.WriteLine("通过readBlock读出的数据：{0}", charBuffer[i]);
                }
                reader.Close();
            }

            using (TextReader reader = new StringReader(text))
            {
                var lineData = reader.ReadLine();
                Console.WriteLine("第一行的数据为:{0}", lineData);
                reader.Close();
            }

            using (TextReader reader = new StringReader(text))
            {
                var allData = reader.ReadToEnd();
                Console.WriteLine("全部的数据为:{0}", allData);
                reader.Close();
            }
        }


        [TestMethod]
        public void TestMethodStreamReader()
        {
            var filePath = @"TextReader.txt";

            //文件地址
            var txtFilePath = Path.GetFullPath(filePath);

            //定义char数组
            var charBuffer2 = new char[3];

            //利用FileStream类将文件文本数据变成流然后放入StreamReader构造函数中
            using (var stream = File.OpenRead(txtFilePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    //StreamReader.Read()方法
                    DisplayResultStringByUsingRead(reader);
                }
            }

            using (var stream = File.OpenRead(txtFilePath))
            {
                //使用Encoding.ASCII来尝试下
                using (var reader = new StreamReader(stream, Encoding.ASCII, false))
                {
                    //StreamReader.ReadBlock()方法
                    DisplayResultStringByUsingReadBlock(reader);
                }
            }

            //尝试用文件定位直接得到StreamReader，顺便使用 Encoding.Default
            using (var reader = new StreamReader(txtFilePath, Encoding.Default, false, 123))
            {
                //StreamReader.ReadLine()方法
                DisplayResultStringByUsingReadLine(reader);
            }

            //也可以通过File.OpenText方法直接获取到StreamReader对象
            using (var reader = File.OpenText(txtFilePath))
            {
                //StreamReader.ReadLine()方法
                DisplayResultStringByUsingReadLine(reader);
            }
        }


        /// <summary>
        ///     使用StreamReader.Read()方法
        /// </summary>
        /// <param name="reader"></param>
        public static void DisplayResultStringByUsingRead(StreamReader reader)
        {
            var readChar = 0;
            var result = string.Empty;
            while ((readChar = reader.Read()) != -1)
            {
                result += (char)readChar;
            }
            Console.WriteLine("使用StreamReader.Read()方法得到Text文件中的数据为 : {0}", result);
        }


        /// <summary>
        ///     使用StreamReader.ReadBlock()方法
        /// </summary>
        /// <param name="reader"></param>
        public static void DisplayResultStringByUsingReadBlock(StreamReader reader)
        {
            var charBuffer = new char[10];
            var result = string.Empty;
            reader.ReadBlock(charBuffer, 0, 10);
            for (var i = 0; i < charBuffer.Length; i++)
            {
                result += charBuffer[i];
            }
            Console.WriteLine("使用StreamReader.ReadBlock()方法得到Text文件中前10个数据为 : {0}", result);
        }


        /// <summary>
        ///     使用StreamReader.ReadLine()方法
        /// </summary>
        /// <param name="reader"></param>
        public static void DisplayResultStringByUsingReadLine(StreamReader reader)
        {
            var i = 1;
            string resultString;
            while ((resultString = reader.ReadLine()) != null)
            {
                Console.WriteLine("使用StreamReader.Read()方法得到Text文件中第{1}行的数据为 : {0}", resultString, i);
                i++;
            }
        }


        /// <summary>
        ///     测试格式化数字
        /// </summary>
        [TestMethod]
        public void TestIFomatProvider()
        {
            //有关数字格式化隐性使用IFomatProvider的例子

            //货币
            Console.WriteLine("显示货币格式{0:c3}", 12);

            //十进制
            Console.WriteLine("显示货币十进制格式{0:d10}", 12);

            //科学计数法
            Console.WriteLine("科学计数法{0:e5}", 12);

            //固定点格式
            Console.WriteLine("固定点格式 {0:f10}", 12);

            //常规格式
            Console.WriteLine("常规格式{0:g10}", 12);

            //数字格式(用分号隔开)
            Console.WriteLine("数字格式 {0:n5}:", 666666666);

            //百分号格式
            Console.WriteLine("百分号格式(不保留小数){0:p0}", 0.55);

            //16进制
            Console.WriteLine("16进制{0:x0}", 12);

            // 0定位器  此示例保留5位小数，如果小数部分小于5位，用0填充
            Console.WriteLine("0定位器{0:000.00000}", 1222.133);

            //数字定位器
            Console.WriteLine("数字定位器{0:(#).###}", 0200.0233000);

            //小数
            Console.WriteLine("小数保留一位{0:0.0}", 12.222);

            //百分号的另一种写法，注意小数的四舍五入
            Console.WriteLine("百分号的另一种写法，注意小数的四舍五入{0:0%.00}", 0.12345);
        }


        [TestMethod]

        public void TestStreamWrite()
        {
            var fileName = "TextWriter.txt";
            var txtFilePath = Path.GetFullPath(fileName);
            NumberFormatInfo numberFomatProvider = new NumberFormatInfo();
            //将小数 “.”换成"?"
            numberFomatProvider.PercentDecimalSeparator = "?";
            var test = new StreamWriterTest(Encoding.Default, txtFilePath, numberFomatProvider);
            //StreamWriter
            test.WriteSomthingToFile();
            //TextWriter
            test.WriteSomthingToFileByUsingTextWriter();
        }


        [TestMethod]

        public void TestFileStream()
        {
            FileStreamTest test = new FileStreamTest();
            //创建文件配置类
            CreateFileConfig createFileConfig = new CreateFileConfig { CreateUrl = @"d:\MyFile.txt", IsAsync = true };
            //复制文件配置类
            CopyFileConfig copyFileConfig = new CopyFileConfig
            {
                OrginalFileUrl = @"d:\8.jpg",
                DestinationFileUrl = @"d:\9.jpg",
                IsAsync = true
            };
            test.Create(createFileConfig);
            test.Copy(copyFileConfig);
        }


        [TestMethod]

        public void TestBufferStream()
        {

        }
    }
}