namespace bingo
{
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
            Selected = true;
            CoveringGroups.ForEach(group => group.Set());
        }

        public void Reset()
        {
            Selected = false;
            CoveringGroups.ForEach(group => group.Reset());
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