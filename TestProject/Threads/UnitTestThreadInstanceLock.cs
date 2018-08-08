using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestProject.Threads
{

    [TestClass]
    public class UnitTestThreadStaticLock
    {

        /// <summary>
        ///     多个方法有静态锁
        ///     多线程，如果操作一个类的多个对象，多个线程修改静态成员的值，必须使用静态锁，因为是全局
        /// </summary>
        [TestMethod]
        public void Test_Multi_Function_With_Static_Lock()
        {
            var addTask = new Task(() => { new JobStaticLock().AddTask(); });
            addTask.Start();

            var addTask2 = new Task(() => { new JobStaticLock().AddTask(); });
            addTask2.Start();

            var reduceTask = new Task(() => { new JobStaticLock().ReduceTask(); });

            reduceTask.Start();

            var reduceTask2 = new Task(() => { new JobStaticLock().ReduceTask(); });

            reduceTask2.Start();

            addTask.Wait();
            reduceTask.Wait();
            addTask2.Wait();
            reduceTask2.Wait();
            Task.WaitAll();
            /*
线程：7加操作，结果：1
线程：7加操作，结果：2
线程：7加操作，结果：3
线程：7加操作，结果：4
线程：7加操作，结果：5
线程：7加操作，结果：6
线程：7加操作，结果：7
线程：7加操作，结果：8
线程：7加操作，结果：9
线程：7加操作，结果：10
线程：6加操作，结果：11
线程：6加操作，结果：12
线程：6加操作，结果：13
线程：6加操作，结果：14
线程：6加操作，结果：15
线程：6加操作，结果：16
线程：6加操作，结果：17
线程：6加操作，结果：18
线程：6加操作，结果：19
线程：6加操作，结果：20
线程：7减操作，结果：19
线程：7减操作，结果：18
线程：7减操作，结果：17
线程：7减操作，结果：16
线程：7减操作，结果：15
线程：7减操作，结果：14
线程：7减操作，结果：13
线程：7减操作，结果：12
线程：7减操作，结果：11
线程：7减操作，结果：10
线程：7减操作，结果：9
线程：7减操作，结果：8
线程：7减操作，结果：7
线程：7减操作，结果：6
线程：7减操作，结果：5
线程：7减操作，结果：4
线程：7减操作，结果：3
线程：7减操作，结果：2
线程：7减操作，结果：1
线程：7减操作，结果：0

             */
        }

    }

    public class JobStaticLock
    {

        private static readonly object Lock = new object();


        public static int Count { get; private set; }


        public void AddTask()
        {
            lock (Lock)
            {
                for (var i = 0; i < 10; i++)
                {
                    Count++;
                    Console.WriteLine($"线程：{Thread.CurrentThread.ManagedThreadId}加操作，结果：" + Count);

                    //模拟长时间操作
                    Thread.Sleep(50);
                }
            }
        }


        public void ReduceTask()
        {
            lock (Lock)
            {
                for (var i = 0; i < 10; i++)
                {
                    Count--;
                    Console.WriteLine($"线程：{Thread.CurrentThread.ManagedThreadId}减操作，结果：" + Count);
                }
            }
        }

    }

}