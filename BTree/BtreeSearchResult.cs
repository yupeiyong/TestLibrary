using System;


namespace BTree
{

    /// <summary>
    ///     查找结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BTreeSearchResult<T> where T : IComparable<T>
    {

        /// <summary>
        ///     找到的结点
        /// </summary>
        public BTreeNode<T> Node { get; set; }


        /// <summary>
        ///     关键字位置
        /// </summary>
        public int KeywordPosition { get; set; }

    }

}