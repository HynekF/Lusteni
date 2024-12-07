using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lusteni.Sudoku
{
    public class Sudoku
    {
        private SudokuCore sudokuCore;

        private SolverMetadata _solverMetadata;

        public bool IsValid { get { return sudokuCore.IsValid; } }

        public bool IsResolved => sudokuCore.IsFinished;

        public string Name { get; private set; }

        public string ResultFileName { get { return Name.Replace(".txt", "_result.txt"); } }

        public event EventHandler<SudokuSolvedEventArgs> SolverFinished;

        public Sudoku(string fileNamePath)
        {
            Name = fileNamePath.Substring(fileNamePath.LastIndexOf(@"\") + 1);
            sudokuCore = new SudokuCore();
            _solverMetadata = new SolverMetadata();
            sudokuCore.LoadSudokuCore(fileNamePath);
        }
        
        public SudokuStatus Solve()
        {
            var _solver = new SudokuSolver(sudokuCore.GetAllData());
            if (_solver.Solve())
            {
                var result = _solver.GetResult();
                sudokuCore.SetCoreData(result);
            }
            return sudokuCore.IsFinished ? SudokuStatus.Completed : SudokuStatus.Incomplete;
        }

        public string GetResult()
        {
            return sudokuCore.GetCurrentSudokuString();
        }
               
    }
}
