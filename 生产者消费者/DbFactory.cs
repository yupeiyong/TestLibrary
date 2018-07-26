using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 生产者消费者
{
    public class DbFactory
    {
        private Queue<Users> _queue;
        public DbFactory(int maxCount)
        {
            _queue = new Queue<Users>();
            this._maxCount = maxCount;
        }

        private int _maxCount;

        public bool Completed;
        public void Push(List<Users> users)
        {
            lock (this)
            {
                while (_queue.Count >= _maxCount)
                {
                    Monitor.Wait(this);
                }
                if (users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        _queue.Enqueue(user);
                    }

                }
                else
                {
                    Completed = true;
                }
                Monitor.PulseAll(this);
            }            
        }

        public Users Pop()
        {
            lock (this)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(this);
                }
                Users user = null;
                if (!Completed)
                {
                    user = _queue.Dequeue();
                }
                Monitor.PulseAll(this);
                return user;
            }           
        }
    }
}
