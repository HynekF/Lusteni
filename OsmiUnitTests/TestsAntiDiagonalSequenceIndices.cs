using Lusteni;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OsmiUnitTests
{
    [TestClass]
    public class TestsAntiDiagonalSequenceIndices
    {
        [TestMethod]
        [DataRow(0, 0,0)]
        [DataRow(1, 0, 1)]
        [DataRow(2, 0, 2)]
        [DataRow(3, 0, 3)]
        [DataRow(4, 0, 4)]
        [DataRow(5, 0, 5)]
        [DataRow(6, 0, 6)]
        [DataRow(7, 0, 7)]
        [DataRow(0, 1, 8)]
        [DataRow(0, 2, 9)]
        [DataRow(0, 3, 10)]
        [DataRow(1, 4, 10)]
        //[DataRow(4, 4, 7)]
        //[DataRow(6, 3, 8)]
        //[DataRow(7, 3, 9)]
        [DataRow(8, 0, -1)]
        [DataRow(0, 4, -1)]
        public void TestAntiDiagonalSequence_row_column(int rowIndex, int columnIndex, int expectedResult)
        {
            var index = AntiDiagonalSequence.GetSequenceIndex(rowIndex, columnIndex, 9, 5);
            Assert.AreEqual(expectedResult, index, "Incorrect anti-diagonal sequence index");
        }
    }
}
