using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Lusteni.Sudoku;
using Lusteni.Osmismerky.Sequences;

namespace Lusteni.Osmismerky
{
    public enum OsmiStatus
    {
        ToDo = 0,
        InProgress = 1,
        Completed = 2,
        Incomplete = 3
    }

    internal class OsmiSmerka
    {
        private OsmiSmerkaCore _coreData;

        private IList<string> _wordsDictionary;

        public string Path { get; set; }

        public string WordsFile { get; set; }
        public string CoreFile { get; set; }

        public string Name
        {
            get
            {
                var _wordsFileParts = WordsFile.Split(Const.Words_Splitter);
                var _coreFileParts = CoreFile.Split(Const.Words_Splitter);

                string _name = string.Empty;

                if (_wordsFileParts.Length > 1 && _wordsFileParts.Length == _coreFileParts.Length &&
                    _wordsFileParts[0] == _coreFileParts[0] && _wordsFileParts[1] == _coreFileParts[1])
                {
                    _name = string.Format("{0} {1}", _wordsFileParts[0], _wordsFileParts[1]);
                }
                return _name;
            }
        }
        public string ResultFile
        {
            get
            {
                string _number = Regex.Match(CoreFile, @"\d+").Value;
                string _resultFileName = CoreFile.Substring(0, CoreFile.IndexOf(_number));
                return String.Format("{0}{1}_result.txt", _resultFileName, _number);
            }
        }

        public string Result { get; private set; } = string.Empty;

        public OsmiStatus ResultStatus { get; private set; } = OsmiStatus.ToDo;

        public IList<string> CoreData { get { return _coreData.CoreData; } }

        public IList<string> WordsDictionary => _wordsDictionary;

        private int WordsCount => _wordsDictionary.Count;

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(WordsFile) && !string.IsNullOrEmpty(CoreFile); }
        }

        private bool _IsCompatibleFile(string newFile)
        {
            bool isCompatible = true;

            var _newFileIndex = _GetFileIndex(newFile);

            if (newFile.Contains("words") && !string.IsNullOrEmpty(CoreFile))
            {
                isCompatible = _newFileIndex == _GetFileIndex(CoreFile);
            }
            else if (newFile.Contains("core") && !string.IsNullOrEmpty(WordsFile))
            {
                isCompatible = _newFileIndex == _GetFileIndex(WordsFile);
            }

            return isCompatible;
        }

        private int _GetFileIndex(string inputFileName)
        {
            var resultString = Regex.Match(inputFileName, @"\d+").Value;
            return int.Parse(resultString);
        }

        private IList<string> _GetReverseWords()
        {
            var _reverseWords = new List<string>();
            foreach(var word in WordsDictionary) 
            {
                char[] charArray = word.ToCharArray();
                Array.Reverse(charArray);
                _reverseWords.Add(new string(charArray));
            }
            return _reverseWords;
        }

        public void AssignFile(string fileName, string path)
        {
            if (string.IsNullOrEmpty(Path)) Path = path;
            if (fileName.Contains("words") && _IsCompatibleFile(fileName))
            {
                WordsFile = fileName;
            }
            else if (fileName.Contains("core") && _IsCompatibleFile(fileName))
            {
                CoreFile = fileName;
            }
        }

        public void LoadCoreData()
        {
            _coreData = new OsmiSmerkaCore();
            _coreData.LoadCoreData(System.IO.Path.Combine(Path, CoreFile));
        }

        public void LoadWordsDictionary()
        {
            _wordsDictionary = new List<string>();
            using (StreamReader file = new StreamReader(System.IO.Path.Combine(Path, WordsFile)))
            {
                string _word;

                while ((_word = file.ReadLine()) != null)
                {
                    var _wordCHsubstituted = Tools.ForwardChSubstitution(_word);
                    var _wordFinal = Tools.RemoveSpaces(_wordCHsubstituted);
                    _wordsDictionary.Add(_wordFinal);
                }
                file.Close();
            }
        }

        public OsmiStatus Resolve()
        {
            
            var _reverseWords = _GetReverseWords();
            var _wordsFoundIndices = new List<int>();

            bool[] columnsUsageMap = new bool[_coreData.ColumnsCount];
            bool[] rowsUsageMap = new bool[_coreData.RowsCount];
            bool[] diagonalsUsageMap = new bool[_coreData.DiagonalsCount];
            bool[] antiDiagonalsUsageMap = new bool[_coreData.AntiDiagonalsCount];

            var _sequencesWithBlankChars = new List<OsmDirectionSequence>();

            for (int _columnIterator = 0; _columnIterator < _coreData.ColumnsCount; _columnIterator++)
            {
                for (int _rowIterator = 0; _rowIterator < _coreData.RowsCount; _rowIterator++)
                {
                    //1. get 4 direction strings
                    OsmDirectionSequence _horizontal = null, _vertical = null, _diagonal = null, _antiDiagonal = null;
                    if (!rowsUsageMap[_rowIterator])
                    {
                        _horizontal = _GetHorizontalSequence(_columnIterator, _rowIterator);
                        rowsUsageMap[_rowIterator] = true;
                    }

                    if (!columnsUsageMap[_columnIterator])
                    {
                        _vertical = _GetVerticalSequence(_columnIterator, _rowIterator);
                        columnsUsageMap[_columnIterator] = true;
                    }

                    var _diagonalIndex = DiagonalSequence.GetSequenceIndex(_rowIterator, _columnIterator, _coreData.RowsCount, _coreData.ColumnsCount);
                    if (_diagonalIndex >= 0 && !diagonalsUsageMap[_diagonalIndex])
                    {
                        _diagonal = _GetDiagonalSequence(_columnIterator, _rowIterator);
                        diagonalsUsageMap[_diagonalIndex] = true;
                    }

                    var _antiDiagonalIndex = AntiDiagonalSequence.GetSequenceIndex(_rowIterator, _columnIterator, _coreData.RowsCount, _coreData.ColumnsCount);
                    if (_antiDiagonalIndex >= 0 && !antiDiagonalsUsageMap[_antiDiagonalIndex])
                    {
                        _antiDiagonal = _GetAntiDiagonalSequence(_columnIterator, _rowIterator);
                        antiDiagonalsUsageMap[_antiDiagonalIndex] = true;
                    }

                    var _directionSequences = new OsmDirectionSequence[] { _horizontal, _vertical, _diagonal, _antiDiagonal };

                    //2. iterate through words dictionary and find matching words
                    foreach (var ds in _directionSequences)
                    {
                        if (ds == null || !ds.IsSequenceValid) continue;

                        for (int wordIndex = 0; wordIndex < WordsDictionary.Count; wordIndex++)
                        {
                            if (_wordsFoundIndices.Contains(wordIndex)) continue;

                            if (ds.Sequence.Contains(WordsDictionary[wordIndex]))
                            {
                                //3. add word index to delete list
                                _wordsFoundIndices.Add(wordIndex);

                                //4. mark CoreDataUsageMap elements as true for corresponding letters
                                int _letterStartIndex = ds.Sequence.IndexOf(WordsDictionary[wordIndex]);
                                if (ds.Sequence.Contains(WordsDictionary[wordIndex]) && _letterStartIndex == -1)
                                {
                                    _letterStartIndex = 0;
                                }
                                for (int letterIndex = _letterStartIndex; letterIndex < _letterStartIndex + WordsDictionary[wordIndex].Length; letterIndex++)
                                {
                                    _coreData.MarkLetterAsUsed(ds.GetLetterPosition(letterIndex));
                                }
                            }
                            else if (ds.Sequence.Contains(_reverseWords[wordIndex]))
                            {
                                //3. add word index to delete list
                                _wordsFoundIndices.Add(wordIndex);

                                //4. mark CoreDataUsageMap elements as true for corresponding letters
                                int _letterStartIndex = ds.Sequence.IndexOf(_reverseWords[wordIndex]);
                                if(ds.Sequence.Contains(_reverseWords[wordIndex]) && _letterStartIndex == -1)
                                {
                                    _letterStartIndex = 0;
                                }
                                for (int letterIndex = _letterStartIndex; letterIndex < _letterStartIndex + _reverseWords[wordIndex].Length; letterIndex++)
                                {
                                    _coreData.MarkLetterAsUsed(ds.GetLetterPosition(letterIndex));
                                }
                            }
                        }
                         
                        if(ds.Sequence.Contains(Const.Char_Blank))
                        {
                            _sequencesWithBlankChars.Add(ds);
                        }
                    }
                    if( _wordsFoundIndices.Count == WordsCount ) { break; }
                }
                if (_wordsFoundIndices.Count == WordsCount) { break; }
            }
            var descendingDelList = _wordsFoundIndices.OrderByDescending(i => i);
            foreach (int wordToDeleteIndex in descendingDelList)
            {
                WordsDictionary.RemoveAt(wordToDeleteIndex);
                _reverseWords.RemoveAt(wordToDeleteIndex);
            }

            if(_sequencesWithBlankChars.Count > 0 && WordsDictionary.Count >0 && WordsDictionary.Count > 0)
            {
                _wordsFoundIndices.Clear();
                //for loop
                for (int wordIndex = 0; wordIndex < WordsDictionary.Count; wordIndex++)
                {
                    foreach(var blankCharSequence in _sequencesWithBlankChars)
                    {
                        // create word sequences with blank characters replacing single character always
                        var targetBlankWordsSequence = _GetBlankCharWordSequence(WordsDictionary[wordIndex]);
                        var targetBlankReverseWordsSequence = _GetBlankCharWordSequence(_reverseWords[wordIndex]);

                        //_coreData.GetBlankCharsDistances(targetWord)

                        for (int blankWordIndex=0; blankWordIndex<targetBlankWordsSequence.Count; blankWordIndex++)
                        {
                            if (blankCharSequence.Sequence.Contains(targetBlankWordsSequence[blankWordIndex]))
                            {
                                // _coreData.MarkLetterAsUsed
                                _wordsFoundIndices.Add(wordIndex);

                                int _letterStartIndex = blankCharSequence.Sequence.IndexOf(targetBlankWordsSequence[blankWordIndex]);
                                for (int letterIndex = _letterStartIndex; letterIndex < _letterStartIndex + targetBlankWordsSequence[blankWordIndex].Length; letterIndex++)
                                {
                                    _coreData.MarkLetterAsUsed(blankCharSequence.GetLetterPosition(letterIndex));
                                }
                            }
                            else if (blankCharSequence.Sequence.Contains(targetBlankReverseWordsSequence[blankWordIndex]))
                            {
                                // _coreData.MarkLetterAsUsed
                                _wordsFoundIndices.Add(wordIndex);

                                int _letterStartIndex = blankCharSequence.Sequence.IndexOf(targetBlankReverseWordsSequence[blankWordIndex]);
                                for (int letterIndex = _letterStartIndex; letterIndex < _letterStartIndex + targetBlankReverseWordsSequence[blankWordIndex].Length; letterIndex++)
                                {
                                    _coreData.MarkLetterAsUsed(blankCharSequence.GetLetterPosition(letterIndex));
                                }
                            }
                        }
                    }

                    // store what is the blank character
                }
                descendingDelList = _wordsFoundIndices.OrderByDescending(i => i);
                foreach (int wordToDeleteIndex in descendingDelList)
                {
                    WordsDictionary.RemoveAt(wordToDeleteIndex);
                    _reverseWords.RemoveAt(wordToDeleteIndex);
                }
            }

            if (WordsDictionary.Count > 0)
            {
                throw new Exception("Words not found: "+ String.Join("|", WordsDictionary.ToArray()));
            }
            Result = Tools.ReverseChSubstitution(_coreData.GetResult());

            ResultStatus = WordsDictionary.Count == 0 ? OsmiStatus.Completed : OsmiStatus.Incomplete;

            return ResultStatus;
        }              

        private IList<string> _GetBlankCharWordSequence(string targetWord)
        {
            IList<string> _blankCharWordsSequence = new List<string>();

            for(var i=0; i < targetWord.Length; i++)
            {
                string _blankCharWord = string.Empty;
                _blankCharWord += targetWord.Substring(0, _blankCharWordsSequence.Count);
                _blankCharWord += Const.Char_Blank;
                _blankCharWord += targetWord.Substring(_blankCharWordsSequence.Count+1);
                             
                _blankCharWordsSequence.Add(_blankCharWord);
            }
            int blankCharDistances = _coreData.GetBlankCharsDistance();
            
            int _firstBlankCharIndex = 0;
            if(blankCharDistances > 1 && blankCharDistances < targetWord.Length)
            {

            }

            return _blankCharWordsSequence;
        }

        private OsmDirectionSequence _GetHorizontalSequence(int _column, int _row)
        {
            int _columnIterator = _column;
            var _lettersPos = new List<LetterPosition>();
            char? _letter;
            string _sequence = _coreData.GetLetterAt(_column, _row).ToString();
            _lettersPos.Add(new LetterPosition(_column, _row));

            do
            {
                _letter = _coreData.GetLeftLetter(_columnIterator, _row);
                if (_letter != null)
                {
                    _sequence = _sequence.Insert(0, _letter.ToString());
                    _lettersPos.Insert(0,new LetterPosition(--_columnIterator, _row));
                }
            }
            while (_letter != null);
                        
            _columnIterator = _column;

            do
            {
                _letter = _coreData.GetRightLetter(_columnIterator, _row);
                if (_letter != null)
                {
                    _sequence += _letter.ToString();
                    _lettersPos.Add(new LetterPosition(++_columnIterator, _row));
                }
            }
            while (_letter != null);

            return new OsmDirectionSequence(_sequence, SequenceDirection.Horizontal, _lettersPos);
        }

        private OsmDirectionSequence _GetVerticalSequence(int _column, int _row)
        {
            int _rowIterator = _row;
            var _lettersPos = new List<LetterPosition>();
            char? _letter;

            string _sequence = _coreData.GetLetterAt(_column, _row).ToString();
            _lettersPos.Add(new LetterPosition(_column, _row));

            do
            {
                _letter = _coreData.GetTopLetter(_column, _rowIterator);
                if (_letter != null)
                {
                    _sequence = _sequence.Insert(0, _letter.ToString());
                    _lettersPos.Insert(0, new LetterPosition(_column, --_rowIterator));
                }                
            }
            while (_letter != null);
                        
            _rowIterator = _row;

            do
            {
                _letter = _coreData.GetBottomLetter(_column, _rowIterator);
                if (_letter != null)
                {
                    _sequence += _letter.ToString();
                    _lettersPos.Add(new LetterPosition(_column, ++_rowIterator));
                }                
            }
            while (_letter != null);

            return new OsmDirectionSequence(_sequence, SequenceDirection.Vertical, _lettersPos);
        }

        private OsmDirectionSequence _GetDiagonalSequence(int _column, int _row)
        {
            int _rowIterator = _row;
            int _columnIterator = _column;
            var _lettersPos = new List<LetterPosition>();
            char? _letter;
            
            string _sequence = _coreData.GetLetterAt(_column, _row).ToString();
            _lettersPos.Add(new LetterPosition(_column, _row));

            do
            {
                _letter = _coreData.GetBottomLeftLetter(_columnIterator, _rowIterator);
                if (_letter != null)
                {
                    _sequence = _sequence.Insert(0, _letter.ToString());
                    _lettersPos.Insert(0, new LetterPosition(--_columnIterator, ++_rowIterator));
                }                
            }
            while (_letter != null);

            
            _rowIterator = _row;
            _columnIterator = _column;

            do
            {
                _letter = _coreData.GetTopRightLetter(_columnIterator, _rowIterator);
                if (_letter != null)
                {
                    _sequence += _letter.ToString();
                    _lettersPos.Add(new LetterPosition(++_columnIterator, --_rowIterator));
                }                
            }
            while (_letter != null);

            return new OsmDirectionSequence(_sequence, SequenceDirection.Diagonal, _lettersPos);
        }

        private OsmDirectionSequence _GetAntiDiagonalSequence(int _column, int _row)
        {
            int _rowIterator = _row;
            int _columnIterator = _column;
            var _lettersPos = new List<LetterPosition>();
            char? _letter;

            string _sequence = _coreData.GetLetterAt(_column, _row).ToString();
            _lettersPos.Add(new LetterPosition(_column, _row));

            do
            {
                _letter = _coreData.GetTopLeftLetter(_columnIterator, _rowIterator);
                if (_letter != null)
                {
                    _sequence = _sequence.Insert(0, _letter.ToString());
                    _lettersPos.Insert(0, new LetterPosition(--_columnIterator, --_rowIterator));
                }                
            }
            while (_letter != null);
                        
            _rowIterator = _row;
            _columnIterator = _column;

            do
            {
                _letter = _coreData.GetBottomRightLetter(_columnIterator, _rowIterator);
                if (_letter != null)
                {
                    _sequence += _letter.ToString();
                    _lettersPos.Add(new LetterPosition(++_columnIterator, ++_rowIterator));
                }                
            }
            while (_letter != null);

            return new OsmDirectionSequence(_sequence, SequenceDirection.AntiDiagonal, _lettersPos);
        }
    }
}
