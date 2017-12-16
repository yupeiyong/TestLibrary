using System;


namespace B_tree
{

    /// <summary>
    ///     B树结点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BTreeNode<T> where T : IComparable<T>
    {

        /// <summary>
        ///     关键字
        /// </summary>
        public T[] Keywords;

        /// <summary>
        ///     关键字当前数量
        /// </summary>
        public int KeywordCount { get; set; }


        /// <summary>
        ///     子结点
        /// </summary>
        public BTreeNode<T>[] Children { get; set; }


        /// <summary>
        ///     是否叶子结点
        /// </summary>
        public bool IsLeaf { get; set; }

    }

}