using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Stream
{
    /// <summary>
    /// FileStreamTest 类
    /// </summary>
    public class FileStreamTest
    {
        private static object _lockObject = new object();
        /// <summary>
        /// 添加文件方法
        /// </summary>
        /// <param name="config"> 创建文件配置类</param>
        public void Create(IFileConfig config)
        {
            lock (_lockObject)
            {
                //得到创建文件配置类对象
                var createFileConfig = config as CreateFileConfig;
                //检查创建文件配置类是否为空
                if (this.CheckConfigIsError(config)) return;
                //假设创建完文件后写入一段话，实际项目中无需这么做，这里只是一个演示
                char[] insertContent = "HellowWorld".ToCharArray();
                //转化成 byte[]
                byte[] byteArrayContent = Encoding.Default.GetBytes(insertContent, 0, insertContent.Length);
                //根据传入的配置文件中来决定是否同步或异步实例化stream对象
                FileStream stream = createFileConfig.IsAsync ?
                            new FileStream(createFileConfig.CreateUrl, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, true)
                            : new FileStream(createFileConfig.CreateUrl, FileMode.Create);
                using (stream)
                {
                    // 如果不注释下面代码会抛出异常，google上提示是WriteTimeout只支持网络流
                    // stream.WriteTimeout = READ_OR_WRITE_TIMEOUT;
                    //如果该流是同步流并且可写
                    if (!stream.IsAsync && stream.CanWrite)
                        stream.Write(byteArrayContent, 0, byteArrayContent.Length);
                    else if (stream.CanWrite)//异步流并且可写
                        stream.BeginWrite(byteArrayContent, 0, byteArrayContent.Length, this.End_CreateFileCallBack, stream);

                    stream.Close();
                }
            }
        }


        private bool CheckConfigIsError(IFileConfig config)
        {
            return config == null;
        }


        /// <summary>
        ///  异步写文件callBack方法
        /// </summary>
        /// <param name="result">IAsyncResult</param>
        private void End_CreateFileCallBack(IAsyncResult result)
        {
            //从IAsyncResult对象中得到原来的FileStream
            var stream = result.AsyncState as FileStream;
            //结束异步写

            Console.WriteLine("异步创建文件地址：{0}", stream.Name);
            stream.EndWrite(result);
        }

        /// <summary>
        /// 复制方法
        /// </summary>
        /// <param name="config">拷贝文件复制</param>
        public void Copy(IFileConfig config)
        {
            lock (_lockObject)
            {
                //得到CopyFileConfig对象
                var copyFileConfig = config as CopyFileConfig;
                // 检查CopyFileConfig类对象是否为空或者OrginalFileUrl是否为空
                if (CheckConfigIsError(copyFileConfig) || !File.Exists(copyFileConfig.OrginalFileUrl)) return;
                //创建同步或异步流
                FileStream stream = copyFileConfig.IsAsync ?
                         new FileStream(copyFileConfig.OrginalFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)
                         : new FileStream(copyFileConfig.OrginalFileUrl, FileMode.Open);
                //定义一个byte数组接受从原文件读出的byte数据
                byte[] orignalFileBytes = new byte[stream.Length];
                using (stream)
                {
                    // stream.ReadTimeout = READ_OR_WRITE_TIMEOUT;
                    //如果异步流
                    if (stream.IsAsync)
                    {
                        //将该流和读出的byte[]数据放入配置类，在callBack中可以使用
                        copyFileConfig.OriginalFileStream = stream;
                        copyFileConfig.OriginalFileBytes = orignalFileBytes;
                        if (stream.CanRead)
                            //异步开始读取，读完后进入End_ReadFileCallBack方法，该方法接受copyFileConfig参数
                            stream.BeginRead(orignalFileBytes, 0, orignalFileBytes.Length, End_ReadFileCallBack, copyFileConfig);
                    }
                    else//否则同步读取
                    {
                        if (stream.CanRead)
                        {
                            //一般读取原文件
                            stream.Read(orignalFileBytes, 0, orignalFileBytes.Length);
                        }
                        //定义一个写流，在新位置中创建一个文件
                        FileStream copyStream = new FileStream(copyFileConfig.DestinationFileUrl, FileMode.CreateNew);
                        using (copyStream)
                        {
                            //  copyStream.WriteTimeout = READ_OR_WRITE_TIMEOUT;
                            //将源文件的内容写进新文件
                            copyStream.Write(orignalFileBytes, 0, orignalFileBytes.Length);
                            copyStream.Close();
                        }
                    }
                    stream.Close();
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// 异步读写文件方法
        /// </summary>
        /// <param name="result"></param>
        private void End_ReadFileCallBack(IAsyncResult result)
        {
            //得到先前的配置文件
            var config = result.AsyncState as CopyFileConfig;
            //结束异步读
            config.OriginalFileStream.EndRead(result);
            //异步读后立即写入新文件地址
            if (File.Exists(config.DestinationFileUrl)) File.Delete(config.DestinationFileUrl);
            FileStream copyStream = new FileStream(config.DestinationFileUrl, FileMode.CreateNew);
            using (copyStream)
            {
                Console.WriteLine("异步复制原文件地址：{0}", config.OriginalFileStream.Name);
                Console.WriteLine("复制后的新文件地址：{0}", config.DestinationFileUrl);
                //调用异步写方法CallBack方法为End_CreateFileCallBack，参数是copyStream
                copyStream.BeginWrite(config.OriginalFileBytes, 0, config.OriginalFileBytes.Length, this.End_CreateFileCallBack, copyStream);
                copyStream.Close();

            }

        }
    }
}
