using System;


namespace Stack
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Stack<T>
    {
        private T[] _datas;

        public int Count { get; private set; }

        private int _size;


        public Stack()
        {
            _size = 8;
            _datas=new T[_size];
        }
        public void Push(T data)
        {
            if (Count == _size)
            {
                _size = _size * 2;
                var tmp=new T[_size];
                for (int i = 0, maxCount = _datas.Length; i < maxCount; i++)
                {
                    tmp[i] = _datas[i];
                }
                _datas = tmp;
                tmp = null;
            }
            _datas[Count++] = data;
        }


        public T Pop()
        {
            if(IsEmpty())
                throw new Exception("栈为空！");

            var index = Count - 1;
            Count--;
            return _datas[index];
        }


        public bool IsEmpty()
        {
            return Count == 0;
        }
        public T GetTop()
        {
            if (IsEmpty())
                throw new Exception("栈为空！");

            var index = Count - 1;
            return _datas[index];
        }
    }

    public class LinkStack<T>
    {

        private class StackNode<TNode>
        {
            public TNode Data { get; set; }
            public StackNode<TNode> Next { get; set; }
        }

        public int Count { get; private set; }

        private StackNode<T> _head = null;
        public LinkStack()
        {
            _head = new StackNode<T>
            {
                Data = default(T),
                Next = null
            };
        }
        public void Push(T data)
        {
            if (_head == null)
            {
                _head = new StackNode<T> {Data = data, Next = null};
            }
            else
            {
                var node= new StackNode<T> { Data = data, Next = null };
                var parent = _head;
                var curNode = _head.Next;
                while (curNode != null)
                {
                    parent = curNode;
                    curNode = curNode.Next;
                }
                curNode = node;
                parent.Next = curNode;
            }
            Count++;
        }


        public T Pop()
        {
            if (IsEmpty())
                throw new Exception("栈为空！");

            if (IsEmpty())
                throw new Exception("栈为空！");

            var parentNode = _head;
            var curNode = _head.Next;
            while (curNode.Next != null)
            {
                parentNode = curNode;
                curNode = curNode.Next;
            }
            var data = curNode.Data;
            parentNode.Next = null;
            Count--;
            return data;
        }


        public bool IsEmpty()
        {
            return Count == 0 || _head.Next==null;
        }
        public T GetTop()
        {
            if (IsEmpty())
                throw new Exception("栈为空！");

            var curNode = _head.Next;
            var parentNode = _head;
            while (curNode.Next != null)
            {
                parentNode = curNode;
                curNode = curNode.Next;
            }
            var data = curNode.Data;
            return data;
        }
    }
}