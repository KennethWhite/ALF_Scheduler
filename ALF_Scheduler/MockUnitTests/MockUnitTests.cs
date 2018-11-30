using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MockUnitTests
{
    [TestClass]
    public class MockUnitTests
    {
        [TestMethod]
        public void AssertTrue_Pass()
        {
            Assert.IsTrue(true);
        } 

        [TestMethod]
        [DataRow(5, 5, 10)]
        [DataRow(5, 0, 5)]
        [DataRow(-5, 5, 0)]
        [DataRow(-5, -5, -10)]
        public void MyMathAdd_AddTwoNumbers_ResultMatchesExpected(int lhs, int rhs, int expected)
        {
            Assert.AreEqual(expected, MyMath.Add(lhs, rhs));
        }
    }


    public static class MyMath
    {
        public static int Add(int lhs, int rhs)
        {
            return lhs + rhs;
        }
    }
}
