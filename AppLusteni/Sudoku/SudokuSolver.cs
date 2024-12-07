
namespace Lusteni.Sudoku
{
    /// <summary>
    /// ChatGPT algorithm
    /// </summary>
    internal class SudokuSolver
    {
        private IList<byte[]> Board;

        private byte Dimension
        {
            get
            {
                byte _dimension = 0;
                if (Board != null)
                {
                    _dimension = (byte)Board.Count;
                }
                return _dimension;
            }
        }

        public SudokuSolver(IList<byte[]> board)
        {            
            Board = board;
        }

        public IList<byte[]> GetResult()
        {
            return Board;
        }

        public bool Solve()
        {
            for (byte row = 0; row < Dimension; row++)
            {
                for (byte col = 0; col < Dimension; col++)
                {
                    if (Board[row][col] == 0)
                    {
                        for (byte num = 1; num <= Dimension; num++)
                        {
                            if (IsNumSafe(Board, row, col, num))
                            {
                                Board[row][col] = num;

                                if (Solve())
                                    return true;

                                Board[row][col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true; 
        }

        private bool IsNumSafe(IList<byte[]> board, byte row, byte col, byte num)
        {
            // Check row && column
            for (byte i = 0; i < Const.NUM_COUNT - 1; i++)
            {
                if (board[row][i] == num || board[i][col] == num)
                    return false;
            }

            // Check 3x3 subgrid
            int startRow = row - row % Const.SLOT_SIZE;
            int startCol = col - col % Const.SLOT_SIZE;

            for (byte i = 0; i < Const.SLOT_SIZE; i++)
                for (byte j = 0; j < Const.SLOT_SIZE; j++)
                    if (board[i + startRow][j + startCol] == num)
                        return false;

            return true;
        }
    }
}
