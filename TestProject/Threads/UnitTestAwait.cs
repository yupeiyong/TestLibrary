using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

namespace TestProject.Threads
{
    [TestClass]
    public class UnitTestAwait
    {
        [TestMethod]
        public void TestMethod1()
        {
            new TestAwait().Method();
        }
    }
    class TestAwait
    {
        public async Task Method()
        {
            Console.WriteLine("start aync CurrentID:" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine("start aync");
            var result = await MyMethod();//等待执行结果才会往下执行，但不会阻塞线程，不会开辟线程，会把控制权交给调用方，此处就是main();
            Console.WriteLine("aync over");
        }

        static async Task<int> MyMethod()//使用await必须使用async.
        {

            Console.WriteLine("CurrentID:" + Thread.CurrentThread.ManagedThreadId.ToString());//获取当前执行的线程ID
            for (int i = 0; i < 30; i++)
            {
                Console.WriteLine("aync execute:" + i.ToString() + "..");
                await Task.Delay(1000); //模拟耗时操作
            }
            return 0;
        }
    }
}
