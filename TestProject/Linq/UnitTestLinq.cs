using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Linq
{
    [TestClass]
    public class UnitTestLinq
    {
        [TestMethod]
        public void TestGroupBy()
        {
            var products = Product.GetList();
            foreach (var group in products.GroupBy(p => p.Category))
            {
                Console.WriteLine(group.Key);
                foreach (var item in group)
                {
                    Console.WriteLine("\t" + item);
                }
            }
        }


        [TestMethod]
        public void TestGroupBy_Remove()
        {
            var products = Product.GetList();
            //groupBy延迟执行
            var groups = products.GroupBy(p => p.Category);
            //删除所有属于Garden的产品  
            products.RemoveAll(p => p.Category == "Garden");

            foreach (var group in groups)
            {
                Console.WriteLine(group.Key);
                foreach (var item in group)
                {
                    Console.WriteLine("\t" + item);
                }
            }
        }

        [TestMethod]
        public void TestLookup()
        {
            var products = Product.GetList();
            //ToLookup（） 方法创建一个类似 字典（Dictionary ） 的列表List, 但是它是一个新的.NET Collection 叫做 lookup。 Lookup，不像Dictionary, 是不可改变的。 这意味着一旦你创建一个lookup, 你不能添加或删除元素。
            var productsByCategory = products.ToLookup(p => p.Category);

            foreach (var group in productsByCategory)
            {
                // the key of the lookup is in key property  
                Console.WriteLine(group.Key);
                // the list of values is the group itself.  
                foreach (var item in group)
                {
                    Console.WriteLine("\t" + item);
                }
            }
            
        }

    }
}
