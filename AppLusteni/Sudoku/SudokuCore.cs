// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;


namespace Lusteni.Sudoku
{
    
    internal class SudokuCore
    {
        private IList<byte[]> Data;        
              
        public string Name { get; private set; }

        public SudokuType SudokuType { get; private set; } = SudokuType.Unknown;

        public byte Dimension
        {
            get
            {
                byte _dimension = 0;
                if (Data != null)
                {
                    _dimension = (byte)Data.Count;
                }
                return _dimension;
            }
        }

        public byte ColumnsCount
        {
            get
            {
                byte _columnsCount = 0;
                if (Data != null)
                {
                    _columnsCount = (byte)Data.First().Length;
                }
                return _columnsCount;
            }
        }

        public bool IsValid
        { get
            {
                bool result = true;
                foreach (byte[] data in Data) { result &= data.Length == Data.Count; }
                return result;
            } 
        }

        public IList<byte[]> GetAllData()
        {
            return Data;
        }

        public string GetCurrentSudokuString()
        {
            
            string result = string.Empty;
            foreach (var sudokuRow in Data)
            {
                if (!string.IsNullOrEmpty(result)) result += Environment.NewLine;
                foreach (var nr in sudokuRow)
                {
                    if (SudokuType == SudokuType.Alphabet)
                    {
                        result += string.Format("{0}", SudokuSolverTools.ConvertNumbers2Letters(nr.ToString()));
                    }
                    else
                    {
                        result += string.Format("{0}", nr);
                    }
                }

            }
            return result;
        }
        
        public void SetCoreData(IList<byte[]> data)
        {
            Data = data;
        }

        public void LoadSudokuCore(string fileFullPath)
        {
            
            var raw_data = new List<string>();
            Name = fileFullPath;
            using (StreamReader file = new StreamReader(fileFullPath))
            {
                string ln = string.Empty;

                while ((ln = file.ReadLine()) != null)
                {
                    raw_data.Add(ln);
                }
                file.Close();
            }

            Data = new List<byte[]>();
            SudokuType = SudokuSolverTools.GetSudokuType(string.Join(String.Empty, raw_data));

            if (SudokuType == SudokuType.Numeric)
            {
                foreach (string ln in raw_data)
                {
                    Data.Add(ln.Select(c => byte.Parse(c.ToString())).ToArray());
                }
            }
            else if (SudokuType == SudokuType.Alphabet)
            {
                foreach (string ln in raw_data)
                {
                    var ln_num = SudokuSolverTools.ConvertLetters2Numbers(ln);
                    Data.Add(ln_num.Select(c => byte.Parse(c.ToString())).ToArray());
                }
            }

        }

        public byte[] GetVerticalNumbers(byte columnIndex)
        {
            byte[] _vertical = new byte[Dimension];
            for (int rowIndex = 0; rowIndex < Dimension; rowIndex++)
            {
                _vertical[rowIndex] = Data[rowIndex][columnIndex];
            }
            return _vertical;
        }

        public byte[] GetHorizontalNumbers(byte rowIndex)
        {
            byte[] _horizontal = new byte[Dimension];
            for (int columnIndex = 0; columnIndex < Dimension; columnIndex++)
            {
                _horizontal[columnIndex] = Data[rowIndex][columnIndex];
            }
            return _horizontal;
        }

        public byte[] GetSlotNumbers(byte _rowIndex, byte _columnIndex)
        {
            return GetSlotNumbers(new CellLocation(_rowIndex, _columnIndex));
        }

        public byte[] GetSlotNumbers(CellLocation location)
        {
            byte[] _slot = new byte[Dimension];
            byte _nrIndex = 0;
            int slotIndexVertical = location.Column / Const.SLOT_SIZE;
            int slotIndexHorizontal = location.Row / Const.SLOT_SIZE;

            for (int _rowIndex = slotIndexHorizontal * Const.SLOT_SIZE; _rowIndex < slotIndexHorizontal * Const.SLOT_SIZE + Const.SLOT_SIZE; _rowIndex++)
            {
                for (int _columnIndex = slotIndexVertical * Const.SLOT_SIZE; _columnIndex < slotIndexVertical * Const.SLOT_SIZE + Const.SLOT_SIZE; _columnIndex++)
                {
                    _slot[_nrIndex++] = Data[_rowIndex][_columnIndex];
                }
            }
            return _slot;
        }    


        public byte GetCellNumber(byte row, byte column)
        {
            return Data[row][column];
        }
              

        //internal CellLocation? FindTheMostLikelyNextCandidate()
        //{
            
        //    var scoreMembers = Enumerable.Range(0, _scoreValues.GetLength(0))
        //    .SelectMany(row => Enumerable.Range(0, _scoreValues.GetLength(1)),
        //                (row, col) => new { Row = row, Col = col, Value = _scoreValues[row, col] })
        //    .OrderByDescending(item => item.Value);

        //    CellLocation? candidate = null;
        //    foreach (var maxBlankMember in scoreMembers)
        //    {
        //        if (Data[maxBlankMember.Row][maxBlankMember.Col] == 0)
        //        {
        //            candidate = new CellLocation((byte)maxBlankMember.Row, (byte)maxBlankMember.Col);
        //        }
        //    }           
        //    return candidate;            
        //}

        public bool IsFinished
        {
            get
            {
                return !Data.Any(array => array.Contains((byte)0));
            }
        }
    }
}