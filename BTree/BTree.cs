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
        ///     1、关键字在非叶子结点，找到左子树中最小关键字（一直找到叶子结点为止），当前关键字替换为最小关键字，然后此最小关键字
        ///     2、关键字在叶子结点
        ///     2.1、结点的关键字数量是否小于M-1？不小于，直接删除，
        ///     2.1.1、不小于，直接删除
        ///     2.1.2、检查右兄弟结点关键字数量，大于M，删除关键字，父结点关键字移到当前位置，兄弟结点最小关键字移到父结点，
        ///     如果右兄弟不大于M，检查左兄弟，符合条件就执行相同动作
        ///     2.1.2.1、左右兄弟结点均不大于M，删除关键字，再将当前结点+父结点+右兄弟结点合并，
        ///     如父结点关键字数量小于M-1，则执行相同动作直到符合M阶
        /// </summary>
        /// <param name="keyword"></param>
        public void Remove(T keyword)
        {
            if (_root == null)
            {
                Console.WriteLine("The tree is empty\n");
                return;
            }

            // Call the remove function for root
            RemoveNode(_root, keyword);

            // If the root node has 0 keys, make its first child as the new root
            //  if it has a child, otherwise set root as NULL
            if (_root.KeywordsCount == 0)
            {
                var tmp = _root;
                if (_root.IsLeaf)
                    _root = null;
                else
                    _root = _root.Children[0];

                tmp = null;
            }
            return;

        }

        /// <summary>
        /// 关键字在结点中的索引位置,可能为非等于
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private int FindKey(BTreeNode<T> node, T keyword)
        {
            var i = 0;
            //判断关键字在数组中的位置
            //如大于等于则返回当前位置
            while (i < node.KeywordsCount && node.Keys[i].CompareTo(keyword) < 0)
                i++;
            return i;
        }

        private void RemoveNode(BTreeNode<T> node, T keyword)
        {
            var index = FindKey(node, keyword);
            //找到了关键字
            if (index < node.KeywordsCount && node.Keys[index].CompareTo(keyword) == 0)
            {
                if (node.IsLeaf)
                {
                    RemoveFromLeaf(node, index);
                }
                else
                {
                    RemoveFromNonLeaf(node, index);
                }
            }
            else
            {
                // If this node is a leaf node, then the key is not present in tree
                if (node.IsLeaf)
                {
                    Console.WriteLine("The key  is does not exist in the tree\n");
                    return;
                }

                // The key to be removed is present in the sub-tree rooted with this node
                // The flag indicates whether the key is present in the sub-tree rooted
                // with the last child of this node
                bool flag = ((index == node.KeywordsCount) ? true : false);

                // If the child where the key is supposed to exist has less that t keys,
                // we fill that child
                if (node.Children[index].KeywordsCount < M)
                    Fill(node, index);

                // If the last child has been merged, it must have merged with the previous
                // child and so we recurse on the (idx-1)th child. Else, we recurse on the
                // (idx)th child which now has atleast t keys
                if (flag && index > node.KeywordsCount)
                    RemoveNode(node.Children[index - 1], keyword);
                else
                    RemoveNode(node.Children[index], keyword);

            }
        }
        private void RemoveFromLeaf(BTreeNode<T> node, int index)
        {
            var i = index;
            while (i < node.KeywordsCount - 1)
            {
                node.Keys[i] = node.Keys[i + 1];
                i++;
            }
            node.KeywordsCount--;
        }

        private void RemoveFromNonLeaf(BTreeNode<T> node, int index)
        {
            if (node.Children[index].KeywordsCount >= M)
            {
                var max = GetMaxFromChildren(node, index);
                node.Keys[index] = max;
                RemoveNode(node.Children[index], max);
            }
            else if (node.Children[index + 1].KeywordsCount >= M)
            {
                var min = GetMinFromChildren(node, index);
                node.Keys[index] = min;
                RemoveNode(node.Children[index + 1], min);
            }
            else
            {
                Merge(node, index);
                var keyword = node.Keys[index];
                RemoveNode(node.Children[index], keyword);
            }
        }

        private T GetMaxFromChildren(BTreeNode<T> node, int index)
        {
            var child = node.Children[index];
            while (!child.IsLeaf)
                child = child.Children[child.KeywordsCount];
            return child.Keys[child.KeywordsCount - 1];
        }

        private T GetMinFromChildren(BTreeNode<T> node, int index)
        {
            var child = node.Children[index + 1];
            while (!child.IsLeaf)
                child = child.Children[0];
            return child.Keys[0];
        }
        private void Merge(BTreeNode<T> node, int index)
        {
            var leftChild = node.Children[index];
            var rightChild = node.Children[index + 1];
            leftChild.Keys[M - 1] = node.Keys[index];

            // Copying the keys from C[idx+1] to C[idx] at the end
            for (int i = 0; i < rightChild.KeywordsCount; ++i)
                leftChild.Keys[i + M] = rightChild.Keys[i];

            // Copying the child pointers from C[idx+1] to C[idx]
            if (!leftChild.IsLeaf)
            {
                for (int i = 0; i <= rightChild.KeywordsCount; ++i)
                    leftChild.Children[i + M] = rightChild.Children[i];
            }

            // Moving all keys after idx in the current node one step before -
            // to fill the gap created by moving keys[idx] to C[idx]
            for (int i = index + 1; i < node.KeywordsCount; ++i)
                node.Keys[i - 1] = node.Keys[i];

            // Moving the child pointers after (idx+1) in the current node one
            // step before
            for (int i = index + 2; i <= node.KeywordsCount; ++i)
                node.Children[i - 1] = node.Children[i];

            // Updating the key count of child and the current node
            leftChild.KeywordsCount += rightChild.KeywordsCount + 1;
            node.KeywordsCount--;

            // Freeing the memory occupied by sibling
            rightChild = null;
            return;
        }

        private void Fill(BTreeNode<T> node, int index)
        {

            // If the previous child(C[idx-1]) has more than t-1 keys, borrow a key
            // from that child
            if (index != 0 && node.Children[index - 1].KeywordsCount >= M)
                BorrowFromPrev(node, index);

            // If the next child(C[idx+1]) has more than t-1 keys, borrow a key
            // from that child
            else if (index != node.KeywordsCount && node.Children[index + 1].KeywordsCount >= M)
                BorrowFromNext(node, index);

            // Merge C[idx] with its sibling
            // If C[idx] is the last child, merge it with with its previous sibling
            // Otherwise merge it with its next sibling
            else
            {
                if (index != node.KeywordsCount)
                    Merge(node, index);
                else
                    Merge(node, index - 1);
            }
            return;
        }
        /// <summary>
        /// 从前一结点借关键字
        /// </summary>
        /// <param name="idx"></param>
        private void BorrowFromPrev(BTreeNode<T> node, int index)
        {

            var child = node.Children[index];
            var preChild = node.Children[index - 1];

            // The last key from C[idx-1] goes up to the parent and key[idx-1]
            // from parent is inserted as the first key in C[idx]. Thus, the  loses
            // sibling one key and child gains one key

            // Moving all key in C[idx] one step ahead
            for (var i = child.KeywordsCount - 1; i >= 0; i--)
            {
                child.Keys[i + 1] = child.Keys[i];
            }
            // If C[idx] is not a leaf, move all its child pointers one step ahead
            if (!child.IsLeaf)
            {
                for (var i = child.KeywordsCount; i >= 0; i--)
                {
                    child.Children[i + 1] = child.Children[i];
                }
            }
            child.Keys[0] = node.Keys[index - 1];
            // Setting child's first key equal to keys[idx-1] from the current node

            // Moving sibling's last child as C[idx]'s first child
            if (!node.IsLeaf)
                child.Children[0] = preChild.Children[preChild.KeywordsCount];

            // Moving the key from the sibling to the parent
            // This reduces the number of keys in the sibling
            node.Keys[index - 1] = preChild.Keys[preChild.KeywordsCount - 1];

            child.KeywordsCount++;
            preChild.KeywordsCount--;
            return;
        }

        // A function to borrow a key from the C[idx+1] and place
        // it in C[idx]

        private void BorrowFromNext(BTreeNode<T> node, int index)
        {

            var child = node.Children[index];
            var nextChild = node.Children[index + 1];

            // keys[idx] is inserted as the last key in C[idx]
            child.Keys[child.KeywordsCount] = node.Keys[index];

            // Sibling's first child is inserted as the last child
            // into C[idx]
            if (!child.IsLeaf)
                child.Children[child.KeywordsCount + 1] = nextChild.Children[0];

            //The first key from sibling is inserted into keys[idx]
            node.Keys[index] = nextChild.Keys[0];

            // Moving all keys in sibling one step behind
            for (var i = 1; i < nextChild.KeywordsCount; i++)
            {
                nextChild.Keys[i - 1] = nextChild.Keys[i];
            }

            // Moving the child pointers one step behind
            if (!nextChild.IsLeaf)
            {
                for (var i = 1; i <= nextChild.KeywordsCount; i++)
                {
                    nextChild.Children[i - 1] = nextChild.Children[i];
                }
            }

            // Increasing and decreasing the key count of C[idx] and C[idx+1]
            // respectively
            child.KeywordsCount++;
            nextChild.KeywordsCount--;
            return;
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
            //初始化根结点
            if (_root == null)
            {
                _root = new BTreeNode<T>
                {
                    Keys = new T[M * 2 - 1],
                    Children = new BTreeNode<T>[M * 2],
                    IsLeaf = true
                };
                _root.Keys[0] = keyword;
                _root.KeywordsCount = 1;
            }
            else
            {
                //结点已满,先分裂根结点，再插入关键字
                if (_root.KeywordsCount == 2 * M - 1)
                {
                    //新结点，分裂出来的结点将插入到新结点中
                    var parentNode = new BTreeNode<T>
                    {
                        Keys = new T[M * 2 - 1],
                        Children = new BTreeNode<T>[M * 2],
                        IsLeaf = false
                    };

                    //第一个子结点引用根结点
                    parentNode.Children[0] = _root;

                    //分裂根结点，分裂出来的关键字插入索引位置＝0
                    SplitNode(parentNode, 0, _root);

                    //分裂出来的关键字有两个子结点，判断应该插入哪个子结点，左或右？
                    var i = 0;
                    if (parentNode.Keys[0].CompareTo(keyword) < 0)
                        i++;

                    //关键字插入到子结点
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


        /// <summary>
        ///     插入关键字到非满结点
        ///     关键字必须插入到叶结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyword"></param>
        private void InsertNonFull(BTreeNode<T> node, T keyword)
        {
            //是否为叶结点
            //直接插入关键字到结点
            if (node.IsLeaf)
            {
                var i = node.KeywordsCount - 1;
                while (i >= 0 && node.Keys[i].CompareTo(keyword) > 0)
                {
                    node.Keys[i + 1] = node.Keys[i];
                    i--;
                }
                node.Keys[i + 1] = keyword;
                node.KeywordsCount++;
            }
            else
            {
                //先找出关键字在关键字数组中的位置
                var i = node.KeywordsCount - 1;
                while (i >= 0 && node.Keys[i].CompareTo(keyword) > 0)
                    i--;

                //子结点关键字数组是否已满
                if (node.Children[i + 1].KeywordsCount == 2 * M - 1)
                {
                    //先分裂子结点
                    SplitNode(node, i + 1, node.Children[i + 1]);

                    //分裂出来的结点有左右两个子结点，判断应该插入哪个结点，左或右？
                    if (node.Keys[i + 1].CompareTo(keyword) < 0)
                        i++;
                }

                //插入关键字到子结点
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
                KeywordsCount = M - 1
            };

            //复制左部到新结点
            /*
             如M阶为4，关键字数组满的话，如下：
             0 1 2 M 4 5 6
             M-1＝3正好在中间位置
             */
            for (var i = 0; i < M - 1; i++)
            {
                leftNode.Keys[i] = node.Keys[M + i];
            }
            if (!node.IsLeaf)
            {
                //非叶子结点，复制子结点到新结点
                /*
                 如M阶为4，关键字数组满的话，如下：
                 关键字：0   1   2   M   4   5   6
                 子结点：P0  P1  P2  PM  P4  P5  P6   P7
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

            //调整结点现有数量
            node.KeywordsCount = M - 1;

            //调整父结点
            /*
             父结点原关键字和子结点如下：
                关键字：0   1   2   
                子结点：P0  P1  P2  PM  

             */
            //父结点的关键字向后移动
            for (var i = parentNode.KeywordsCount - 1; i >= position; i--)
            {
                parentNode.Keys[i + 1] = parentNode.Keys[i];
            }

            //父结点的子树向后移动
            for (var i = parentNode.KeywordsCount; i >= position + 1; i--)
            {
                parentNode.Children[i + 1] = parentNode.Children[i];
            }
            /*
             父结点现在关键字和子结点如下：
                关键字：0   1   position  2   
                子结点：P0  P1  P2  position+1 PM  
                position为待记录位置
             */

            //记录中间关键字和子结点 M-1为中间索引
            parentNode.Keys[position] = node.Keys[M - 1];
            parentNode.Children[position + 1] = leftNode;
            /*
             父结点现在关键字和子结点如下：
                关键字：0   1   中间位置关键字  2   
                子结点：P0  P1  P2  leftNode  PM  
                其中leftNode中的关键字都大于中间位置关键字
             */

            //父结点关键字数量递增
            parentNode.KeywordsCount++;
        }

        #endregion


        #region 查找

        /// <summary>
        ///     查找关键所在结点
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public BTreeSearchResult<T> Search(T keyword)
        {
            return SearchNode(_root, keyword);
        }


        private BTreeSearchResult<T> SearchNode(BTreeNode<T> node, T keyword)
        {
            var i = 0;
            while (i < node.KeywordsCount && node.Keys[i].CompareTo(keyword) < 0) i++;
            if (node.Keys[i].CompareTo(keyword) == 0) return new BTreeSearchResult<T> { Node = node, KeywordPosition = i };
            return node.IsLeaf ? null : SearchNode(node.Children[i], keyword);
        }


        ///// <summary>
        /////     在指定结点关键字的左孩子中查找最小关键字结点
        ///// </summary>
        ///// <param name="node"></param>
        ///// <returns></returns>
        //public BTreeSearchResult<T> FindMinKeywordNodeInLeftChildren(BTreeNode<T> node)
        //{
        //    while (true)
        //    {
        //        if (node == null)
        //            throw new Exception("指定的结点为空！");

        //        if (node.KeywordsCount < 0)
        //            throw new Exception("指定的关键字位置不正确！");

        //        if (node.IsLeaf) return new BTreeSearchResult<T> {Node = node, KeywordPosition = 0};
        //        node = node.Children[0];
        //    }
        //}

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
            for (; i < node.KeywordsCount; i++)
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