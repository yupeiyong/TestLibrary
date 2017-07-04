using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Stream
{
    /// <summary>
    /// 文件配置接口
    /// </summary>
    public interface IFileConfig
    {
        string FileName { get; set; }
        bool IsAsync { get; set; }
    }
}
