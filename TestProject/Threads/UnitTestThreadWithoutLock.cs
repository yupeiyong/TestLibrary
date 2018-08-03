using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestProject.Threads
{

    [TestClass]
    public class UnitTestThreadInstanceLock
    {

        /// <summary>
        ///     多个方法有锁
        ///     多线程，且有实例对象锁，一个时间内只能拥有一个对象锁
        /// </summary>
        [TestMethod]
        public void Test_Multi_Function_Has_Lock()
        {
            var job = new Job();
            var addTask = new Task(() =>
              {
                  job.AddTask();
              });
            addTask.Start();

            var addTask2 = new Task(() =>
            {
                job.AddTask();
            });
            addTask2.Start();

            var reduceTask = new Task(() =>
             {
                 job.ReduceTask();
             });

            reduceTask.Start();

            var reduceTask2 = new Task(() =>
            {
                job.ReduceTask();
            });

            reduceTask2.Start();

            addTask.Wait();
            reduceTask.Wait();
            addTask2.Wait();
            reduceTask2.Wait();
            Task.WaitAll();
            /*
             * 第一个加线程
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
                第二个加线程
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
                第一个减线程
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
                第二个减线程
                线程：6减操作，结果：9
                线程：6减操作，结果：8
                线程：6减操作，结果：7
                线程：6减操作，结果：6
                线程：6减操作，结果：5
                线程：6减操作，结果：4
                线程：6减操作，结果：3
                线程：6减操作，结果：2
                线程：6减操作，结果：1
                线程：6减操作，结果：0



             */
        }

    }

    public class Job
    {
        public void AddTask()
        {
            lock (this)
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
            lock (this)
            {
                for (var i = 0; i < 10; i++)
                {
                    Count--;
                    Console.WriteLine($"线程：{Thread.CurrentThread.ManagedThreadId}减操作，结果：" + Count);
                }
            }
        }


        public int Count { get; private set; } = 0;

    }

}