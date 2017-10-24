using System;


namespace Stack
{
    /// <summary>
    /// 泛型栈
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
            _datas = new T[_size];
        }
        public void Push(T data)
        {
            if (Count == _size)
            {
                _size = _size * 2;
                var tmp = new T[_size];
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
            if (IsEmpty())
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

        private StackNode<T> _top = null;
        public LinkStack()
        {

        }
        public void Push(T data)
        {
            var node = new StackNode<T> { Data = data, Next = null };
            if (_top == null)
            {
                _top = node;
            }
            else
            {
                node.Next = _top;
                _top = node;
            }
            Count++;
        }


        public T Pop()
        {
            if (IsEmpty())
                throw new Exception("栈为空！");

            var data = _top.Data;
            _top = _top.Next;
            Count--;
            return data;
        }


        public bool IsEmpty()
        {
            return Count == 0 || _top == null;
        }
        public T GetTop()
        {
            if (IsEmpty())
                throw new Exception("栈为空！");

            return _top.Data;
        }
    }
}