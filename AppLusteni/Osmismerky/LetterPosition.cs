using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lusteni.Osmismerky
{
    internal class LetterPosition
    {
        public int RowIndex { get; private set; }
        public int ColumnIndex { get; private set; }

        public LetterPosition(int _column, int _row)
        {
            ColumnIndex = _column;
            RowIndex = _row;
        }
    }
}
