namespace Lusteni.Sudoku
{
    public class SudokuSolvedEventArgs : EventArgs
    {
        public string SudokuName { get; set; }

        public string Status { get; set; }

        public string Result { get; set; }

        public string MetadataLog { get; set; }

        public string SolutionReport { get; set; }

        public string ResultFileName { get; set; }

        public long Duration { get; set; }


    }
}