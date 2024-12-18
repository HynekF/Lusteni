using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OsmiUnitTests")]
namespace Lusteni.Osmismerky.Sequences
{
    internal static class AntiDiagonalSequence
    {
        public static int GetSequenceIndex(int row, int column, int _rowCount, int _columnsCount)
        {
            int _index = -1;
            if (row == 0 && column == _columnsCount - 1 || row == _rowCount - 1 && column == 0)
            {
                return _index;
            }

            if (row >= 0)
            {
                _index = row;

                if (row - column < 0)
                {
                    _index = _rowCount - 1 + column - 1 - row;
                }
            }

            return _index;
        }
    }
}
