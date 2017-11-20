using System;


namespace BTree
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var tree = new BTree<int>(3);
            for (var i = 0; i < 20; i++)
            {
                tree.Insert(i);
            }
            Console.ReadKey();
        }

    }

}