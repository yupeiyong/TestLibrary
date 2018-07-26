using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

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
                    buffer.P(i);
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
                    var value = buffer.V();
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
                    buffer.P(i);
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
                    var value = buffer.V();
                    Console.WriteLine($"消费产品：{value}");
                    Thread.Sleep(2000);
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

        public void P(int value)
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

        public int V()
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

        public void P(int value)
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

        public int V()
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

}
