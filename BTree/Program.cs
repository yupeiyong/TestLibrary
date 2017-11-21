using System;


namespace BTree
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var tree = new BTree<int>(3);
            for (var i = 0; i < 200; i++)
            {
                tree.Insert(i);
            }
            tree.Traverse(x=>Console.Write(x+" "));
            Console.ReadKey();
        }

    }

}