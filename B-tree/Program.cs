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
            Console.ReadKey();
        }
    }
}
