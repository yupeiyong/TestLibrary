using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Tree
{
    [TestClass]
    public class UnitTestBinaryTree
    {
        [TestMethod]
        public void TestAdd()
        {
            var tree=new BinaryTree<int>();
            var array =new int[] {1,2,3,4,5 };
            foreach (var item in array)
            {
                tree.Add(item);
            }
            Assert.IsTrue(tree.Count==array.Length);
        }


        [TestMethod]

        public void TestPreorderTraversal()
        {
            var tree = new BinaryTree<int>();
            var array = new int[] { 1, 2, 3, 4, 5 };
            foreach (var item in array)
            {
                tree.Add(item);
            }
            tree.PostorderTraversal(Console.WriteLine);
        }
    }
}
