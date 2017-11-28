using System;


namespace B_tree
{

    /// <summary>
    ///     b树，多路平衡树，每个结点有多个关键字，所有叶结点都在同一层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BTree<T> where T : IComparable<T>
    {

        private BTreeNode<T> _root;


        public BTree(int m)
        {
            M = m;
        }


        /// <summary>
        ///     M阶
        ///     每个结点的度，
        ///     其中根结点最小子树>=2(因为是平衡树，所以子树必须大于1)
        ///     关键字数量M-1到M*2-1
        ///     子结点数量M到M*2
        /// </summary>
        private int M { get; set; }


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
        /// </summary>
        /// <param name="keyword"></param>
        public void Insert(T keyword)
        {
            //1、根结点为空
            //初始化根结点
            if (_root == null)
            {
                _root = new BTreeNode<T>
                {
                    IsLeaf = true,
                    KeywordCount = 1,
                    Keywords = new T[M*2 - 1],
                    Children = new BTreeNode<T>[M*2]
                };
                _root.Keywords[0] = keyword;
            }

            //2、根结点关键字数量等于最大数量，分裂为两个结点，中间关键字插入到父结点
            else if (_root.KeywordCount == 2*M - 1)
            {
                var parent = new BTreeNode<T>
                {
                    Keywords = new T[M*2 - 1],
                    Children = new BTreeNode<T>[M*2],
                    IsLeaf = false
                };

                //新结点的第一个子结点为当前根结点
                parent.Children[0] = _root;

                //分裂根结点，根结点分裂为两个结点，中间关键字插入到新结点
                SplitNode(parent, 0, _root);

                //判断应该插入到哪个子结点，左右？
                //如果指定位置关键字小于待插入的关键字，插入右结点，否则左结点
                var i = 0;
                if (parent.Keywords[i].CompareTo(keyword) < 0) i++;
                InsertNonFull(parent.Children[i], keyword);

                //根结点等于新结点，整个B树高度增加一层
                _root = parent;
            }

            //3、根结点不满
            else
            {
                InsertNonFull(_root, keyword);
            }
        }


        /// <summary>
        ///     插入非满结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyword"></param>
        private void InsertNonFull(BTreeNode<T> node, T keyword)
        {
            while (true)
            {
                //叶结点，直接将关键插入到关键数组
                if (node.IsLeaf)
                {
                    //调整关键字位置
                    var i = node.KeywordCount - 1;

                    //遍历所有关键字并判断每个关键字是否大于插入关键字
                    while (i >= 0 && node.Keywords[i].CompareTo(keyword) > 0)
                    {
                        node.Keywords[i + 1] = node.Keywords[i];
                        i--;
                    }
                    node.Keywords[i + 1] = keyword;
                    node.KeywordCount++;
                }
                else
                {
                    //调整子结点位置
                    var i = node.KeywordCount - 1;

                    //遍历所有关键字并判断每个关键字是否大于插入关键字
                    while (i >= 0 && node.Keywords[i].CompareTo(keyword) > 0)
                    {
                        i--;
                    }

                    //要插入的子结点的关键字数量是否已满
                    //如果已满就分裂
                    if (node.Children[i + 1].KeywordCount == M*2 - 1)
                    {
                        //分裂子结点
                        SplitNode(node, i + 1, node.Children[i + 1]);

                        //因为新插入了关键字
                        //重新判断应该插入哪个子结点
                        if (node.Keywords[i + 1].CompareTo(keyword) < 0)
                        {
                            i++;
                        }
                    }
                    node = node.Children[i + 1];
                    continue;
                }
                break;
            }
        }


        /// <summary>
        ///     分裂结点
        /// </summary>
        /// <param name="parent">中间关键字将要插入的结点</param>
        /// <param name="position">中间关键字插入的位置</param>
        /// <param name="node">被分裂的结点</param>
        private void SplitNode(BTreeNode<T> parent, int position, BTreeNode<T> node)
        {
            //新结点，关键字为被分裂的结点左边的关键字
            var leftNode = new BTreeNode<T>
            {
                Keywords = new T[M*2 - 1],
                Children = new BTreeNode<T>[M*2],
                IsLeaf = node.IsLeaf
            };

            //复制左部到新结点
            /*
             如M阶为4，关键字数组满的话，如下：
             0 1 2 M-1 4 5 6
             M-1＝3正好在中间位置
             */
            for (var i = 0; i < M - 1; i++)
            {
                leftNode.Keywords[i] = node.Keywords[M + i];
                leftNode.KeywordCount++;
            }
            if (!node.IsLeaf)
            {
                //非叶子结点，复制子结点到新结点
                /*
                 如M阶为4，关键字数组满的话，如下：
                 关键字：0   1   2   M-1   4   5   6
                 子结点：P0  P1  P2  PM-1  P4  P5  P6   P7
                 M正好在中间位置
                 新结点关键字 4 5 6
                 对应的子结点 P4  P5  P6   P7
                 原结点关键字 0   1   2
                 对应的子结点 P0  P1  P2  PM
                 中间结点M将会被分裂出去
                 */
                for (var i = 0; i < M; i++)
                {
                    leftNode.Children[i] = node.Children[M + i];
                }
            }

            //被分裂结点当前的关键字数量
            node.KeywordCount = M - 1;

            //调整父结点关键字
            //从指定的位置position到关键字数组末尾，向后移一位，留出位置插入中间关键字
            for (var i = parent.KeywordCount - 1; i >= position; i--)
            {
                parent.Keywords[i + 1] = parent.Keywords[i];
            }

            //调整父结点的子结点
            if (!parent.IsLeaf)
            {
                //从指定的位置position到子结点数组末尾，向后移一位，留出位置插入中间关键字的子结点
                for (var i = parent.KeywordCount; i >= position+1; i--)
                {
                    parent.Children[i + 1] = parent.Children[i];
                }
            }

            //插入被分裂结点的中间关键字
            parent.Keywords[position] = node.Keywords[M - 1];

            //指定位置的右子结点为新分裂出来的左结点
            parent.Children[position + 1] = leftNode;

            //关键字数量加1
            parent.KeywordCount++;
        }

        #endregion


        #region 查找

        /// <summary>
        ///     查找关键字
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public BTreeSearchResult<T> Search(T keyword)
        {
            if (_root == null || _root.KeywordCount == 0) return null;
            return SearchNode(_root, keyword);
        }


        /// <summary>
        ///     查找关键字
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public BTreeSearchResult<T> SearchNode(BTreeNode<T> node, T keyword)
        {
            var i = 0;
            while (i < node.KeywordCount && node.Keywords[i].CompareTo(keyword) < 0)
                i++;
            if (node.Keywords[i].CompareTo(keyword) == 0)
            {
                return new BTreeSearchResult<T> {Node = node, KeywordPosition = i};
            }
            if (node.IsLeaf) return null;

            return SearchNode(node.Children[i], keyword);
        }

        #endregion


        #region 遍历

        /// <summary>
        ///     遍历所有关键字
        /// </summary>
        public void Traverse(Action<T> action)
        {
            if (_root == null || _root.KeywordCount == 0) return;
            TraverseNode(_root, action);
        }


        /// <summary>
        ///     中序遍历所有关键字
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        private static void TraverseNode(BTreeNode<T> node, Action<T> action)
        {
            while (true)
            {
                if (node != null && node.KeywordCount > 0)
                {
                    var i = 0;
                    while (i < node.KeywordCount)
                    {
                        if (!node.IsLeaf)
                        {
                            TraverseNode(node.Children[i], action);
                        }
                        action(node.Keywords[i]);
                        i++;
                    }
                    if (!node.IsLeaf)
                    {
                        node = node.Children[i];
                        continue;
                    }
                }
                break;
            }
        }

        #endregion
    }

}