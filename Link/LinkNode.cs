using System;
using System.Collections;
using System.Collections.Generic;


namespace Link
{

    public class LinkNode<T>
    {

        public LinkNode()
        {
            
        } 
        public LinkNode(T data)
        {
            Data = data;
        }


        public LinkNode(T data, LinkNode<T> next)
        {
            Data = data;
            Next = next;
        }


        public T Data { get; set; }

        public LinkNode<T> Next { get; set; }

    }

    public class LinkList<T>:IEnumerable<T>
    {
        private LinkNode<T> _head;

        public int Count { get; private set; }
        public void Add(T data)
        {
            var node=new LinkNode<T>(data);
            if (_head == null)
            {
                _head = node;
            }
            else
            {
                var p = _head;
                while (p.Next != null)
                {
                    p = p.Next;
                }
                p.Next = node;
            }
            Count++;
        }

        /// <summary>
        /// 插入节点
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">从0开始的索引位置插入数据，原数据向后移</param>
        public void Insert(T data, int index)
        {
            if(index>Count-1||index<0)
                throw new Exception("提供了错误的索引位置！");

            if(IsEmpty())
                throw new Exception("链表为空！");

            var node= new LinkNode<T>(data);
            if (index == 0)
            {
                node.Next = _head;
                _head = node;
                return;
            }
            var curIndex = 0;
            var curNode = _head;
            while (curNode.Next != null)
            {
                var parent = curNode;
                curNode = curNode.Next;
                curIndex++;
                if (curIndex != index) continue;
                node.Next = curNode;
                parent.Next = node;
                return;
            }
            
        }


        public bool IsEmpty()
        {
            return Count == 0 || _head == null;
        }


        public void Remove(T data)
        {
            if (IsEmpty())
                throw new Exception("链表为空！");

            if (_head.Data.Equals(data))
            {
                _head = _head.Next;
                Count--;
                return;
            }
            var curNode = _head;
            var parent = _head;
            while (curNode.Next != null)
            {
                if (curNode.Data.Equals(data))
                {
                    parent.Next = curNode.Next;
                    return;
                }
                parent = curNode;
                curNode = curNode.Next;
            }

        }


        public LinkNode<T> Find(T data)
        {
            if (IsEmpty())
                throw new Exception("链表为空！");
            var p = _head;
            while (p != null)
            {
                if (p.Data.Equals(data))
                    return p;
                p = p.Next;
            }
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var curNode = _head;
            while (curNode != null)
            {
                yield return curNode.Data;
                curNode = curNode.Next;
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}