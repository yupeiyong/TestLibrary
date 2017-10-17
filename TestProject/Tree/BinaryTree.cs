using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Tree
{
    /// <summary>
    /// 二叉树
    /// </summary>
    public class BinaryTree<T>
    {
        private BinaryNode<T> _root;

        public  int Count { get; private set; }
        public void Add(T data)
        {
            if (_root == null)
            {
                _root = new BinaryNode<T> { Data = data };
                Count++;
                return;
            }
            var queue = new Queue<BinaryNode<T>>();
            queue.Enqueue(_root);
            while (queue.Count > 0)
            {
                var curNode = queue.Dequeue();
                var left = curNode.Left;
                if (left == null)
                {
                    curNode.Left = new BinaryNode<T> { Data = data };
                    Count++;
                    return;
                }
                queue.Enqueue(left);
                var right = curNode.Right;
                if (right == null)
                {
                    curNode.Right = new BinaryNode<T> { Data = data };
                    Count++;
                    return;
                }
                queue.Enqueue(right);
            }
        }


        public void PreorderTraversal(Action<T> action)
        {
            PreorderTraversalNode(_root, action);
        }


        private void PreorderTraversalNode(BinaryNode<T> node, Action<T> action)
        {
            if (action == null)
                throw new Exception("委托不能为空！");
            if (node == null) return;
            action(node.Data);
            PreorderTraversalNode(node.Left, action);
            PreorderTraversalNode(node.Right, action);
        }

        public void InorderTraversal(Action<T> action)
        {
            InorderTraversalNode(_root, action);
        }
        private void InorderTraversalNode(BinaryNode<T> node, Action<T> action)
        {
            if (action == null)
                throw new Exception("委托不能为空！");
            if (node == null) return;
            action(node.Data);
            InorderTraversalNode(node.Left, action);
            InorderTraversalNode(node.Right, action);
        }


        public void PostorderTraversal(Action<T> action)
        {
            PostorderTraversalNode(_root, action);
        }

        private void PostorderTraversalNode(BinaryNode<T> node, Action<T> action)
        {
            if (action == null)
                throw new Exception("委托不能为空！");
            if (node == null) return;
            action(node.Data);
            PostorderTraversalNode(node.Left, action);
            PostorderTraversalNode(node.Right, action);
        }

    }

    public class BinaryNode<T>
    {

        public T Data { get; set; }

        public BinaryNode<T> Left { get; set; }

        public BinaryNode<T> Right { get; set; }

    }
}
