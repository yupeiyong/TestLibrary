using System;


namespace BTree
{

    /// <summary>
    ///     B树 平衡多路查找树
    ///     树的高度决定了读取磁盘的次数，因为是多叉，高度比二叉查找树少了很多，所以相对来说读取次数也少了很多
    /// </summary>
    /// <typeparam name="T">泛型类型，实现了IComparable接口的类型</typeparam>
    public class BTree<T> where T : IComparable<T>
    {

        private BTreeNode<T> _root;


        public BTree(int m)
        {
            M = m;
        }


        /// <summary>
        ///     M阶
        ///     每个结点最小的度，
        ///     其中根结点最小子树>=2(因为是平衡树，所以子树必须大于1) 
        ///     关键字数量M-1到M*2-1
        ///     子结点数量M到M*2
        /// </summary>
        public int M { get; set; }


        #region 删除

        /// <summary>
        ///     删除关键字
        /// </summary>
        /// <param name="keyword"></param>
        public void Remove(T keyword)
        {
        }

        #endregion


        #region 新增

        /// <summary>
        ///     插入关键字
        ///     从根结点开始，先判断根结点是否满，如果满则先分裂再插入字，否则直接插入关键字
        ///     所有关键字都插入到叶结点
        /// </summary>
        /// <param name="keyword"></param>
        public void Insert(T keyword)
        {
            //根结点为空
            if (_root == null)
            {
                _root = new BTreeNode<T>
                {
                    Keys = new T[M * 2 - 1],
                    Children = new BTreeNode<T>[M * 2],
                    IsLeaf = true
                };
                _root.Keys[0] = keyword;
                _root.NodeCount = 1;
            }
            else
            {
                //结点已满,先分裂根结点，再插入关键字
                if (_root.NodeCount == 2 * M - 1)
                {
                    var parentNode = new BTreeNode<T>
                    {
                        Keys = new T[M * 2 - 1],
                        Children = new BTreeNode<T>[M * 2]
                    };
                    parentNode.Children[0] = _root;
                    SplitNode(parentNode, 0, _root);
                    var i = 0;
                    if (parentNode.Keys[0].CompareTo(keyword) < 0)
                        i++;

                    InsertNonFull(parentNode.Children[i], keyword);
                    _root = parentNode;
                }
                else
                {
                    //根结点不满，直接插入关键字
                    InsertNonFull(_root, keyword);
                }
            }
        }


        private void InsertNonFull(BTreeNode<T> node, T keyword)
        {
            if (node.IsLeaf)
            {
                var i = node.NodeCount - 1;
                while (i >= 0 && node.Keys[i].CompareTo(keyword) > 0)
                {
                    node.Keys[i + 1] = node.Keys[i];
                    i--;
                }
                node.Keys[i + 1] = keyword;
                node.NodeCount++;
            }
            else
            {
                var i = node.NodeCount - 1;
                while (i >= 0 && node.Keys[i].CompareTo(keyword) > 0)
                    i--;

                if (node.Children[i + 1].NodeCount == 2 * M - 1)
                {
                    SplitNode(node, i + 1, node.Children[i + 1]);
                    i = 0;
                    if (node.Keys[0].CompareTo(keyword) < 0)
                        i++;
                }
                InsertNonFull(node.Children[i + 1], keyword);
            }
        }


        private void SplitNode(BTreeNode<T> parentNode, int position, BTreeNode<T> node)
        {
            //新结点
            var leftNode = new BTreeNode<T>
            {
                Keys = new T[M * 2 - 1],
                Children = new BTreeNode<T>[M * 2],
                IsLeaf = node.IsLeaf,
                NodeCount = M - 1
            };
            //复制左部到新结点
            for (var i = 0; i < M - 1; i++)
            {
                leftNode.Keys[i] = node.Keys[M + i];
            }
            if (!node.IsLeaf)
            {
                for (var i = 0; i < M; i++)
                {
                    leftNode.Children[i] = node.Children[M + i];
                }
            }
            //调整结点现有数量
            node.NodeCount = M - 1;

            //调整父结点
            //父结点的子树向后移动
            for (var i = parentNode.NodeCount; i >= position + 1; i--)
            {
                parentNode.Children[i + 1] = parentNode.Children[i];
            }

            parentNode.Children[position + 1] = leftNode;

            //父结点的关键字向后移动
            for (var i = parentNode.NodeCount-1; i >= position; i--)
            {
                parentNode.Keys[i + 1] = parentNode.Keys[i];
            }

            parentNode.Keys[position] = node.Keys[M - 1];
            parentNode.NodeCount++;
        }

        #endregion


        #region 查找

        /// <summary>
        ///     查找关键所在结点
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public BTreeNode<T> Search(T keyword)
        {
            return SearchNode(_root, keyword);
        }


        private BTreeNode<T> SearchNode(BTreeNode<T> node, T keyword)
        {
            var i = 0;
            while (i < node.NodeCount && node.Keys[i].CompareTo(keyword) < 0) i++;
            if (node.Keys[i].CompareTo(keyword) == 0) return node;
            return node.IsLeaf ? null : SearchNode(node.Children[i], keyword);
        }

        #endregion


        #region 遍历

        /// <summary>
        ///     中序遍历所有结点
        /// </summary>
        public void Traverse(Action<T> action)
        {
            if (_root == null) return;
            TraverseNode(_root, action);
        }


        private static void TraverseNode(BTreeNode<T> node, Action<T> action)
        {
            var i = 0;
            for (; i < node.NodeCount; i++)
            {
                if (!node.IsLeaf)
                {
                    TraverseNode(node.Children[i], action);
                }
                action(node.Keys[i]);
            }
            if (!node.IsLeaf)
                TraverseNode(node.Children[i], action);
        }

        #endregion
    }

}