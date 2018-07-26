using System.Collections.Generic;
using System.Threading;


namespace 生产者消费者
{

    public class DbFactory
    {

        private int _maxCount;
        private Queue<Users> _queue;

        public bool Completed;


        public DbFactory(int maxCount)
        {
            _queue = new Queue<Users>();
            _maxCount = maxCount;
        }


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
                    if (!Completed)
                    {
                        Monitor.Wait(this);
                    }
                    else
                    {
                        break;
                    }
                }
                Users user = null;
                if (_queue.Count > 0)
                {
                    user = _queue.Dequeue();
                }
                Monitor.PulseAll(this);
                return user;
            }
        }

    }

}