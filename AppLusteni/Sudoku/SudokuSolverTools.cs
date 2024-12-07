using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lusteni.Sudoku
{
    internal class SudokuSolverTools
    {

        public SolverMetadata Metadata { get; private set; }

        public SudokuCore SudokuCore { get; private set; }


        public SudokuSolverTools(SudokuCore sudokuCore)
        {

        }

        public static SudokuType GetSudokuType(string sequence)
        {
            string patternLetters = @"[a-zA-Z]";
            string patternNumbers = @"^[0-9]+$";

            SudokuType sudokuType = SudokuType.Unknown;

            if (Regex.IsMatch(sequence, patternLetters))
            {
                sudokuType = SudokuType.Alphabet;
            }
            else if (Regex.IsMatch(sequence, patternNumbers))
            {
                sudokuType = SudokuType.Numeric;
            }

            return sudokuType;
        }

        public static string ConvertNumbers2Letters(string sequence)
        {
            return string.Join("", sequence.ToCharArray().Select(c => (char)(c - '1' + 'A')));
        }

        public static byte[] ConvertLetters2Numbers(string sequence)
        {
            var line_letters_only = sequence.Replace('0', 'X');
            
            var intArray = line_letters_only.ToUpper().ToCharArray().Select(c => c - 'A' + 1).ToArray();

            byte[] byteArray = new byte[intArray.Length];
            
            for (int i = 0; i < intArray.Length; i++)
            {
                if (intArray[i] > 9)
                    byteArray[i] = 0;
                else
                    byteArray[i] = (byte)intArray[i];
            }
            return byteArray;
        }
    }
}
