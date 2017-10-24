using System;
using System.Collections.Generic;


namespace Tree
{

    public class BinaryTree<T>
    {

        private BinaryTreeNode<T> _head;


        public void Add(T data)
        {
            var node = new BinaryTreeNode<T> {Data = data};
            if (_head == null)
            {
                _head = node;
                return;
            }

            //按层序添加新元素
            var queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_head);
            while (queue.Count > 0)
            {
                var curNode = queue.Dequeue();
                if (curNode.Left == null)
                {
                    curNode.Left = node;
                    return;
                }
                queue.Enqueue(curNode.Left);
                if (curNode.Rigth == null)
                {
                    curNode.Rigth = node;
                    return;
                }
                queue.Enqueue(curNode.Rigth);
            }
        }


        public bool Find(T data)
        {
            if (_head == null)
            {
                return false;
            }

            //按层序添加新元素
            var queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_head);
            while (queue.Count > 0)
            {
                var curNode = queue.Dequeue();
                if (curNode.Data.Equals(data))
                    return true;
                if (curNode.Left != null)
                {
                    queue.Enqueue(curNode.Left);
                }

                if (curNode.Rigth != null)
                {
                    queue.Enqueue(curNode.Rigth);
                }
            }
            return false;
        }


        public void Preorder(Action<T> action)
        {
            PreorderNode(_head, action);
        }


        private void PreorderNode(BinaryTreeNode<T> root, Action<T> action)
        {
            if (root != null)
            {
                action(root.Data);
                PreorderNode(root.Left, action);
                PreorderNode(root.Rigth, action);
            }
        }


        public void Inorder(Action<T> action)
        {
            InorderNode(_head, action);
        }


        private void InorderNode(BinaryTreeNode<T> root, Action<T> action)
        {
            if (root != null)
            {
                action(root.Data);
                InorderNode(root.Left, action);
                InorderNode(root.Rigth, action);
            }
        }


        public void Postorder(Action<T> action)
        {
            PostorderNode(_head, action);
        }


        private void PostorderNode(BinaryTreeNode<T> root, Action<T> action)
        {
            if (root != null)
            {
                action(root.Data);
                PostorderNode(root.Left, action);
                PostorderNode(root.Rigth, action);
            }
        }


        /// <summary>
        ///     层序遍历
        /// </summary>
        /// <param name="action"></param>
        public void Levelorder(Action<T> action)
        {
            if (_head == null)
            {
                return;
            }

            //按层序添加新元素
            var queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(_head);
            while (queue.Count > 0)
            {
                var curNode = queue.Dequeue();
                action(curNode.Data);
                if (curNode.Left != null)
                {
                    queue.Enqueue(curNode.Left);
                }

                if (curNode.Rigth != null)
                {
                    queue.Enqueue(curNode.Rigth);
                }
            }
        }

    }

    public class BinaryTreeNode<T>
    {

        public T Data { get; set; }

        public BinaryTreeNode<T> Left { get; set; }

        public BinaryTreeNode<T> Rigth { get; set; }

    }

}