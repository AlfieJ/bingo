namespace bingo
{
    /// <summary>
    /// Represents a row, column, or diagonal on a Card2
    /// 
    /// As a Cell is selected, each of its CellGroups is notified, and
    /// a CellGroup is a winner if all 5 cells in its group have been selected.
    /// </summary>
    public abstract class CellGroup
    {
        protected Card2 _card;
        private int _id;

        public int Count{ get; private set; }
        public bool IsWinner { get{ return Count == 5; }}

        public CellGroup(Card2 card, int id)
        {
            _card = card;
            _id = id;
        }

        /// <summary>
        /// Called by a Cell when it's being set. Intended to be overridden
        /// by the child classes. Updates the count and the child class then
        /// updates the appropriate flag in the Card2
        /// </summary>
        public virtual void Set()
        {
            Count++;
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

    public class HorizontalCellGroup : CellGroup
    {
        public HorizontalCellGroup(Card2 card, int id) : base(card, id)
        {
        }

        public override void Set()
        {
            base.Set();
            _card.IsWinnerHorizontally = _card.IsWinnerHorizontally || IsWinner;
        }
    }

    public class VerticalCellGroup : CellGroup
    {
        public VerticalCellGroup(Card2 card, int id) : base(card, id)
        {
        }

        public override void Set()
        {
            base.Set();
            _card.IsWinnerVertically = _card.IsWinnerVertically || IsWinner;
        }
    }


    public class DiagonalCellGroup : CellGroup
    {
        public DiagonalCellGroup(Card2 card, int id) : base(card, id)
        {
        }

        public override void Set()
        {
            base.Set();
            _card.IsWinnerDiagonally = _card.IsWinnerDiagonally || IsWinner;
        }
    }
}