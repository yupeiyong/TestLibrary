using System;


namespace Queue
{

    /// <summary>
    ///     链式队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkQueue<T>
    {

        /// <summary>
        ///     尾部
        /// </summary>
        private Node<T> _tail;

        /// <summary>
        ///     头部
        /// </summary>
        private Node<T> _top;

        public int Count { get; private set; }


        /// <summary>
        ///     从 Queue 中移除所有的元素。
        /// </summary>
        public void Clear()
        {
            while (!IsEmpty())
            {
                Dequeue();
            }
        }


        /// <summary>
        ///     判断某个元素是否在 Queue 中。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Contains(T data)
        {
            if (IsEmpty())
                throw new Exception("队列为空！");

            var curNode = _top;
            while (curNode.Next != null)
            {
                if (curNode.Data.Equals(data))
                {
                    return true;
                }
                curNode = curNode.Next;
            }
            return false;
        }


        /// <summary>
        ///     移除并返回在 Queue 的开头的对象。
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (IsEmpty())
                throw new Exception("队列为空！");

            var data = _top.Data;
            _top = _top.Next;
            Count--;
            return data;
        }


        public bool IsEmpty()
        {
            return Count == 0 || _top == null;
        }


        /// <summary>
        ///     向 Queue 的末尾添加一个对象。
        /// </summary>
        /// <param name="data"></param>
        public void Enqueue(T data)
        {
            var node = new Node<T> {Data = data};
            Count++;
            if (_tail == null)
            {
                _tail = node;
                _top = node;
                return;
            }
            _tail.Next = node;
            _tail = _tail.Next;
        }


        private class Node<TNode>
        {

            public TNode Data { get; set; }

            public Node<TNode> Next { get; set; }

        }

    }

}