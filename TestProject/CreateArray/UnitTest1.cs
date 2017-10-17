using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestProject.CreateArray
{
    [TestClass]
    public class UnitTestNumberCombination
    {
        [TestMethod]
        public void TestCreateBinaryCombinations()
        {
            var combination = new NumberCombination();
            var arr = new List<int>() { 1, 2, 3, 4 };
            var nodes = combination.CreateBinaryCombinations<int>(arr);
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Count == 3);
            Assert.IsTrue(nodes[0].Left != null && nodes[0].Left.Count == 2 && nodes[0].Left[0] == 1 && nodes[0].Left[1] == 2);
            Assert.IsTrue(nodes[0].Right != null && nodes[0].Right.Count == 2 && nodes[0].Right[0] == 3 && nodes[0].Right[1] == 4);

            Assert.IsTrue(nodes[1].Left != null && nodes[1].Left.Count == 2 && nodes[1].Left[0] == 1 && nodes[1].Left[1] == 3);
            Assert.IsTrue(nodes[1].Right != null && nodes[1].Right.Count == 2 && nodes[1].Right[0] == 2 && nodes[1].Right[1] == 4);

            Assert.IsTrue(nodes[2].Left != null && nodes[2].Left.Count == 2 && nodes[2].Left[0] == 1 && nodes[2].Left[1] == 4);
            Assert.IsTrue(nodes[2].Right != null && nodes[2].Right.Count == 2 && nodes[2].Right[0] == 2 && nodes[2].Right[1] == 3);
        }


        [TestMethod]
        public void TestCreateBinaryCombinations_Length_Three()
        {
            var combination = new NumberCombination();
            var arr = new List<int>() { 1, 2, 3 };
            var nodes = combination.CreateBinaryCombinations<int>(arr);
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Count == 3);
            Assert.IsTrue(nodes[0].Left != null && nodes[0].Left.Count == 1 && nodes[0].Left[0] == 1);
            Assert.IsTrue(nodes[0].Right != null && nodes[0].Right.Count == 2 && nodes[0].Right[0] == 2 && nodes[0].Right[1] == 3);

            Assert.IsTrue(nodes[1].Left != null && nodes[1].Left.Count == 1 && nodes[1].Left[0] == 2);
            Assert.IsTrue(nodes[1].Right != null && nodes[1].Right.Count == 2 && nodes[1].Right[0] == 1 && nodes[1].Right[1] == 3);

            Assert.IsTrue(nodes[2].Left != null && nodes[2].Left.Count == 1 && nodes[2].Left[0] == 3);
            Assert.IsTrue(nodes[2].Right != null && nodes[2].Right.Count == 2 && nodes[2].Right[0] == 1 && nodes[2].Right[1] == 2);
        }

        [TestMethod]
        public void TestCreateBinaryCombinations_Length_Three2()
        {
            var combination = new NumberCombination();
            var arr = new List<int>() { 1, 2, 3 };
            var nodes = combination.CreateBinaryCombinations<int>(arr, 1);
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Count == 3);
            Assert.IsTrue(nodes[0].Left != null && nodes[0].Left.Count == 2 && nodes[0].Left[0] == 1 && nodes[0].Left[1] == 2);
            Assert.IsTrue(nodes[0].Right != null && nodes[0].Right.Count == 1 && nodes[0].Right[0] == 3);

            Assert.IsTrue(nodes[1].Left != null && nodes[1].Left.Count == 2 && nodes[1].Left[0] == 1 && nodes[1].Left[1] == 3);
            Assert.IsTrue(nodes[1].Right != null && nodes[1].Right.Count == 1 && nodes[1].Right[0] == 2);

            Assert.IsTrue(nodes[2].Left != null && nodes[2].Left.Count == 2 && nodes[2].Left[0] == 2 && nodes[2].Left[1] == 3);
            Assert.IsTrue(nodes[2].Right != null && nodes[2].Right.Count == 1 && nodes[2].Right[0] == 1);
        }

        [TestMethod]
        public void TestCreateBinaryCombinations_Length1()
        {
            var combination = new NumberCombination();
            var arr = new List<int>() { 1 };
            var nodes = combination.CreateBinaryCombinations<int>(arr);
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Count == 1);
            Assert.IsTrue(nodes[0].Left != null && nodes[0].Left.Count == 1 && nodes[0].Left[0] == 1);
            Assert.IsTrue(nodes[0].Right != null && nodes[0].Right.Count == 0);
        }

        [TestMethod]
        public void TestCreateBinaryCombinations_Length2()
        {
            var combination = new NumberCombination();
            var arr = new List<int>() { 1, 2 };
            var nodes = combination.CreateBinaryCombinations<int>(arr);
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Count == 1);
            Assert.IsTrue(nodes[0].Left != null && nodes[0].Left.Count == 1 && nodes[0].Left[0] == 1);
            Assert.IsTrue(nodes[0].Right != null && nodes[0].Right.Count == 1 && nodes[0].Right[0] == 2);
        }

        [TestMethod]
        public void TestCreateBinaryCombinations_Length49()
        {
            var combination = new NumberCombination();
            var arr = new List<int>();
            for (var i = 1; i <= 10; i++)
            {
                arr.Add(i);
            }
            var nodes = combination.CreateBinaryCombinations<int>(arr);
            Assert.IsNotNull(nodes);
            Assert.IsTrue(nodes.Count == 1);
            Assert.IsTrue(nodes[0].Left != null && nodes[0].Left.Count == 1 && nodes[0].Left[0] == 1);
            Assert.IsTrue(nodes[0].Right != null && nodes[0].Right.Count == 1 && nodes[0].Right[0] == 2);
        }

    }
}