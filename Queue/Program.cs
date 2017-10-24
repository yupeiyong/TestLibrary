using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queue
{
    class Program
    {
        static void Main(string[] args)
        {
            var queue=new LinkQueue<int>();
            for (var i = 0; i < 50; i++)
            {
                queue.Enqueue(i);
            }
            Console.WriteLine("\r\n查找45---------------------------------------------------------------");
            var result = queue.Contains(45);
            if (result == false)
            {
                Console.WriteLine("未找到元素");
            }
            else
            {
                Console.WriteLine("找到元素：45");
            }

            while (!queue.IsEmpty())
            {
                Console.Write(queue.Dequeue()+", ");
            }


            Console.ReadKey();
        }
    }
}
