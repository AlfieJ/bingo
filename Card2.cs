namespace bingo
{
    public class Card2 : ICard
    {
        private static int _nextCardNum = 1;

        private Dictionary<(int, int), Cell> _gridToCell;
        private Dictionary<int, Cell> _valueToCell;

        public int CardNum { get; private set; }

        public bool IsWinner { get{ return IsWinnerHorizontally || IsWinnerVertically || IsWinnerDiagonally; } }
        public bool IsWinnerHorizontally { get; set; }
        public bool IsWinnerVertically { get; set; }
        public bool IsWinnerDiagonally { get; set; }

        public Card2()
        {
            CardNum = _nextCardNum++;

            _gridToCell = new Dictionary<(int, int), Cell>();
            _valueToCell = new Dictionary<int, Cell>();

            for(int row = 0; row < 5; row++)
            {
                for(int col = 0; col < 5; col++)
                {
                    Cell cell = new Cell();
                    _gridToCell[(row, col)] = cell;
                }
            }

            // There are 12 covering groups per card. 5 cover the 5 rows, each with 5 cells
            // 5 cover the 5 columns, each with 5 cells, and 1 for each diagonal, also
            // with 5 cells.
            for(int row = 0; row < 5; row++)
            {
                CellGroup group = new CellGroup(this, CellGroup.Type.Horizontal, row);
                for(int col = 0; col < 5; col++)
                {
                    Cell cell = _gridToCell[(row, col)];
                    cell.CoveringGroups.Add(group);
                }
            }
            for(int col = 0; col < 5; col++)
            {
                CellGroup group = new CellGroup(this, CellGroup.Type.Vertical, col + 5);
                for(int row = 0; row < 5; row++)
                {
                    Cell cell = _gridToCell[(row, col)];
                    cell.CoveringGroups.Add(group);
                }
            }
            CellGroup diag1 = new CellGroup(this, CellGroup.Type.Diagonal, 11);
            CellGroup diag2 = new CellGroup(this, CellGroup.Type.Diagonal, 12);
            for(int i = 0; i < 5; i++)
            {
                Cell cell1 = _gridToCell[(i, i)];
                Cell cell2 = _gridToCell[(i, 4 - i)];

                cell1.CoveringGroups.Add(diag1);
                cell2.CoveringGroups.Add(diag2);
            }

            Reset();
        }

        public void Reset()
        {
            foreach (Cell cell in _gridToCell.Values)
                cell.Reset();
            _gridToCell[(2, 2)].Set();
            IsWinnerHorizontally = IsWinnerVertically = IsWinnerDiagonally = false;
        }

        public void SetCellValue(int row, int col, int value)
        {
            if (IsValid(value) == false)
                return;

            if(_gridToCell.TryGetValue((row, col), out Cell? cell))
            {
                _valueToCell.Remove(cell.Value);
                cell.Value = value;
                _valueToCell[value] = cell;
            }
        }

        public bool SelectNumber(int number)
        {
            if(_valueToCell.TryGetValue(number, out Cell? cell))
            {
                cell.Set();
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            string str = $"Card {CardNum}\n";
            for(int row = 0; row < 5; row++)
            {
                string rowStr = string.Empty;
                for(int col = 0; col < 5; col++)
                {
                    Cell cell = _gridToCell[(row, col)];
                    string selected = cell.Selected ? "*" : " ";

                    if(row == 2 && col == 2)
                        rowStr += $"-- ({selected}) ";
                    else
                        rowStr += $"{cell.Value:d2} ({selected}) ";
                }
                str += rowStr + "\n";
            }

            // for (int row = 0; row < 5; row++)
            // {
            //     for (int col = 0; col < 5; col++)
            //     {
            //         Cell cell = _gridToCell[(row, col)];
            //         str += cell.ToString() + "\n";
            //     }
            // }

            if (IsWinnerHorizontally)
                str += "Horizontal\n";
            if (IsWinnerVertically)
                str += "Vertical\n";
            if (IsWinnerDiagonally)
                str += "Diagonal";

            return str;
        }

        public static bool IsValid(int number)
        {
            return (number >= 1 && number <= 75) || number == -1;
        }

        public static Card2 CreateRandom(Random random)
        {
            Card2 card = new Card2();

            for(int col = 0; col < 5; col++)
            {
                List<int> numsInCol = new List<int>(Enumerable.Range(col * 15 + 1, 15));

                for(int row = 0; row < 5; row++)
                {
                    int index = random.Next(numsInCol.Count);
                    int value = numsInCol[index];
                    numsInCol.RemoveAt(index);

                    // Console.WriteLine($"Col == {col}, low {lowOfRange}, high {highOfRange - 1}, value {value}");
                    card.SetCellValue(row, col, value);
                }
            }

            // Use -1 to indicate the free space at the center of the card
            card.SetCellValue(2, 2, -1);

            return card;
        }
    }
}