using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace async_await
{
    class Program
    {
        static void Main1(string[] args)
        {
            Console.WriteLine("Main test start..");
            Console.WriteLine("Main CurrentID:" + Thread.CurrentThread.ManagedThreadId.ToString());
            AsyncMethod();
            //Thread thread = new Thread(show);
            //thread.Start();
            Console.WriteLine("Main test end..");
            Console.WriteLine("s");
            Console.ReadKey();
        }

        static async void AsyncMethod()
        {
            Console.WriteLine("start aync CurrentID:" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine("start aync");
            var result = await MyMethod();//等待执行结果才会往下执行，但不会阻塞线程，不会开辟线程，会把控制权交给调用方，此处就是main();
            Console.WriteLine("aync over");
        }

        static async Task<int> MyMethod()//使用await必须使用async.
        {

            Console.WriteLine("CurrentID:" + Thread.CurrentThread.ManagedThreadId.ToString());//获取当前执行的线程ID
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("aync execute:" + i.ToString() + "..");
                await Task.Delay(1000); //模拟耗时操作
            }
            return 0;
        }


        static void show()
        {
            Console.WriteLine("new thread CurrentID:" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine("I am a new thread!");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("execute :" + i.ToString() + "..");
                Thread.Sleep(100);
            }
        }
    }
}
