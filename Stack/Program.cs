using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stack
{
    class Program
    {
        static void Main(string[] args)
        {
            var stack=new Stack<int>();
            for (var i = 0; i < 50; i++)
            {
                stack.Push(i);
            }
            while(!stack.IsEmpty())
                Console.WriteLine(stack.Pop());
            Console.WriteLine("---------------------------------------------------------------------------\r\n");

            var linkStack=new LinkStack<int>();
            for (var i = 0; i < 50; i++)
            {
                linkStack.Push(i);
            }
            while (!linkStack.IsEmpty())
                Console.WriteLine(linkStack.Pop());

            Console.ReadKey();
        }
    }
}
