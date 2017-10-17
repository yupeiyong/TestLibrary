using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.Redis;


namespace TestProject.Redis
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var re = new RedisClient();
            re.Set("123", "888888888");
           var str=Encoding.UTF8.GetString(re.Get("123"));
            Assert.IsTrue(str== "888888888");
        }
    }
}
