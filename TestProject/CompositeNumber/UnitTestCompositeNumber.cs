using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.CompositeNumber
{
    [TestClass]
    public class UnitTestCompositeNumber
    {
        [TestMethod]
        public void TestCreate()
        {
            var compositeNumber=new CompositeNumber(1,23);
            var numbers = compositeNumber.CompositeNumbers;
        }
    }
}
