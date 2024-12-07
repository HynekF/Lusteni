using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lusteni.Sudoku
{
    public enum SudokuStatus
    {
        ToDo = 0,
        InProgress = 1,
        Completed = 2,
        Incomplete = 3
    }

    public enum SolverPart
    {
        Part1=0,
        Part2=1,
    }

    public class SolverMetadata
    {
        public SudokuStatus Status { get; set; } = SudokuStatus.ToDo;

        private IList<int> _solverResultsCalculater = new List<int>();



        public void SetSolverResult(byte result, SolverPart part)
        {
            if (_solverResultsCalculater.Count > (int)part)
            {
                _solverResultsCalculater[(int)part] = result;
            }
            else
            {
                _solverResultsCalculater.Add(result);
            }
        }

        public string GetReport(long time)
        {
            string resutlsText = string.Empty;
            resutlsText += String.Format("Time: {0} [μs]\n", time.ToString());
            resutlsText += String.Format("Status: {0}\n", Status.ToString());
            for (int resultIndex = 0; resultIndex < _solverResultsCalculater.Count; resultIndex++)
            {
                resutlsText += String.Format("Part {0}: Resolved cells count: {1}\n", resultIndex, _solverResultsCalculater[resultIndex]);
            }
            return resutlsText;
        }

        internal void IncrementSolverResult(SolverPart targetComponent)
        {
            if (_solverResultsCalculater.Count > (int)targetComponent)
            {
                _solverResultsCalculater[(int)targetComponent]++;
            }
        }
    }
}
