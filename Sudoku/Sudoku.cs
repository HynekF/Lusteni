using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Sudoku
    {
        private SudokuCore sudokuCore;
        
        private const byte SLOT_SIZE = 3;

        public Sudoku(string fileNamePath)
        {
            sudokuCore = new SudokuCore();
            sudokuCore.LoadSudokuCore(fileNamePath);
        }

        public void Solve()
        {
            while(!sudokuCore.IsResultFinished)
            {
                for (byte rowIndex = 0; rowIndex < sudokuCore.Data.Count; rowIndex++)
                {
                    var horizontalNumbers = GetHorizontalNumbers(rowIndex);
                    for (byte colIndex = 0; colIndex < sudokuCore.Data[rowIndex].Length; colIndex++)
                    {
                        if (sudokuCore.Data[rowIndex][colIndex] > 0) continue;

                        var verticalNumbers = GetVerticalNumbers(rowIndex);
                        var slotNumbers = GetSlotNumbers(colIndex, rowIndex);

                        byte[] candidates = GetCandidates(horizontalNumbers, verticalNumbers, slotNumbers);
                        if(candidates.Length == 1)
                        {
                            sudokuCore.Data[rowIndex][colIndex] = candidates[0];
                            horizontalNumbers = GetHorizontalNumbers(rowIndex);
                        }
                    }
                }
            }
        }

        private byte[] GetCandidates(byte[] horizontalNumbers, byte[] verticalNumbers, byte[] slotNumbers)
        {
            byte[] num_ocurrences_count = new byte[Const.NUM_COUNT];

            // Merge the arrays
            byte[] allNumbers = horizontalNumbers
                .Concat(verticalNumbers)
                .Concat(slotNumbers)
                .ToArray();

            // Iterate through each element in the merged array
            foreach (byte number in allNumbers)
            {
                num_ocurrences_count[number + 1]++;
            }

            // candidates indices
            List<byte> candidate_numbers = new List<byte>();
            for (byte i = 0; i < Const.NUM_COUNT; i++)
            {
                if (num_ocurrences_count[i+1] == 0)
                {
                    candidate_numbers.Add((byte)(i+1));
                }
            }                        

            return candidate_numbers.ToArray();
        }

        private byte[] GetHorizontalNumbers(byte rowIndex)
        {
            byte[] result = new byte[sudokuCore.Dimension];
            for(int columnIndex=0;columnIndex< sudokuCore.Dimension;columnIndex++)
            {
                result[columnIndex] = sudokuCore.Data[rowIndex][columnIndex];
            }
            return result;
        }

        private byte[] GetVerticalNumbers(byte columnIndex)
        {
            byte[] result = new byte[sudokuCore.Dimension];
            for (int rowIndex = 0; rowIndex < sudokuCore.Dimension; rowIndex++)
            {
                result[rowIndex] = sudokuCore.Data[rowIndex][columnIndex];
            }
            return result;
        }

        private byte[] GetSlotNumbers(byte columnIndex, byte rowIndex)
        {
            byte[] result = new byte[sudokuCore.Dimension];
            byte _nrIndex = 0;
            int slotIndexVertical= rowIndex / SLOT_SIZE;
            int slotIndexHorizontal = columnIndex / SLOT_SIZE;

            for(int _rowIndex= slotIndexHorizontal* SLOT_SIZE; _rowIndex< slotIndexHorizontal * SLOT_SIZE + SLOT_SIZE; _rowIndex++)
            {
                for (int _columnIndex = slotIndexVertical * SLOT_SIZE; _columnIndex < slotIndexVertical * SLOT_SIZE + SLOT_SIZE; _columnIndex++)
                {
                    result[_nrIndex++] = sudokuCore.Data[_rowIndex][_columnIndex];
                }
            }
            return result;
        }
    }
}
