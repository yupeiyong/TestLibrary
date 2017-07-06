using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Stream
{
    /// <summary>
    /// 分段上传例子
    /// </summary>
    public class UpFileSingleTest
    {
        //我们定义Buffer为1000
        public const int BUFFER_COUNT = 1000;

        /// <summary>
        /// 将文件上传至服务器（本地），由于采取分段传输所以，
        /// 每段必须有一个起始位置和相对应该数据段的数据
        /// </summary>
        /// <param name="filePath">服务器上文件地址</param>
        /// <param name="startPositon">分段起始位置</param>
        /// <param name="btArray">每段的数据</param>
        private void WriteToServer(string filePath, int startPositon, byte[] btArray)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
            using (fileStream)
            {
                //将流的位置设置在该段起始位置
                fileStream.Position = startPositon;
                //将该段数据通过FileStream写入文件中，每次写一段的数据，就好比是个水池，分段蓄水一样，直到蓄满为止
                fileStream.Write(btArray, 0, btArray.Length);
            }
        }
         
        /// <summary>
        /// 处理单独一段本地数据上传至服务器的逻辑，根据客户端传入的startPostion
        /// 和totalCount来处理相应段的数据上传至服务器（本地）
        /// </summary>
        /// <param name="localFilePath">本地需要上传的文件地址</param>
        /// <param name="uploadFilePath">服务器（本地）目标地址</param>
        /// <param name="startPostion">该段起始位置</param>
        /// <param name="totalCount">该段最大数据量</param>
        public void UpLoadFileFromLocal(string localFilePath, string uploadFilePath, int startPostion, int totalCount)
        {
            //if(!File.Exists(localFilePath)){return;}
            //每次临时读取数据数
            int tempReadCount = 0;
            int tempBuffer = 0;
            //定义一个缓冲区数组
            byte[] bufferByteArray = new byte[BUFFER_COUNT];
            //定义一个FileStream对象
            FileStream fileStream = new FileStream(localFilePath, FileMode.Open);
            //将流的位置设置在每段数据的初始位置
            fileStream.Position = startPostion;
            using (fileStream)
            {
                //循环将该段数据读出再写入服务器中
                while (tempReadCount < totalCount)
                {
                    tempBuffer = BUFFER_COUNT;
                    //每段起始位置+每次循环读取数据的长度
                    var writeStartPosition = startPostion + tempReadCount;
                    //当缓冲区的数据加上临时读取数大于该段数据量时，
                    //则设置缓冲区的数据为totalCount-tempReadCount 这一段的数据
                    if (tempBuffer + tempReadCount > totalCount)
                    {
                        //缓冲区的数据为totalCount-tempReadCount
                        tempBuffer = totalCount - tempReadCount;
                        //读取该段数据放入bufferByteArray数组中
                        fileStream.Read(bufferByteArray, 0, tempBuffer);
                        if (tempBuffer > 0)
                        {
                            byte[] newTempBtArray = new byte[tempBuffer];
                            Array.Copy(bufferByteArray, 0, newTempBtArray, 0, tempBuffer);
                            //将缓冲区的数据上传至服务器
                            this.WriteToServer(uploadFilePath, writeStartPosition, newTempBtArray);
                        }

                    }
                    //如果缓冲区的数据量小于该段数据量，并且tempBuffer=设定BUFFER_COUNT时，通过
                    //while 循环每次读取一样的buffer值的数据写入服务器中，直到将该段数据全部处理完毕
                    else if (tempBuffer == BUFFER_COUNT)
                    {
                        fileStream.Read(bufferByteArray, 0, tempBuffer);
                        this.WriteToServer(uploadFilePath, writeStartPosition, bufferByteArray);
                    }

                    //通过每次的缓冲区数据，累计增加临时读取数
                    tempReadCount += tempBuffer;
                }
            }
        }
    }
    
}
