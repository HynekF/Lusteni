

using Lusteni.Sudoku;

namespace LusteniUnitTests
{
    [TestClass]
    public class SudokuSolverTests
    {
        internal static string GetTestSudokuPath()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(codeBase), "SudokuTestData");
        }       
               

        //[TestMethod]
        //public void SolveNewAllTestData()
        //{
        //    var path = GetTestSudokuPath();

        //    var allSudokuFiles = Directory.GetFiles(path);

        //    foreach (var sudokuTestData in allSudokuFiles)
        //    {
        //        if (sudokuTestData.Contains("_result.txt")) continue;

        //        Console.WriteLine(String.Format("Testing: {0}", Path.GetFileName(sudokuTestData)));

        //        var sudokuCore_ref = new SudokuCore();
        //        sudokuCore_ref.LoadSudokuCore(sudokuTestData.Replace(".txt", "_result.txt"));
        //        string expected_result = sudokuCore_ref.GetCurrentSudokuString();

        //        var sudoku = new Sudoku(sudokuTestData);
        //        sudoku.SolveNew();
        //        string solution_result = sudoku.GetResult();
        //        Assert.AreEqual(solution_result, expected_result);
        //    }
        //}

        [TestMethod]
        public void Solver_TestAll()
        {
            var path = GetTestSudokuPath();

            var allSudokuFiles = Directory.GetFiles(path);

            foreach (var sudokuTestData in allSudokuFiles)
            {
                if (sudokuTestData.Contains("_result.txt")) continue;

                Console.WriteLine(String.Format("Testing: {0}", Path.GetFileName(sudokuTestData)));

                var sudokuCore_ref = new SudokuCore();
                sudokuCore_ref.LoadSudokuCore(sudokuTestData.Replace(".txt", "_result.txt"));
                string expected_result = sudokuCore_ref.GetCurrentSudokuString();

                var sudoku = new Sudoku(sudokuTestData);
                sudoku.Solve();
                string solution_result = sudoku.GetResult();
                Assert.AreEqual(solution_result, expected_result);
            }
        }
    }
}