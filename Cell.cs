namespace bingo
{
    /// <summary>
    /// Represents a single cell in a Bingo card. It has the numeric value, and a bool
    /// indicating if the cell has been selected or not.
    /// 
    /// As a Cell is selected, it notifies the covering CellGroups to see if the card is a winner.
    /// </summary>
    public class Cell
    {
        public int Value { get; set; }
        public bool Selected { get; private set; }
        public List<CellGroup> CoveringGroups { get; private set; }

        public Cell()
        {
            CoveringGroups = new List<CellGroup>();
        }

        public void Set()
        {
            if(Selected)
                return;

            Selected = true;
            CoveringGroups.ForEach(group => group.Set());
        }

        public void Reset()
        {
            Selected = false;
            // CoveringGroups.ForEach(group => group.Reset());
        }

        public override string ToString()
        {
            string selected = Selected ? "*" : " ";
            string str = $"Value: {Value} {selected}\n";
            CoveringGroups.ForEach(group => str += group.ToString() + "\n");
            return str;
        }
    }
}