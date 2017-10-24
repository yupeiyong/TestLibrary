using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree=new BinaryTree<int>();

            for (var i = 0; i < 20; i++)
            {
                tree.Add(i);
            }

            Console.WriteLine("\r\n先序遍历");
            tree.Preorder(d=>Console.Write(d+", "));
            Console.WriteLine("\r\n中序遍历");
            tree.Inorder(d => Console.Write(d + ", "));
            Console.WriteLine("\r\n后序遍历");
            tree.Postorder(d => Console.Write(d + ", "));
            Console.WriteLine("\r\n层序遍历");
            tree.Levelorder(d => Console.Write(d + ", "));

            Console.WriteLine("\r\n查找15");
            var result=tree.Find(15);
            if (result)
            {
                Console.WriteLine("\r\n查找15成功！");
            }
            else
            {
                Console.WriteLine("\r\n查找15失败！");
            }
            Console.ReadKey();
        }
    }
}
