using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 回溯法
{
    class Program
    {
        static void Main(string[] args)
        {
            var queens=new NQueens();
            var locations= queens.NQueen(8);
            Console.WriteLine(string.Join(",",locations));
            Console.ReadKey();
        }
    }

}
