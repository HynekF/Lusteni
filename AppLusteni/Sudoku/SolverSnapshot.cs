using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lusteni.Sudoku
{
    internal class SolverSnapshot
    {
        public IList<byte[]>? SudokuData { get; private set; }

      
        public SolverSnapshot()
        {            
        }

       
    }
}
