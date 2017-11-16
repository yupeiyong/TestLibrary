using System;
using System.Collections.Generic;


namespace BTree
{

    /// <summary>
    ///     树节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> where T : IComparable<T>
    {

        /// <summary>
        ///     元素个数
        /// </summary>
        public int ElementCount = 0;

        /// <summary>
        ///     元素集合,存在elementNum个
        /// </summary>
        public IList<T> Elements = new List<T>();

        /// <summary>
        ///     是否为叶子节点
        /// </summary>
        public bool IsLeaf = true;

        /// <summary>
        ///     元素指针，存在elementNum+1
        /// </summary>
        public IList<TreeNode<T>> Pointers = new List<TreeNode<T>>();

    }

}