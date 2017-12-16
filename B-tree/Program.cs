using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_tree
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new BTree<int>(3);
            for (var i = 0; i < 100; i++)
            {
                tree.Insert(i);
            }
            tree.Traverse(x => Console.Write(x + " "));


            Console.WriteLine();

            tree.Remove(6);
            Console.WriteLine("删除 6之后的整个树遍历\n");
            tree.Traverse(x => Console.Write(x + " "));
            Console.WriteLine();

            tree.Remove(13);
            Console.WriteLine("删除 13之后的整个树遍历\n");
            tree.Traverse(x => Console.Write(x + " "));
            Console.WriteLine();

            tree.Remove(7);
            Console.WriteLine("删除 7之后的整个树遍历\n");
            tree.Traverse(x => Console.Write(x + " "));
            Console.WriteLine();

            tree.Remove(4);
            Console.WriteLine("删除 4之后的整个树遍历\n");
            tree.Traverse(x => Console.Write(x + " "));
            Console.WriteLine();

            tree.Remove(2);
            Console.WriteLine("删除 2之后的整个树遍历\n");
            tree.Traverse(x => Console.Write(x + " "));
            Console.WriteLine();

            tree.Remove(16);
            Console.WriteLine("删除 16之后的整个树遍历\n");
            tree.Traverse(x => Console.Write(x + " "));
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
