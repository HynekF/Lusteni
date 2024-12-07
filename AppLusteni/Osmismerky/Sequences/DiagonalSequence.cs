using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OsmiUnitTests")]
namespace Lusteni.Osmismerky
{
    internal static class DiagonalSequence
    {       

        public static int GetSequenceIndex(int row, int column, int _rowCount, int _columnsCount)
        {
            int _index = -1;
            if (row == _rowCount - 1 && column == _columnsCount - 1)
            {
                return _index;
            }

            if (row > 0)
            {
                _index = row - 1;

                if (column > 0)
                {
                    _index += column;
                }
                return _index;
            }            
            
            if (column > 0 && row < _rowCount - column)
            {
                _index += column;
            }
            else if (row + column > _rowCount && column < _columnsCount - 1)
            {
                _index = _rowCount - 2 + column - 1;
            }
            else if(row == _rowCount -1 && column == _columnsCount - 1)
            {
                _index = -1;
            }
            return _index;
        }
    }
}
