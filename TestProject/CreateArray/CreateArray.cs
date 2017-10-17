using System;
using System.Collections.Generic;
using System.Linq;


namespace TestProject.CreateArray
{
    /// <summary>
    /// 号码组合
    /// </summary>
    public class NumberCombination
    {
        /// <summary>
        /// 按输入数组生成二分组合集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="middleIndex">进行二分组合时，从0开始的索引位置</param>
        /// <returns></returns>
        public List<BinaryNode<T>> CreateBinaryCombinations<T>(List<T> arr, int middleIndex = 0) where T : struct
        {
            if (arr == null || arr.Count() == 0)
                throw new Exception("错误，输入的集合为空！");

            var nodes = new List<BinaryNode<T>>();
            var length = arr.Count();
            if (length == 1)
            {
                nodes.Add(new BinaryNode<T> { Left = new List<T>() { arr[0] }, Right = new List<T>() });
                return nodes;
            }
            else if (length == 2)
            {
                nodes.Add(new BinaryNode<T> { Left = new List<T>() { arr[0] }, Right = new List<T>() { arr[1] } });
                return nodes;
            }
            if (middleIndex == 0)
            {
                //如果数组长度为奇数，和偶数取相同的中间位置索引，如[1,2,3,4,5]=>[1,2][3,4,5]
                middleIndex = length / 2 - 1;
            }
            //声明新数组，记录数组左部索引变化
            var leftIndexArray = new int[middleIndex + 1];
            for (var i = 0; i <= middleIndex; i++)
            {
                leftIndexArray[i] = i;
            }

            //当前位置
            var currentIndex = middleIndex;
            //处理左部数组时，头部索引位置
            var top = length % 2 == 0 ? 1 : 0;
            do
            {
                if (currentIndex == middleIndex)
                {
                    for (var i = leftIndexArray[middleIndex]; i < length; i++)
                    {
                        //记录组合数
                        nodes.Add(CreateNode<T>(arr, leftIndexArray));
                        leftIndexArray[currentIndex]++;
                    }
                }
                currentIndex -= 1;
                while (currentIndex >= top)
                {
                    var indexValue = leftIndexArray[currentIndex];
                    if (indexValue < length - currentIndex) break;
                    currentIndex--;
                }
                if (currentIndex >= top)
                {
                    var indexValue = leftIndexArray[currentIndex];
                    for (var i = currentIndex; i <= middleIndex; i++)
                    {
                        leftIndexArray[i] = ++indexValue;
                    }
                    currentIndex = middleIndex;
                }

            } while (currentIndex >= top);
            return nodes;
        }

        private BinaryNode<T> CreateNode<T>(List<T> arr, int[] leftIndexArray) where T : struct
        {

            var leftArry = new List<T>();
            var rightArray = new List<T>();
            for (var i = 0; i < arr.Count; i++)
            {
                var item = arr[i];
                if (leftIndexArray.Contains(i))
                {
                    leftArry.Add(item);
                }
                else
                {
                    rightArray.Add(item);
                }
            }
            return new BinaryNode<T>() { Left = leftArry, Right = rightArray };
        }
    }

    /// <summary>
    /// 二分节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryNode<T> where T : struct
    {
        public List<T> Left { get; set; }
        public List<T> Right { get; set; }
    }
}