using System.IO;


namespace TestProject.Stream
{

    /// <summary>
    ///     文件复制
    /// </summary>
    public class CopyFileConfig : IFileConfig
    {

        //原文件地址
        public string OrginalFileUrl { get; set; }

        //拷贝目的地址
        public string DestinationFileUrl { get; set; }

        //文件流，异步读取后在回调方法内使用
        public FileStream OriginalFileStream { get; set; }

        //原文件字节数组，异步读取后在回调方法内使用
        public byte[] OriginalFileBytes { get; set; }

        // 文件名
        public string FileName { get; set; }

        //是否异步操作
        public bool IsAsync { get; set; }

    }

}