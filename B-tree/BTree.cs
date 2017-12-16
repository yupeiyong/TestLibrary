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
        ///     关键字在数组中的索引位置
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private int IndexOfKeywords(BTreeNode<T> node, T keyword)
        {
            var i = 0;
            while (i < node.KeywordCount && node.Keywords[i].CompareTo(keyword) < 0)
                i++;
            return i;
        }


        /// <summary>
        ///     删除关键字
        /// </summary>
        /// <param name="keyword"></param>
        public void Remove(T keyword)
        {
            if (_root == null)
                throw new Exception("错误，B树为空！");

            RemoveNode(_root, keyword);
            if (_root.KeywordCount == 0)
            {
                _root = _root.IsLeaf ? null : _root.Children[0];
            }
        }


        /// <summary>
        ///     删除关键字
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyword"></param>
        private void RemoveNode(BTreeNode<T> node, T keyword)
        {
            while (true)
            {
                var index = IndexOfKeywords(node, keyword);
                if (index < node.KeywordCount && node.Keywords[index].CompareTo(keyword) == 0)
                {
                    if (node.IsLeaf)
                    {
                        RemoveFromLeafNode(node, index);
                    }
                    else
                    {
                        RemoveFromNonLeftNode(node, index);
                    }
                }
                else
                {
                    if (node.IsLeaf)
                        throw new Exception($"错误，未找到关键字{keyword},删除失败！");

                    var flag = index == node.KeywordCount;

                    //子结点关键字小于M阶
                    //1、合并 2、向前一兄弟结点借 3、向后一兄弟结点借（为的是满足M阶）
                    if (node.Children[index].KeywordCount < M)
                        Fill(node, index);

                    //是最后一个子结点并且合并了的
                    if (flag && index > node.KeywordCount)
                    {
                        //关键字合并到了前一兄弟结点
                        //在前一兄弟结点执行删除
                        node = node.Children[index - 1];
                        continue;
                    }
                    node = node.Children[index];
                    continue;
                }
                break;
            }
        }


        /// <summary>
        ///     index索引位置的关键字不够M阶，需要让兄弟结点借或者合并兄弟结点和父结点
        ///     //1、合并 2、向前一兄弟结点借 3、向后一兄弟结点借（为的是满足M阶）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        private void Fill(BTreeNode<T> node, int index)
        {
            //左兄弟结点关键字数量是否大于等于M阶，是的话向左兄弟借关键字
            if (index != 0 && node.Children[index - 1].KeywordCount >= M)
            {
                BorrowFromPreChild(node, index);
            }

            //否则看右兄弟结点关键字数量大于等于M阶，是的话向右兄弟借关键字
            else if (index != node.KeywordCount && node.Children[index + 1].KeywordCount >= M)
            {
                BorrowFromNextChild(node, index);
            }

            //左右兄弟关键字都不够，则合并
            else
            {
                //子结点索引位置＝最后位置
                if (index == node.KeywordCount)
                {
                    Merge(node, index - 1);
                }
                else
                {
                    Merge(node, index);
                }
            }
        }


        /// <summary>
        ///     从叶子结点删除
        /// </summary>
        private void RemoveFromLeafNode(BTreeNode<T> node, int index)
        {
            //从索引位置向前移动元素，占据index位置的关键字
            for (var i = index; i < node.KeywordCount - 1; i++)
            {
                node.Keywords[i] = node.Keywords[i + 1];
            }
            node.KeywordCount--;
        }


        /// <summary>
        ///     从非叶子结点删除
        /// </summary>
        private void RemoveFromNonLeftNode(BTreeNode<T> node, int index)
        {
            if (node.Children[index].KeywordCount >= M)
            {
                //从左子结点中找最大关键字
                var max = GetMaxKeywordFromChildren(node.Children[index]);

                //替换为子结点的最大关键字
                node.Keywords[index] = max;

                //删除左子结点中的最大关键字
                RemoveNode(node.Children[index], max);
            }
            else if (node.Children[index + 1].KeywordCount >= M)
            {
                //从右子结点中找最小关键字
                var min = GetMinKeywordFromChildren(node.Children[index + 1]);

                //替换为右子结点的最小关键字
                node.Keywords[index] = min;

                //删除右子结点的最小关键字
                RemoveNode(node.Children[index + 1], min);
            }
        }


        /// <summary>
        ///     从子结点找到最大关键字
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private T GetMaxKeywordFromChildren(BTreeNode<T> node)
        {
            var curNode = node;
            while (!curNode.IsLeaf)

                //最后子结点
                curNode = curNode.Children[curNode.KeywordCount];

            //返回最大关键字
            return curNode.Keywords[curNode.KeywordCount - 1];
        }


        /// <summary>
        ///     从子结点找到最小关键字
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private T GetMinKeywordFromChildren(BTreeNode<T> node)
        {
            var curNode = node;
            while (!curNode.IsLeaf)

                //首子结点
                curNode = curNode.Children[0];

            //返回最小关键字
            return curNode.Keywords[0];
        }


        /// <summary>
        ///     合并
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        private void Merge(BTreeNode<T> node, int index)
        {
            //指定索引位置的子结点
            var curNode = node.Children[index];
            //当前子结点的下一兄弟结点
            var nextSibiling = node.Children[index + 1];

            curNode.Keywords[curNode.KeywordCount] = node.Keywords[index];
            //复制兄弟结点的关键字到当前结点
            for (var i = 0; i < nextSibiling.KeywordCount; i++)
            {
                curNode.Keywords[curNode.KeywordCount + 1 + i] = nextSibiling.Keywords[i];
            }
            if (!curNode.IsLeaf)
            {
                //复制兄弟结点的子结点到当前结点
                for (var i = 0; i <= nextSibiling.KeywordCount; i++)
                {
                    curNode.Children[curNode.KeywordCount + 1 + i] = nextSibiling.Children[i];
                }

            }

            //调整当前结点的父结点

            //关键字从index索引位置开始，向前移动一位
            for (var i = index; i < node.KeywordCount - 1; i++)
            {
                node.Keywords[i] = node.Keywords[i + 1];
            }

            //子结点从index索引位置开始，向前移动一位
            for (var i = index + 1; i < node.KeywordCount; i++)
            {
                node.Children[i] = node.Children[i + 1];
            }

            //当前结点的关键字数量
            curNode.KeywordCount += nextSibiling.KeywordCount + 1;

            //父结点关键字数量调整
            node.KeywordCount--;

            //兄弟结点置空
            nextSibiling = null;
        }


        /// <summary>
        ///     从前一兄弟结点借最大关键字
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        private void BorrowFromPreChild(BTreeNode<T> node, int index)
        {
            /**
            1、关键字
                index索引位置的子结点 首关键字等于，它父结点的index-1索引位置的关键字
                它父结点的index-1索引位置的关键字，等于index-1索引位置的子结点最后一个的关键字 keywords[KeywordCount-1]
            2、子结点 
                如果index索引位置的子结点不是叶结点，它的第一个子结点等于它的前一个兄弟结点（父结点index-1索引位置的子结点）最后一个子结点，
                因为父结点的index-1索引位置的关键字的子结点就是这个索引位置
             */

            //指定索引位置的子结点
            var curNode = node.Children[index];

            //当前子结点的前兄弟结点
            var preSibiling = node.Children[index - 1];

            //关键字向后移动一位
            for (var i = curNode.KeywordCount - 1; i >= 0; i--)
            {
                curNode.Keywords[i + 1] = curNode.Keywords[i];
            }

            //如果是非叶结点，子结点也向后移动一位
            if (!curNode.IsLeaf)
            {
                for (var i = curNode.KeywordCount; i >= 0; i--)
                {
                    curNode.Children[i + 1] = curNode.Children[i];
                }

                //首子结点等于前一兄弟结点的最后一个子结点
                curNode.Children[0] = preSibiling.Children[preSibiling.KeywordCount];
            }

            //首关键字＝父结点index-1索引位置的关键字
            curNode.Keywords[0] = node.Keywords[index - 1];

            node.Keywords[index - 1] = preSibiling.Keywords[preSibiling.KeywordCount - 1];
            curNode.KeywordCount++;
            preSibiling.KeywordCount--;
        }


        /// <summary>
        ///     从下一兄弟结点借最小关键字
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        private void BorrowFromNextChild(BTreeNode<T> node, int index)
        {
            /**
            1、关键字
                index索引位置的子结点 最后关键字等于，它父结点的index+1索引位置的关键字
                它父结点的index+1索引位置的关键字，等于index+1索引位置的子结点第一个关键字 keywords[0]
            2、子结点 
                如果index索引位置的子结点不是叶结点，它的最后一个子结点等于它的下一个兄弟结点（父结点index+1索引位置的子结点）第一个子结点，
                因为父结点的index+1索引位置的关键字的子结点就是这个索引位置
             */

            //指定索引位置的子结点
            var curNode = node.Children[index];
            //当前子结点的下一兄弟结点
            var nextSibiling = node.Children[index + 1];

            //增加父结点关键字到当前结点
            curNode.Keywords[curNode.KeywordCount] = node.Keywords[index];
            //父结点
            node.Keywords[index] = nextSibiling.Keywords[0];
            //兄弟结点的关键字向前移动一位
            for (var i = 1; i < nextSibiling.KeywordCount; i++)
            {
                nextSibiling.Keywords[i] = nextSibiling.Keywords[i + 1];
            }

            //如果是非叶结点，当前结点和兄弟结点的子结点都要调整
            if (!curNode.IsLeaf)
            {
                //增加前一兄弟结点的第一个子结点到当前结点
                curNode.Children[curNode.KeywordCount + 1] = nextSibiling.Children[0];

                for (var i = curNode.KeywordCount; i >= 0; i--)
                {
                    curNode.Children[i + 1] = curNode.Children[i];
                }
            }
            curNode.KeywordCount++;
            nextSibiling.KeywordCount--;
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
                    Keywords = new T[M * 2 - 1],
                    Children = new BTreeNode<T>[M * 2]
                };
                _root.Keywords[0] = keyword;
            }

            //2、根结点关键字数量等于最大数量，分裂为两个结点，中间关键字插入到父结点
            else if (_root.KeywordCount == 2 * M - 1)
            {
                var parent = new BTreeNode<T>
                {
                    Keywords = new T[M * 2 - 1],
                    Children = new BTreeNode<T>[M * 2],
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
                    if (node.Children[i + 1].KeywordCount == M * 2 - 1)
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
                Keywords = new T[M * 2 - 1],
                Children = new BTreeNode<T>[M * 2],
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
                for (var i = parent.KeywordCount; i >= position + 1; i--)
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
                return new BTreeSearchResult<T> { Node = node, KeywordPosition = i };
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