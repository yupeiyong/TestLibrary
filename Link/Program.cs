using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link
{
    class Program
    {
        static void Main(string[] args)
        {
            var link=new LinkList<int>();
            for (var i = 0; i < 100; i++)
            {
                link.Add(i);
            }

            foreach (var item in link)
            {
                Console.Write(item+",   ");
            }
            Console.WriteLine("\r\n删除头部0---------------------------------------------------------------");
            link.Remove(0);
            foreach (var item in link)
            {
                Console.Write(item + ",   ");
            }
            Console.WriteLine("\r\n删除尾部99---------------------------------------------------------------");
            link.Remove(99);
            foreach (var item in link)
            {
                Console.Write(item + ",   ");
            }
            Console.WriteLine("\r\n删除48---------------------------------------------------------------");
            link.Remove(48);
            foreach (var item in link)
            {
                Console.Write(item + ",   ");
            }

            Console.WriteLine("\r\n查找45---------------------------------------------------------------");
            var node = link.Find(45);
            if (node == null)
            {
                Console.WriteLine("未找到元素");
            }
            else
            {
                Console.WriteLine("找到元素："+node.Data);
            }
            Console.ReadKey();
        }
    }
}
