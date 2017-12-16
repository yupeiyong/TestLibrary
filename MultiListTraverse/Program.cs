using System;
using System.Collections.Generic;


namespace MultiListTraverse
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var multiList = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            TraverseList(multiList);
            Console.ReadKey();
        }


        /// <summary>
        ///     遍历多个列表(动态数组，N个列表，每个列表有N个元素)
        ///     比如：
        ///     { 1, 2 }, { 3, 4 },
        ///     排列结果：
        ///     1,3
        ///     1,4
        ///     2,3
        ///     2,4
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="multiList"></param>
        public static void TraverseList<T>(List<List<T>> multiList)
        {
            var length = multiList.Count;
            var arr = new T[length];
            var indexArray = new int[length];
            var i = 0;
            while (i < length)
            {
                if (i < length - 1)
                {
                    var curLength = multiList[i].Count;
                    if (indexArray[i] < curLength)
                    {
                        arr[i] = multiList[i][indexArray[i]];
                        indexArray[i]++;
                    }
                    else
                    {
                        if (i == 0) break;
                        indexArray[i] = 0;
                        i--;
                        continue;
                    }
                }
                else
                {
                    for (var j = 0; j < multiList[i].Count; j++)
                    {
                        arr[i] = multiList[i][j];
                        Console.WriteLine(string.Join(",", arr));
                    }
                    i--;
                    continue;
                }
                i++;
            }
        }

    }

}