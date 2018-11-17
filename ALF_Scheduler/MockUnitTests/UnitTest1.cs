using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MockUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AssertTrue_Pass()
        {
            Assert.IsTrue(true);
        }
    }
}
