using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;

namespace TestProject.ProducerConsumer
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Single_Buffer()
        {
            var buffer = new SingleBuffer();
            var producer = new Thread(() =>
            {
                for (var i = 0; i < 16; i++)
                {
                    buffer.Produce(i);
                    Console.WriteLine($"生产产品：{i}");
                    Thread.Sleep(1000);
                }
                Console.WriteLine($"生产完成");
            });

            producer.Start();

            var consumer = new Thread(() =>
            {
                for (var i = 0; i < 16; i++)
                {
                    var value = buffer.Consume();
                    Console.WriteLine($"消费产品：{value}");
                    Thread.Sleep(2000);
                }
                Console.WriteLine($"消费完成");
            });

            consumer.Start();

            producer.Join();
            consumer.Join();

        }

        /// <summary>
        /// 测试s1=0的情况，初始可用缓冲区=0 软件工程师教材 图3-7
        /// </summary>
        [TestMethod]
        public void Test_Single_Buffer2()
        {
            var buffer = new SingleBuffer2();
            var producer = new Thread(() =>
            {
                for (var i = 0; i < 16; i++)
                {
                    buffer.Produce(i);
                    Console.WriteLine($"生产产品：{i}");
                    Thread.Sleep(1000);
                }
                Console.WriteLine($"生产完成");
            });

            producer.Start();

            var consumer = new Thread(() =>
            {
                for (var i = 0; i < 16; i++)
                {
                    var value = buffer.Consume();
                    Console.WriteLine($"消费产品：{value}");
                    Thread.Sleep(2000);
                }
                Console.WriteLine($"消费完成");
            });

            consumer.Start();

            producer.Join();
            consumer.Join();

        }


        /// <summary>
        /// 测试多缓冲区的情况
        /// </summary>
        [TestMethod]
        public static void Test_Multi_Buffer()
        {
            var buffer = new MultiBuffer(6);
            var producer = new Thread(() =>
            {
                for (var i = 0; i < 20; i++)
                {
                    buffer.Produce(i);
                    Console.WriteLine($"生产产品：{i}");
                    Thread.Sleep(500);
                }
                Console.WriteLine($"生产完成");
            });

            producer.Start();

            var consumer = new Thread(() =>
            {
                for (var i = 0; i < 20; i++)
                {
                    var value = buffer.Consume();
                    Console.WriteLine($"消费产品：{value}");
                    Thread.Sleep(3000);
                }
                Console.WriteLine($"消费完成");
            });

            consumer.Start();

            producer.Join();
            consumer.Join();

        }



    }
    class SingleBuffer
    {
        /// <summary>
        /// 信号量，代表缓冲区可访问
        /// </summary>
        private int s1 = 1;

        /// <summary>
        /// 信号量，代表产品数量
        /// </summary>
        private int s2 = 0;

        /// <summary>
        /// 生产的产品
        /// </summary>
        private int value;

        /// <summary>
        /// 生产
        /// </summary>
        /// <param name="value"></param>
        public void Produce(int value)
        {
            lock (this)
            {
                //先申请资源
                s1 = s1 - 1;
                //如果s1<0,则线程阻塞,因为后面的线程已经没有可用资源
                while (s1 < 0)
                {
                    Monitor.Wait(this);
                }

                this.value = value;
                s2 = s2 + 1;
                if (s2 <= 0)
                {
                    Monitor.Pulse(this);
                }
            }
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <returns></returns>
        public int Consume()
        {
            lock (this)
            {
                s2 = s2 - 1;
                while (s2 < 0)
                {
                    Monitor.Wait(this);
                }
                s1 = s1 + 1;
                //因为s1<0的绝对值表示有多少个线程被阻塞，所以 s1+当前被释放的一个资源后<=0,唤醒阻塞队列中的一个
                if (s1 <= 0)
                {
                    Monitor.Pulse(this);
                }
                return value;
            }
        }
    }

    class SingleBuffer2
    {
        /// <summary>
        /// 信号量，代表缓冲区可访问
        /// </summary>
        private int s1 = 0;

        /// <summary>
        /// 信号量，代表产品数量
        /// </summary>
        private int s2 = 0;

        /// <summary>
        /// 生产的产品
        /// </summary>
        private int value;

        public void Produce(int value)
        {
            lock (this)
            {
                this.value = value;
                s2 = s2 + 1;
                if (s2 <= 0)
                {
                    Monitor.Pulse(this);
                }

                //先申请资源
                s1 = s1 - 1;
                //如果s1<0,则线程阻塞,因为后面的线程已经没有可用资源
                while (s1 < 0)
                {
                    Monitor.Wait(this);
                }
            }
        }

        public int Consume()
        {
            lock (this)
            {
                s2 = s2 - 1;
                while (s2 < 0)
                {
                    Monitor.Wait(this);
                }
                s1 = s1 + 1;
                //因为s1<0的绝对值表示有多少个线程被阻塞，所以 s1+当前被释放的一个资源后<=0,唤醒阻塞队列中的一个
                if (s1 <= 0)
                {
                    Monitor.Pulse(this);
                }
                return value;
            }
        }
    }


    /// <summary>
    /// 多缓冲区 生产者消费者
    /// </summary>
    class MultiBuffer
    {
        //缓冲区容量
        private int s1;

        //互斥信号，生产者和消费者不能同时进入缓冲区
        private int mutex = 1;


        /// <summary>
        /// 表示是否有产品
        /// </summary>
        private int s2 = 0;

        /// <summary>
        /// 此处使用队列作缓冲区，因为生产和消费可能速率不一致，比如生产快，那么s1会<0,如果使用数组的话，此时消费者取数，buffer[-1]会出错
        /// </summary>
        private Queue<int> buffer;
        public MultiBuffer(int capacity)
        {
            this.s1 = capacity;
            buffer = new Queue<int>();
        }

        public void Produce(int value)
        {
            lock (this)
            {
                //先申请资源
                s1 = s1 - 1;
                //如果s1<0,则线程阻塞,因为后面的线程已经没有可用的缓冲区了
                while (s1 < 0)
                {
                    Monitor.Wait(this);
                }

                mutex = mutex - 1;
                //其他线程正在缓冲区，不能执行操作
                if (mutex < 0)
                {
                    Monitor.Wait(this);
                }
                buffer.Enqueue(value);


                mutex = mutex + 1;
                if (mutex <= 0)
                {
                    Monitor.Pulse(this);
                }

                s2 = s2 + 1;
                if (s2 <= 0)
                {
                    Monitor.Pulse(this);
                }
            }
        }

        public int Consume()
        {
            lock (this)
            {
                s2 = s2 - 1;
                if (s2 < 0)
                {
                    Monitor.Wait(this);
                }
                mutex = mutex - 1;
                if (mutex < 0)
                {
                    Monitor.Wait(this);
                }
                if (buffer.Count == 0)
                    throw new Exception("程序错，队列数为0");

                var value = buffer.Dequeue();

                mutex = mutex + 1;
                if (mutex <= 0)
                {
                    Monitor.Pulse(this);
                }

                s1 = s1 + 1;
                //因为s1<0的绝对值表示有多少个线程被阻塞，所以 s1+当前被释放的一个资源后<=0,唤醒阻塞队列中的一个
                if (s1 <= 0)
                {
                    Monitor.Pulse(this);
                }
                return value;
            }
        }

    }

}
