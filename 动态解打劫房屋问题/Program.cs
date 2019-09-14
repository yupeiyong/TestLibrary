using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 动态解打劫房屋问题
{
    /*
     打劫房屋问题

【问题】假设你是一个专业的窃贼，准备沿着一条街打劫房屋。每个房子都存放着特定金额的钱。你面临的唯一约束条件是：相邻的房子装着相互联系的防盗系统，且 当相邻的两个房子同一天被打劫时，该系统会自动报警。
  给定一个非负整数列表，表示每个房子中存放的钱， 算一算，如果今晚去打劫，你最多可以得到多少钱 在不触动报警装置的情况下。

【递归思路】问题可以描述为，抢劫所有的房屋，使得抢劫的钱最多。 那么，这个问题的最优解，可以由子问题的最优解构造：除了当前房屋以外，抢劫剩余房屋获得的最多的钱，那么对于当前房屋会有两种策略，抢劫或者不抢劫。 所以，抢劫所有房屋获得的做多的钱 = max（抢劫当下房屋的钱+抢劫剩余n-2个房屋获得的最多的钱， 抢劫剩余n-1个房屋获得的钱）

递归问题可以描述为：从0位置开始到结束位置打劫的最高收益，它等于打劫当前房屋+从下下个位置开始打劫的最高收益 或者 从下个位置开始打劫的最高收益的 两者的最大值。
     */
    class Program
    {
        static void Main(string[] args)
        {
            var arr = new int[] {89,56,12,36,12,55,63,10 };//89+36+55+10
            var max = Recursion(arr, arr.Length-1 );

            var arr2 = new int[arr.Length];
            for (var i = 0; i < arr2.Length; i++)
            {
                arr2[i] = int.MinValue;
            }
            Console.WriteLine(max);

            var max2 = Dy(arr,arr2, arr.Length - 1);
            Console.WriteLine(max2);

            var max3 = Dy2(arr);
            Console.WriteLine(max3);

            Console.ReadKey();
        }

        static int Recursion(int[]arr,int n)
        {
            if (n < 0)
                return 0;

            return Math.Max(Recursion(arr, n - 1), arr[n] + Recursion(arr, n - 2));
        }

        static int Dy(int[] arr, int[]arr2,int n)
        {
            if (n < 0)
                return 0;

            if (arr2[n] != Int32.MinValue)
                return arr2[n];

            arr2[n] = Math.Max(Dy(arr,arr2, n - 1), arr[n] + Dy(arr, arr2,n - 2));
            return arr2[n];
        }

        static int Dy2(int[] arr)
        {
            if (arr == null || arr.Length == 0)
                return 0;

            if (arr.Length == 1)
                return arr[0];

            var arr3 = new int[arr.Length];
            arr3[0] = arr[0];
            arr3[1] = Math.Max(arr[1], arr3[0]);

            if (arr.Length == 2)
                return arr3[1];

            for (var i = 2; i < arr.Length; i++)
            {
                arr3[i] = Math.Max(arr3[i - 1], arr[i] + arr3[i - 2]);
            }

            return arr3[arr3.Length - 1];
        }
    }
}
