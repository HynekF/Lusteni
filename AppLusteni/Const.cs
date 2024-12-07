
namespace Lusteni
{
    internal class Const
    {
        public const string Char_CH = "CH";

        public const string Char_CH_Substitution = "@";

        public const string Char_Blank = "$";

        public const string Char_Done = "✔️";

        public const string Char_Micro = "μ";        

        public const char Words_Splitter = '_';

        public const char NoGoZone = '☻';

        public const string Char_Space = " ";

        public const byte NUM_COUNT = 10;

        public const byte SLOT_SIZE = 3;

        public byte CELL_MAX_SCORE
        {
            get
            {
                byte _score = 0;
                for (byte i = 1; i < NUM_COUNT; i++)
                {
                    _score += i;
                }
                return (byte)(_score * 3);
            }
        }
    }
}
