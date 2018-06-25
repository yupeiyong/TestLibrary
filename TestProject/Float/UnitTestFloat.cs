using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestProject.Float
{

    [TestClass]
    public class UnitTestFloat
    {

        [TestMethod]
        public void TestMethod1()
        {
            //      在Java中float和double类型的数据，无法精确表示计算结果。这是由于float和double是不精确的计算。
            //例：
            //  System.out.println(0.05 + 0.01);
            //      System.out.println(1.0 - 0.42);
            //      System.out.println(4.015 * 100);
            //      System.out.println(123.3 / 100);
            //      System.out.println(0.05);
            //      得到如下结果：
            //  0.060000000000000005
            //  0.5800000000000001
            // 401.49999999999994
            // 1.2329999999999999
            Console.WriteLine(0.05 + 0.01);
            Console.WriteLine(1.0 - 0.42);
            Console.WriteLine(4.015*100);
            Console.WriteLine(123.3/100);
            Console.WriteLine(0.05);
        }


        /// <summary>
        ///     浮点数比较大小
        /// //当x与0之差的绝对值小于0.00001（即：1e-5）时 认为x等于y   
        /// </summary>
        [TestMethod]
        public void Test_Compare()
        {
            var x = 0.6;
            var y = 0.61;
            Console.WriteLine(Math.Abs(x - y) < 0.00001);
        }

    }

}