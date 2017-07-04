using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 动态规划法解兔子问题
{
    class Program
    {
        static void Main(string[] args)
        {
            var f=new Fibonacci();
            Console.WriteLine("请输入月份：");
            var k = Console.ReadLine();
            var month = int.Parse(k);
            var count1 = f.Recursion(month);
            var count2 = f.DynamicAlignment(month);
            Console.WriteLine("用递归法解得结果：{0}", count1);
            Console.WriteLine("用动态归划法解得结果：{0}", count2);
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 兔子问题是斐波那契数 1、1、2、3、5、8、13、21
    /// </summary>
    public class Fibonacci
    {
        /// <summary>
        /// 递归法
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int Recursion(int n)
        {
            if (n == 1 || n == 2) return 1;
            return Recursion(n-1)+Recursion(n-2);
        }


        public int DynamicAlignment(int n)
        {
            var arr =new int[n];
            for (var i = 0; i < n; i++)
            {
                if (i == 0 || i == 1)
                {
                    arr[i] = 1;
                    continue;
                }
                arr[i] = arr[i - 1] + arr[i - 2];
            }
            return arr[n - 1];
        }

    }
}
