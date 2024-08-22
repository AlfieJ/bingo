namespace bingo
{
    public class CellGroup
    {
        public enum Type {Horizontal, Vertical, Diagonal };

        private Card2 _card;
        private Type _type;
        private int _id;

        public int Count{ get; private set; }
        public bool IsWinner { get{ return Count == 5; }}

        public CellGroup(Card2 card, Type type, int id)
        {
            _card = card;
            _type = type;
            _id = id;
        }

        public void Set()
        {
            Count++;
            switch (_type)
            {
                case Type.Horizontal: _card.IsWinnerHorizontally = _card.IsWinnerHorizontally || IsWinner; break;
                case Type.Vertical: _card.IsWinnerVertically = _card.IsWinnerVertically || IsWinner; break;
                case Type.Diagonal: _card.IsWinnerDiagonally = _card.IsWinnerDiagonally || IsWinner; break;
            }
        }

        public void Reset()
        {
            Count = 0;
        }

        public override string ToString()
        {
            return $"Group {_id:d2}: count {Count}";
        }
    }
}