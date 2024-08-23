using System.Diagnostics;

namespace bingo
{
    /// <summary>
    /// A second attempt at a Bingo card. In this class, a series of Cell objects
    /// is created, each representing one cell in the board. Each Cell then is
    /// connected to CellGroup objects, which represent the rows, columns, and diagonals.
    /// Each Cell is connected to at least 2 CellGroup objects, some are connected to 3,
    /// and the center Cell is connected to a row, column, and both diagonal CellGroups.
    /// 
    /// A couple of Dictionaries are used to link the cell coordinates the the corresponding
    /// Cell, and link the cell's value to the Cell that holds that value.
    /// 
    /// When a value is chosen in the game, and this board contains that value, the Cell
    /// is set, and the Cell then notifies the covering CellGroups. If any of the CellGroups
    /// are fully selected, the card is a winner.
    /// </summary>
    public class Card2 : ICard
    {
        private Dictionary<(int, int), Cell> _gridToCell;
        private Dictionary<int, Cell> _valueToCell;
        private CellGroup[] _groups;

        public bool IsWinner { get{ return IsWinnerHorizontally || IsWinnerVertically || IsWinnerDiagonally; } }
        public bool IsWinnerHorizontally { get; set; }
        public bool IsWinnerVertically { get; set; }
        public bool IsWinnerDiagonally { get; set; }

        public Card2()
        {
            _gridToCell = new Dictionary<(int, int), Cell>();
            _valueToCell = new Dictionary<int, Cell>();

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    Cell cell = new Cell();
                    _gridToCell[(row, col)] = cell;
                }
            }

            // There are 12 covering groups per card. 5 cover the 5 rows, each with 5 cells
            // 5 cover the 5 columns, each with 5 cells, and 1 for each diagonal, also
            // with 5 cells.
            _groups = new CellGroup[12];
            int groupIndex = 0;
            for (int row = 0; row < 5; row++)
            {
                CellGroup group = new HorizontalCellGroup(this, row);
                for (int col = 0; col < 5; col++)
                {
                    Cell cell = _gridToCell[(row, col)];
                    cell.CoveringGroups.Add(group);
                }
                _groups[groupIndex++] = group;
            }
            for(int col = 0; col < 5; col++)
            {
                CellGroup group = new VerticalCellGroup(this, col + 5);
                for(int row = 0; row < 5; row++)
                {
                    Cell cell = _gridToCell[(row, col)];
                    cell.CoveringGroups.Add(group);
                }
                _groups[groupIndex++] = group;
            }
            CellGroup diag1 = new DiagonalCellGroup(this, 10);
            CellGroup diag2 = new DiagonalCellGroup(this, 11);
            for(int i = 0; i < 5; i++)
            {
                Cell cell1 = _gridToCell[(i, i)];
                Cell cell2 = _gridToCell[(i, 4 - i)];

                cell1.CoveringGroups.Add(diag1);
                cell2.CoveringGroups.Add(diag2);
            }
            _groups[groupIndex++] = diag1;
            _groups[groupIndex++] = diag2;

            Reset();
        }

        /// <summary>
        /// Resets the Cell objects so they're no longer selected. Is done
        /// each time a game is started using the card.
        /// </summary>
        public void Reset()
        {
            foreach (Cell cell in _gridToCell.Values)
                cell.Reset();
            foreach (CellGroup group in _groups)
                group.Reset();
            _gridToCell[(2, 2)].Set();
            IsWinnerHorizontally = IsWinnerVertically = IsWinnerDiagonally = false;
        }

        /// <summary>
        /// Sets the value of the cell in the specified coordinates.
        /// Is only done when the card is being created.
        /// </summary>
        /// <param name="row">The row the value should go in</param>
        /// <param name="col">The column the value should go in</param>
        /// <param name="value">The value going in the specified cell</param>
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

        /// <summary>
        /// Called as the game is playing. Indicates that the specified number
        /// has been selected, and marks the appropriate Cell.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool SelectNumber(int number)
        {
            if(_valueToCell.TryGetValue(number, out Cell? cell))
            {
                cell.Set();
                return true;
            }
            return false;
        }

        public (int, bool)? GetCell(int number)
        {
            _valueToCell.TryGetValue(number, out Cell? cell);
            return cell != null ? (cell.Value, cell.Selected) : null;
        }

        public override string ToString()
        {
            string str = string.Empty;
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

        /// <summary>
        /// Static constructor class to create a random Card2
        /// </summary>
        /// <param name="random">The Random object to use</param>
        /// <returns>A new, random, Card2</returns>
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

                    card.SetCellValue(row, col, value);
                }
            }

            // Use -1 to indicate the free space at the center of the card
            card.SetCellValue(2, 2, -1);

            return card;
        }

        public static List<ICard> GetRandomCards(int numCards, Random random)
        {
            Stopwatch watch = Stopwatch.StartNew();
            List<ICard> cards = new List<ICard>(numCards);
            for (int i = 0; i < numCards; i++)
                cards.Add(Card2.CreateRandom(random));
            Console.WriteLine($"GetRandomCards-2: Creating {numCards} took {watch.ElapsedMilliseconds} ms");
            return cards;
        }
    }
}