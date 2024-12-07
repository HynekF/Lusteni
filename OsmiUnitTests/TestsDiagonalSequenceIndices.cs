using Lusteni.Osmismerky;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OsmiUnitTests
{
    [TestClass]
    public class TestsDiagonalSequenceIndices
    {
        [TestMethod]
        [DataRow(0, 0, -1)]
        [DataRow(1, 0, 0)]
        [DataRow(0, 1, 0)]
        [DataRow(2, 0, 1)]
        [DataRow(3, 0, 2)]
        [DataRow(4, 0, 3)]
        [DataRow(5, 0, 4)]
        [DataRow(6, 0, 5)]
        [DataRow(7, 0, 6)]
        [DataRow(8, 0, 7)]
        [DataRow(3, 3, 5)]
        [DataRow(4, 3, 6)]
        [DataRow(4, 4, 7)]
        [DataRow(6, 3, 8)]
        [DataRow(7, 3, 9)]
        [DataRow(8, 4, -1)]
        public void TestDiagonalSequence_row_column(int rowIndex, int columnIndex, int expectedResult)
        {
            var index = DiagonalSequence.GetSequenceIndex(rowIndex, columnIndex, 9, 5);
            Assert.AreEqual(expectedResult, index, "Incorrect diagonal sequence index");
        }
    }
}
