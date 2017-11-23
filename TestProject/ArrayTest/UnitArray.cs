using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestProject.ArrayTest
{
    [TestClass]
    public class UnitArray
    {
        /// <summary>
        /// 插入元素到元素
        /// </summary>
        [TestMethod]
        public void TestInsertElement()
        {
            var arr = new int[6];
            arr[0] = 1;
            arr[1] = 6;
            var element = 2;
            var count = 2;
            //插入到中间位置
            Console.WriteLine("插入到中间位置");
            var i = count - 1;
            while (i >= 0 && element < arr[i])
            {
                arr[i + 1] = arr[i];
                i--;
            }
            arr[i + 1] = element;
            count++;
            for (var l = 0; l < count; l++)
            {
                Console.WriteLine(arr[l]);
            }

            //插入到首位置
            Console.WriteLine("插入到首位置");
            element = 0;
            i = count - 1;
            while (i >= 0 && element < arr[i])
            {
                arr[i + 1] = arr[i];
                i--;
            }
            arr[i + 1] = element;
            count++;
            for (var l = 0; l < count; l++)
            {
                Console.WriteLine(arr[l]);
            }

            //插入到末尾
            Console.WriteLine("插入到末尾");
            element = 99;
            i = count - 1;
            while (i >= 0 && element < arr[i])
            {
                arr[i + 1] = arr[i];
                i--;
            }
            arr[i + 1] = element;
            count++;
            for (var l = 0; l < count; l++)
            {
                Console.WriteLine(arr[l]);
            }
        }


        [TestMethod]
        public void TestInsertIntoList()
        {
            var list = new List<int>() { 1, 6 };

            //插入到中间位置
            Console.WriteLine("插入到中间位置");
            var element = 2;
            var i = list.Count - 1;
            while (i >= 0 && element < list[i])
            {
                i--;
            }
            list.Insert(i + 1, element);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            //插入到首位置
            Console.WriteLine("插入到首位置");
            element = -1;
            i = list.Count - 1;
            while (i >= 0 && element < list[i])
            {
                i--;
            }
            list.Insert(i + 1, element);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }


            //插入到末尾
            Console.WriteLine("插入到末尾");
            element = 99;
            i = list.Count - 1;
            while (i >= 0 && element < list[i])
            {
                i--;
            }
            list.Insert(i + 1, element);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

    }
}
