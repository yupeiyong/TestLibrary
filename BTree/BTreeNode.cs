using System;


namespace BTree
{

    /// <summary>
    ///     B树结点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BTreeNode<T> where T : IComparable<T>
    {

        /// <summary>
        ///     结点数量
        /// </summary>
        public int NodeCount { get; set; }


        /// <summary>
        ///     关键字
        /// </summary>
        public T[] Keys { get; set; }


        /// <summary>
        ///     子结点
        /// </summary>
        public BTreeNode<T>[] Children { get; set; }


        /// <summary>
        ///     是否叶结点
        /// </summary>
        public bool IsLeaf { get; set; }

    }

}