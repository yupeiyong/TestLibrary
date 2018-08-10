using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Threads
{
    [TestClass]
    public class UnitTestAwait
    {
        [TestMethod]
        public void TestMethod1()
        {
            var result=Test();
            Task.WaitAll();
        }


        public async Task<int> Test()
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(1000*200);
                return 0;
            });
        }
    }
}
