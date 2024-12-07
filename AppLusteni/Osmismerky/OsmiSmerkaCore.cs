using System.IO;
using System.Text.RegularExpressions;

namespace Lusteni.Osmismerky
{
    internal class OsmiSmerkaCore
    {
        private List<string> _coreData;

        public IList<string> CoreData { get => _coreData; }

        public IList<bool[]> CoreDataUsageMap;

        // CH substitution with @

        public void LoadCoreData(string fileFullPath)
        {
            _coreData = new List<string>();

            using (StreamReader file = new StreamReader(fileFullPath))
            {
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    _coreData.Add(ln);
                }
                file.Close();
            }

            var _correctCoreData = new List<string>();
            foreach (string line in _coreData)
            {
                if (line.Length > ColumnsCount)
                {
                    _correctCoreData.Add(Tools.ForwardChSubstitution(line));
                }
                else
                {
                    _correctCoreData.Add(line);
                }
            }
            _coreData = _correctCoreData;

            foreach (string line in _coreData)
            {
                if (!_IsLineLengthConsistent(ColumnsCount, line))
                    throw new Exception("Core Data inconsistency");
            }

            CoreDataUsageMap = new List<bool[]>();
            for (int rowIndex = 0; rowIndex < RowsCount; rowIndex++)
            {
                bool[] rowMap = new bool[ColumnsCount];
                CoreDataUsageMap.Add(rowMap);
            }
        }

        private bool _IsLineLengthConsistent(int columnsCount, string currentLine)
        {
            var _isLineLengthConsistent = columnsCount == currentLine.Length;

            if(!_isLineLengthConsistent)
            {
                _isLineLengthConsistent = columnsCount == currentLine.Length - Regex.Matches(currentLine, "CH").Count;
            }
            return _isLineLengthConsistent;
        }

        private int _columnsCnt = 0;
        public int ColumnsCount 
        { 
            get 
            {
                if (_columnsCnt == 0)
                {
                    var _columnCounts = new List<int>();
                    foreach (var line in _coreData)
                    {
                        _columnCounts.Add(line.Length);
                    }
                    _columnsCnt = _columnCounts.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                }
                return _columnsCnt;
                
            } 
        }

        public int GetBlankCharsDistance()
        {
            int _result = 1;
            for(int _row=0; _row<RowsCount; _row++)
            {
                if (_coreData[_row].Contains(Const.Char_Blank))
                {
                    var firstIndex = _coreData[_row].IndexOf(Const.Char_Blank);
                    var secondIndex = _coreData[_row].Substring(firstIndex+1).IndexOf(Const.Char_Blank) +2;
                    _result = secondIndex - firstIndex;
                }
            }
            return _result;
        }

        private int GetBlankCharsCount(string targetSequence)
        {
            return targetSequence.Count(f => f == char.Parse(Const.Char_Blank));
        }

        public IList<int> GetBlankCharsDistances(string targetSequence)
        {
            var _blankCharsCount = GetBlankCharsCount(targetSequence);
            IList<int> _results = new List<int>();

            for (int _index = 0; _index < _blankCharsCount - 1; _index++)
            {
                var firstIndex = targetSequence.IndexOf(Const.Char_Blank);
                var secondIndex = targetSequence.Substring(firstIndex + 1).IndexOf(Const.Char_Blank) + 2;
                _results.Add(secondIndex - firstIndex);
            }
            return _results;
        }

        public int RowsCount { get { return CoreData.Count; } }

        public int DiagonalsCount { get { return RowsCount - 1 + ColumnsCount - 2; } }

        public int AntiDiagonalsCount => DiagonalsCount;

        public char? GetLetterAt(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr >= 0 && _columnNr < ColumnsCount && _rowNr >= 0 && _rowNr < RowsCount)
            {
                _resultLetter = _coreData[_rowNr].ElementAt(_columnNr);
            }
            return _resultLetter;
        }

        #region --- Get neighbour letters ---
        private static bool IsNullOrNoGoZone(char? ch)
        {
            return ch == null || ch == '☻';
        }

        private char? ApplyImageFilter(char? input)
        {
            char? result = null;
            if (!IsNullOrNoGoZone(input)) { result = input; }
            return result;
        }
        public char? GetTopLeftLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr > 0 && _columnNr < ColumnsCount && _rowNr > 0 && _rowNr < RowsCount)
            {
                _resultLetter = _coreData[_rowNr - 1].ElementAt(_columnNr - 1);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetTopLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_rowNr > 0 && _rowNr < RowsCount)
            {
                _resultLetter = _coreData[_rowNr - 1].ElementAt(_columnNr);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetTopRightLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr >= 0 && _columnNr < ColumnsCount - 1 && _rowNr > 0 && _rowNr < RowsCount)
            {
                _resultLetter = _coreData[_rowNr - 1].ElementAt(_columnNr + 1);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetRightLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_rowNr >= 0 && _rowNr < RowsCount && _columnNr >= 0 && _columnNr + 1 < _coreData[_rowNr].Length)
            {
                _resultLetter = _coreData[_rowNr].ElementAt(_columnNr + 1);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetBottomRightLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr >= 0 && _columnNr < ColumnsCount - 1 && _rowNr >= 0 && _rowNr < RowsCount - 1)
            {
                _resultLetter = _coreData[_rowNr + 1].ElementAt(_columnNr + 1);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetBottomLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr >= 0 && _columnNr <= ColumnsCount - 1 && _rowNr >= 0 && _rowNr < RowsCount - 1)
            {
                _resultLetter = _coreData[_rowNr + 1].ElementAt(_columnNr);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetBottomLeftLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr > 0 && _columnNr <= ColumnsCount - 1 && _rowNr >= 0 && _rowNr < RowsCount - 1)
            {
                _resultLetter = _coreData[_rowNr + 1].ElementAt(_columnNr - 1);
            }
            return ApplyImageFilter(_resultLetter);
        }

        public char? GetLeftLetter(int _columnNr, int _rowNr)
        {
            char? _resultLetter = null;
            if (_columnNr > 0 && _columnNr <= ColumnsCount - 1 && _rowNr >= 0 && _rowNr < RowsCount)
            {
                _resultLetter = _coreData[_rowNr].ElementAt(_columnNr - 1);
            }
            return ApplyImageFilter(_resultLetter);
        }
        #endregion

        public void MarkLetterAsUsed(int _column, int _row)
        {
            CoreDataUsageMap[_row][_column] = true;
        }
        public void MarkLetterAsUsed(LetterPosition _pos)
        {
            MarkLetterAsUsed(_pos.ColumnIndex, _pos.RowIndex);
        }

        public string GetResult()
        {
            string _result = string.Empty;

            for (int _row = 0; _row < RowsCount; _row++)
            {
                for (int _col = 0; _col < ColumnsCount; _col++)
                {
                    if (CoreDataUsageMap[_row][_col] == false && _coreData[_row].ElementAt(_col) != Const.NoGoZone)
                    {
                        _result += _coreData[_row].ElementAt(_col);
                    }
                }
            }
            return _result;
        }
    }
}