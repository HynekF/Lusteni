using System.Collections.Generic;

namespace Lusteni.Osmismerky.Sequences
{
    internal enum SequenceDirection
    {
        Horizontal,
        Vertical,
        Diagonal,
        AntiDiagonal
    }

    internal class OsmDirectionSequence
    {
        public string Sequence { get; private set; }

        public SequenceDirection Direction { get; private set; }

        public IList<LetterPosition> LettersPos { get; private set; }

        public OsmDirectionSequence(string _sequence, SequenceDirection _direction, IList<LetterPosition> _lettersPos)
        {
            Sequence = _sequence;
            Direction = _direction;
            LettersPos = _lettersPos;
        }

        public LetterPosition GetLetterPosition(int _letterIndex)
        {
            return LettersPos[_letterIndex];
        }

        public bool IsSequenceValid { get { return Sequence.Length > 1; } }
    }
}
