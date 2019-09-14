using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 动态规划求三角形数字之和最大
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] arr = new int[6, 32];
            int[,] sum = new int[6, 32];
            var rnd = new Random();
            for (var r = 0; r < arr.GetLongLength(0); r++)
            {
                for (var c = 0; c < arr.GetLongLength(1); c++)
                {
                    arr[r, c] = rnd.Next(1, 16);
                    sum[r, c] = int.MinValue;
                }
            }

            var maxSum = CalcRecursion(arr, 5, 0, 0);
            CalcDy(arr, sum, 5, 0, 0);
            Console.Write(maxSum);
            Console.Write(sum[0,0]);

            Console.ReadKey();
        }


        /// <summary>
        /// 递归法解
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        static int CalcRecursion(int[,] arr, int n, int r, int c)
        {
            if (r == n)
            {
                return arr[r, c];
            }

            var x = CalcRecursion(arr, n, r + 1, c);
            var y = CalcRecursion(arr, n, r + 1, c + 1);
            return Math.Max(x, y) + arr[r, c];
        }

        /// <summary>
        /// 动态规划法解
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="sum"></param>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        static int CalcDy(int[,] arr, int[,] sum, int n, int r, int c)
        {
            if (sum[r, c] != int.MinValue)
            {
                return sum[r, c];
            }
            if (r == n)
            {
                sum[r, c] = arr[r, c];
            }
            else
            {
                var x = CalcRecursion(arr, n, r + 1, c);
                var y = CalcRecursion(arr, n, r + 1, c + 1);
                sum[r, c] = Math.Max(x, y) + arr[r, c];
            }
            return sum[r, c];
        }

    }
}
