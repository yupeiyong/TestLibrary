using System;
using System.IO;
using System.Linq;
using System.Text;


namespace TestProject.Stream
{
    /// <summary>
    ///  TextWriter和StreamWriter的举例
    /// </summary>
    public class StreamWriterTest
    {
        /// <summary>
        /// 编码
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// IFomatProvider
        /// </summary>
        private IFormatProvider _provider;

        /// <summary>
        /// 文件路径
        /// </summary>
        private string _textFilePath;


        public StreamWriterTest(Encoding encoding, string textFilePath)
            : this(encoding, textFilePath, null)
        {

        }

        public StreamWriterTest(Encoding encoding, string textFilePath, IFormatProvider provider)
        {
            this._encoding = encoding;
            this._textFilePath = textFilePath;
            this._provider = provider;
        }

        /// <summary>
        ///  我们可以通过FileStream 或者 文件路径直接对该文件进行写操作
        /// </summary>
        public void WriteSomthingToFile()
        {
            //获取FileStream
            using (FileStream stream = File.OpenWrite(_textFilePath))
            {
                //获取StreamWriter
                using (StreamWriter writer = new StreamWriter(stream, this._encoding))
                {
                    this.WriteSomthingToFile(writer);
                }

                //也可以通过文件路径和设置bool append，编码和缓冲区来构建一个StreamWriter对象
                using (StreamWriter writer = new StreamWriter(_textFilePath, true, this._encoding, 20))
                {
                    this.WriteSomthingToFile(writer);
                }
            }
        }

        /// <summary>
        ///  具体写入文件的逻辑
        /// </summary>
        /// <param name="writer">StreamWriter对象</param>
        public void WriteSomthingToFile(StreamWriter writer)
        {
            //需要写入的数据
            string[] writeMethodOverloadType =
           {
              "1.Write(bool);",
              "2.Write(char);",
              "3.Write(Char[])",
              "4.Write(Decimal)",
              "5.Write(Double)",
              "6.Write(Int32)",
              "7.Write(Int64)",
              "8.Write(Object)",
              "9.Write(Char[])",
              "10.Write(Single)",
              "11.Write(Char[])",
              "12.Write(String)",
              "13Write(UInt32)",
              "14.Write(string format,obj)",
              "15.Write(Char[])"
           };

            //定义writer的AutoFlush属性，如果定义了该属性，就不必使用writer.Flush方法
            writer.AutoFlush = true;
            writer.WriteLine("这个StreamWriter使用了{0}编码", writer.Encoding.HeaderName);
            //这里重新定位流的位置会导致一系列的问题
            //writer.BaseStream.Seek(1, SeekOrigin.Current);
            writer.WriteLine("这里简单演示下StreamWriter.Writer方法的各种重载版本");

            writeMethodOverloadType.ToList().ForEach(writer.WriteLine);
            writer.WriteLine("StreamWriter.WriteLine()方法就是在加上行结束符，其余和上述方法是用一致");
            //writer.Flush();
            writer.Close();
        }

        public void WriteSomthingToFileByUsingTextWriter()
        {
            using (TextWriter writer = new StringWriter(_provider))
            {
                writer.WriteLine("这里简单介绍下TextWriter 怎么使用用户设置的IFomatProvider，假设用户设置了NumberFormatInfoz.PercentDecimalSeparator属性");
                writer.WriteLine("看下区别吧 {0:p10}", 0.12);
                Console.WriteLine(writer.ToString());
                writer.Flush();
                writer.Close();
            }

        }
    }

}
