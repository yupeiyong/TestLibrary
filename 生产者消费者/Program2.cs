using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 生产者消费者1
{
    class Program2
    {
        public static void Main(String[] args)
        {
            int result = 0; //一个标志位，如果是0表示程序没有出错，如果是1表明有错误发生
            Factory factory = new Factory();

            //下面使用cell初始化CellProd和CellCons两个类，生产和消费次数均为20次
            var prod = new Producer(factory,1);
            var cons = new Consumer(factory);

            var prod2 = new Producer(factory,21);
            var cons2 = new Consumer(factory);


            var prod3 = new Producer(factory,41);
            var cons3 = new Consumer(factory);

            var prod4 = new Producer(factory,61);
            var cons4 = new Consumer(factory);

            Thread producer1 = new Thread(new ThreadStart(prod.Run));
            Thread consumer1 = new Thread(new ThreadStart(cons.Run));

            Thread producer2 = new Thread(new ThreadStart(prod2.Run));
            Thread consumer2 = new Thread(new ThreadStart(cons2.Run));

            Thread producer3 = new Thread(new ThreadStart(prod3.Run));
            Thread consumer3 = new Thread(new ThreadStart(cons3.Run));

            Thread producer4 = new Thread(new ThreadStart(prod4.Run));
            Thread consumer4 = new Thread(new ThreadStart(cons4.Run));
            //生产者线程和消费者线程都已经被创建，但是没有开始执行 
            try
            {
                producer1.Start();
                consumer1.Start();

                producer2.Start();
                consumer2.Start();

                producer3.Start();
                consumer3.Start();

                producer4.Start();
                consumer4.Start();

                producer1.Join();
                consumer1.Join();

                producer2.Join();
                consumer2.Join();

                producer3.Join();
                consumer3.Join();

                producer4.Join();
                consumer4.Join();

                Console.WriteLine($"结束！");
                Console.ReadKey();
            }
            catch (ThreadStateException e)
            {
                //当线程因为所处状态的原因而不能执行被请求的操作
                Console.WriteLine(e);
                result = 1;
            }
            catch (ThreadInterruptedException e)
            {
                //当线程在等待状态的时候中止
                Console.WriteLine(e);
                result = 1;
            }
            //尽管Main()函数没有返回值，但下面这条语句可以向父进程返回执行结果
            Environment.ExitCode = result;
        }
    }

    class Factory
    {
        private readonly int[] _list = new int[10];

        private int _count = 0;

        public void Push(int value)
        {
            lock (this)
            {
                while (_count >= _list.Length)
                {
                    Monitor.Wait(this);
                }

                _list[_count] = value;
                _count++;
                Monitor.PulseAll(this);
            }
        }


        public int Pop()
        {
            lock (this)
            {
                while (_count == 0)
                {
                    Monitor.Wait(this);
                }

                var value = _list[_count - 1];
                _count--;
                Monitor.PulseAll(this);
                return value;
            }

        }


    }
    class Producer
    {
        private Factory factory;
        private int start;
        public Producer(Factory factory,int start)
        {
            this.factory = factory;
            this.start = start;
        }

        public void Run()
        {
            for (var i = start; i < start+20; i++)
            {
                factory.Push(i);
                var id = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"生产{id}：{i}");
            }
        }
    }

    class Consumer
    {
        private Factory factory;
        public Consumer(Factory factory)
        {
            this.factory = factory;
        }


        public void Run()
        {
            for (var i = 0; i < 20; i++)
            {
                var value=factory.Pop();
                var id=Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"消费者{id}：{value}");
            }
        }
    }
}
