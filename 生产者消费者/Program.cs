using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 生产者消费者
{
    public class MonitorSample
    {
        public static void Main2(String[] args)
        {
            int result = 0; //一个标志位，如果是0表示程序没有出错，如果是1表明有错误发生
            Factory factory = new Factory();

            //下面使用cell初始化CellProd和CellCons两个类，生产和消费次数均为20次
            var prod = new Producer(factory, 20);
            var cons = new Consumer(factory, 20);

            Thread producer = new Thread(new ThreadStart(prod.Produce));
            Thread consumer = new Thread(new ThreadStart(cons.Consume));
            //生产者线程和消费者线程都已经被创建，但是没有开始执行 
            try
            {
                producer.Start();
                consumer.Start();

                producer.Join();
                consumer.Join();
                Console.ReadLine();
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

    //public class Cell
    //{
    //    int cellContents; // Cell对象里边的内容
    //    bool readerFlag = false; // 状态标志，为true时可以读取，为false则正在写入
    //    public int ReadFromCell()
    //    {
    //        lock (this) // Lock关键字保证了什么，请大家看前面对lock的介绍
    //        {
    //            if (!readerFlag)//如果现在不可读取
    //            {
    //                try
    //                {
    //                    //等待WriteToCell方法中调用Monitor.Pulse()方法
    //                    Monitor.Wait(this);
    //                }
    //                catch (SynchronizationLockException e)
    //                {
    //                    Console.WriteLine(e);
    //                }
    //                catch (ThreadInterruptedException e)
    //                {
    //                    Console.WriteLine(e);
    //                }
    //            }
    //            Console.WriteLine("Consume: {0}", cellContents);
    //            readerFlag = false;
    //            //重置readerFlag标志，表示消费行为已经完成
    //            Monitor.Pulse(this);
    //            //通知WriteToCell()方法（该方法在另外一个线程中执行，等待中）
    //        }
    //        return cellContents;
    //    }

    //    public void WriteToCell(int n)
    //    {
    //        lock (this)
    //        {
    //            if (readerFlag)
    //            {
    //                try
    //                {
    //                    Monitor.Wait(this);
    //                }
    //                catch (SynchronizationLockException e)
    //                {
    //                    //当同步方法（指Monitor类除Enter之外的方法）在非同步的代码区被调用
    //                    Console.WriteLine(e);
    //                }
    //                catch (ThreadInterruptedException e)
    //                {
    //                    //当线程在等待状态的时候中止 
    //                    Console.WriteLine(e);
    //                }
    //            }
    //            cellContents = n;
    //            Console.WriteLine("Produce: {0}", cellContents);
    //            readerFlag = true;
    //            Monitor.Pulse(this);
    //            //通知另外一个线程中正在等待的ReadFromCell()方法
    //        }
    //    }
    //}
    ////下面定义生产者类 CellProd 和消费者类 CellCons ，它们都只有一个方法ThreadRun()，以便在Main()函数中提供给线程的ThreadStart代理对象，作为线程的入口。
    //public class CellProd
    //{
    //    Cell cell; // 被操作的Cell对象
    //    int quantity = 1; // 生产者生产次数，初始化为1 

    //    public CellProd(Cell box, int request)
    //    {
    //        //构造函数
    //        cell = box;
    //        quantity = request;
    //    }
    //    public void ThreadRun()
    //    {
    //        for (int looper = 1; looper <= quantity; looper++)
    //            cell.WriteToCell(looper); //生产者向操作对象写入信息
    //    }
    //}

    //public class CellCons
    //{
    //    Cell cell;
    //    int quantity = 1;

    //    public CellCons(Cell box, int request)
    //    {
    //        //构造函数
    //        cell = box;
    //        quantity = request;
    //    }
    //    public void ThreadRun()
    //    {
    //        int valReturned;
    //        for (int looper = 1; looper <= quantity; looper++)
    //            valReturned = cell.ReadFromCell();//消费者从操作对象中读取信息
    //    }
    //}


    public class Factory
    {
        private bool IsRead;
        private int content;
        public void Write(int n)
        {
            lock (this)
            {
                if (IsRead)
                {
                    Monitor.Wait(this);
                }
                content = n;
                Console.WriteLine("生产: {0}", n);
                //已经写完
                IsRead = true;
                Monitor.Pulse(this);


            }
        }

        public int Read()
        {
            lock (this)
            {
                if (!IsRead)
                {
                    Monitor.Wait(this);
                }
                Console.WriteLine("消费: {0}", content);
                //已经读完
                IsRead = false;
                Monitor.Pulse(this);
            }
            return content;
        }
    }

    public class Producer
    {
        private Factory factory;
        private int request;
        public Producer(Factory factory, int requestCount)
        {
            this.factory = factory;
            this.request = requestCount;
        }

        public void Produce()
        {
            for (int looper = 1; looper <= request; looper++)
                factory.Write(looper); //生产者向操作对象写入信息
        }
    }

    public class Consumer
    {
        private Factory factory;
        private int request;
        public Consumer(Factory factory, int requestCount)
        {
            this.factory = factory;
            this.request = requestCount;
        }

        public void Consume()
        {
            for (int looper = 1; looper <= request; looper++)
                factory.Read();
        }

    }
}
