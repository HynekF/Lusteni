namespace Lusteni.Sudoku
{
    internal class CellLocation
    {
        public byte Column { get; set; }
        public byte Row { get; set; }

        public CellLocation(byte row = 0, byte column = 0)
        {
            Set(row, column);
        }

        public void Set(byte row, byte column)
        {
            Row = row;
            Column = column;
        }
    }
}
