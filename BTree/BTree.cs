using System;
using System.Collections.Generic;


namespace BTree
{

    /// <summary>
    ///     B树是一种为辅助存储设计的一种数据结构，在1970年由R.Bayer和E.mccreight提出。
    ///     在文件系统和数据库中为了减少IO操作大量被应用。遗憾的是，他们并没有说明为什么取名为B树，
    ///     但按照B树的性质来说B通常被解释为Balance。在国内通常有说是B-树，其实并不存在B-树，只是由英文B-Tree直译成了B-树。
    ///     用阶定义的B树
    ///     B 树又叫平衡多路查找树。一棵m阶的B 树(注：切勿简单的认为一棵m阶的B树是m叉树，虽然存在四叉树，八叉树，KD树，及vp/R树/R*
    ///     树/R+树/X树/M树/线段树/希尔伯特R树/优先R树等空间划分树，但与B树完全不等同)的特性如下：
    ///     树中每个结点最多含有m个孩子（m>=2）；
    ///     除根结点和叶子结点外，其它每个结点至少有[ceil(m / 2)] 个孩子（其中ceil(x)是一个取上限的函数）；
    ///     若根结点不是叶子结点，则至少有2个孩子（特殊情况：没有孩子的根结点，即根结点为叶子结点，整棵树只有一个根节点）；
    ///     所有叶子结点都出现在同一层，叶子结点不包含任何关键字信息(可以看做是外部接点或查询失败的接点，实际上这些结点不存在，指向这些结点的指针都为null)；
    ///     （读者反馈     @冷岳：这里有错，叶子节点只是没有孩子和指向孩子的指针，这些节点也存在，也有元素。
    ///     @研究者July：其实，关键是把什么当做叶子结点，因为如红黑树中，每一个NULL指针即当做叶子结点，只是没画出来而已）。
    ///     每个非终端结点中包含有n个关键字信息： (n，P0，K1，P1，K2，P2，......，Kn，Pn)。
    ///     其中：
    ///     a)   Ki (i=1...n)为关键字，且关键字按顺序升序排序K(i-1)
    ///     < Ki。
    ///         b) Pi为指向子树根的接点， 且指针P( i-1) 指向子树种所有结点的关键字均小于Ki， 但都大于K( i-1)。
    ///         c) 关键字的个数n必须满足： [ ceil( m / 2)-1]<= n <= m-1。如下图所示：
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BTree<T> where T : IComparable<T>
    {

        /// <summary>
        ///     每个节点关键字数量
        /// </summary>
        private const int PerNodeCount = 4;

        /// <summary>
        ///     根节点
        /// </summary>
        private TreeNode<T> _root;


        public BTree()
        {
            _root = new TreeNode<T>
            {
                ElementCount = 0,
                IsLeaf = true
            };
        }


        //B树中的节点分裂
        private static void SplitNode(TreeNode<T> fatherNode, int position, TreeNode<T> nodeToBeSplit)
        {
            //左元素数量
            var leftElementCount = PerNodeCount / 2 ;
            //右元素数量
            var rightElementCount = PerNodeCount - leftElementCount;

            //创建新节点，容纳分裂后被移动的元素
            //新节点的层级和原节点位于同一层
            var rightNode = new TreeNode<T>
            {
                IsLeaf = nodeToBeSplit.IsLeaf,
                ElementCount = rightElementCount
            };
            
            //新节点元素的个数大约为分裂节点的一半
            for (var i = leftElementCount; i < nodeToBeSplit.ElementCount; i++)
            {
                //将原页中后半部分复制到新页中
                rightNode.Elements.Add(nodeToBeSplit.Elements[i]);
            }

            //如果不是叶子节点，将指针也复制过去
            if (!nodeToBeSplit.IsLeaf)
            {
                for (var j = leftElementCount; j < nodeToBeSplit.Pointers.Count; j++)
                {
                    rightNode.Pointers.Add(nodeToBeSplit.Pointers[j]);
                }
            }

            //原节点剩余元素个数
            nodeToBeSplit.ElementCount = leftElementCount;

            //将父节点指向子节点的指针向后推一位
            for (var k = fatherNode.ElementCount + 1; k > position + 1; k--)
            {
                fatherNode.Pointers[k] = fatherNode.Pointers[k - 1];
            }

            //将父节点的元素向后推一位
            for (var k = fatherNode.ElementCount; k > position + 1; k--)
            {
                fatherNode.Elements[k] = fatherNode.Elements[k - 1];
            }

            //将被分裂的页的中间节点插入父节点
            fatherNode.Elements.Insert(position - 1, nodeToBeSplit.Elements[PerNodeCount / 2]);

            //父节点元素大小+1
            fatherNode.ElementCount += 1;

            //将FatherNode,NodeToBeSplit,newNode写回磁盘,三次IO写操作
        }

        /// <summary>
        /// 插入关键字
        /// 1、根节点为叶节点，未满时，插入到根节点。满则分裂根节点
        /// 2、根节点不为叶节点，向下查找
        /// </summary>
        /// <param name="keyWord"></param>
        public void Insert(T keyWord)
        {
            //如果根节点满了，则对跟节点进行分裂
            if (_root.ElementCount == PerNodeCount)
            {
                //中间索引位置
                var middleIndex = _root.ElementCount / 2 - 1;

                //新节点，将升级为根节点
                var newRoot = new TreeNode<T>
                {
                    ElementCount = 0,
                    IsLeaf = false
                };

                //将newRoot节点变为根节点
                SplitNode(newRoot, middleIndex, _root);

                //分裂后插入新根的树
                BTreeInsertNotFull(newRoot, keyWord);

                //将树的根进行变换
                _root = newRoot;
            }
            else
            {
                //如果根节点没有满，直接插入
                BTreeInsertNotFull(_root, keyWord);
            }
        }


        //在节点非满时寻找插入节点
        private void BTreeInsertNotFull(TreeNode<T> node, T keyWord)
        {
            var i = node.ElementCount;

            //如果是叶子节点，则寻找合适的位置直接插入
            if (node.IsLeaf)
            {
                while (i >= 1 && keyWord.CompareTo(node.Elements[i - 1]) < 0)
                {
                    //所有的元素后推一位
                    node.Elements[i] = node.Elements[i - 1];
                    i -= 1;
                }

                //将节点写入磁盘，IO写+1
                //将关键字插入节点
                node.Elements.Insert(i, keyWord);
                node.ElementCount += 1;
            }

            //如果是非叶子节点
            else
            {
                while (i >= 1 && keyWord.CompareTo(node.Elements[i - 1]) < 0)
                {
                    i -= 1;
                }

                //这步将指针所指向的节点读入内存,IO读+1
                if (node.Pointers[i].ElementCount == PerNodeCount)
                {
                    //如果子节点已满，进行节点分裂
                    SplitNode(node, i, node.Pointers[i]);
                }
                if (keyWord.CompareTo(node.Elements[i - 1]) > 0)
                {
                    //根据关键字的值决定插入分裂后的左孩子还是右孩子
                    i += 1;
                }

                //迭代找叶子，找到叶子节点后插入
                BTreeInsertNotFull(node.Pointers[i], keyWord);
            }
        }


        ///// <summary>
        ///// 按升序插入关键字到节点
        ///// </summary>
        ///// <param name="node"></param>
        ///// <param name="keyword"></param>
        ///// <param name="middleIndex"></param>
        //private void AscInsertNode(TreeNode<T> node, T keyword,int middleIndex)
        //{
        //    if(middleIndex<0||middleIndex>node.ElementNum)
        //        throw new Exception("错误，中间索引位置不正确！必须大于等于0和小于等于节点数。");
        //    var middleItem = node.Elements[middleIndex];
        //    var compareResult = keyword.CompareTo(middleItem);
        //    if (compareResult < 0)
        //    {

        //    }else if (compareResult > 0)
        //    {

        //    }
        //    else
        //    {

        //    }
        //    var i = node.ElementNum;

        //    //如果是叶子节点，则寻找合适的位置直接插入
        //    if (node.IsLeaf)
        //    {
        //        while (i >= 1 && keyWord.CompareTo(node.Elements[i - 1]) < 0)
        //        {
        //            //所有的元素后推一位
        //            node.Elements[i] = node.Elements[i - 1];
        //            i -= 1;
        //        }

        //        //将节点写入磁盘，IO写+1
        //        //将关键字插入节点
        //        node.Elements.Insert(i, keyWord);
        //        node.ElementNum += 1;
        //    }

        //}
        public ReturnValue<T> Search(T keyword)
        {
            return BTreeSearch(_root, keyword);
        }


        //从B树中搜索节点，存在则返回节点和元素在节点的值，否则返回NULL
        private ReturnValue<T> BTreeSearch(TreeNode<T> rootNode, T keyword)
        {
            var i = 1;

            while (i <= rootNode.ElementCount && keyword.CompareTo(rootNode.Elements[i - 1]) > 0)
            {
                i = i + 1;
            }
            if (i <= rootNode.ElementCount && keyword.CompareTo(rootNode.Elements[i - 1]) == 0)
            {
                var r = new ReturnValue<T>
                {
                    Node = rootNode.Pointers[i],
                    Position = i
                };
                return r;
            }
            if (rootNode.IsLeaf)
            {
                return null;
            }

            //从磁盘将内容读出来,做一次IO读
            return BTreeSearch(rootNode.Pointers[i], keyword);
        }

    }

    /// <summary>
    ///     查询返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReturnValue<T> where T : IComparable<T>
    {

        public TreeNode<T> Node;
        public int Position;

    }

}