using System.Diagnostics;

namespace bingo
{
    /// <summary>
    /// Represents a single Bingo card. Board consists of a 5x5 array of (int, bool) which
    /// contains the value stored in that array location, and a bool if the value has been
    /// selected or not.
    /// </summary>
    public class Card : ICard
    {
        public (int, bool)[,] Board { get; set; }

        public bool IsWinner { get{ return IsWinnerHorizontally || IsWinnerVertically || IsWinnerDiagonally; } }
        public bool IsWinnerHorizontally { get; set; }
        public bool IsWinnerVertically { get; set; }
        public bool IsWinnerDiagonally { get; set; }

        public Card()
        {
            Board = new (int, bool)[5,5];
            Reset();
        }

        public void Reset()
        {
            // When resetting, simply reset the selection flag, not the value
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 5; col++)
                    Board[row, col] = (Board[row, col].Item1, false);
            Board[2, 2] = (-1, true);
            IsWinnerHorizontally = IsWinnerVertically = IsWinnerDiagonally = false;
        }

        /// <summary>
        /// Called when the number is chosen in the game. Sees if this card
        /// contains the number, and if it does, it is selected.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool SelectNumber(int number)
        {
            bool selected = false;
            int? column = GetColumn(number);
            if(column.HasValue)
            {
                for(int row = 0; row < 5; row++)
                {
                    if(Board[row, column.Value].Item1 == number)
                    {
                        Board[row, column.Value].Item2 = true;
                        selected = true;
                        CheckWinningCard();
                        break;
                    }
                }
            }
            else
                Console.Error.WriteLine($"Unable to determine column for number {number}");

            return selected;
        }

        public (int, bool)? GetCell(int number)
        {
            (int, bool)? cell = null;
            int? column = GetColumn(number);
            if (column.HasValue)
            {
                for (int row = 0; row < 5; row++)
                {
                    if (Board[row, column.Value].Item1 == number)
                    {
                        cell = Board[row, column.Value];
                        break;
                    }
                }
            }

            return cell;
        }

        public override string ToString()
        {
            string str = string.Empty;

            for(int row = 0; row < 5; row++)
            {
                string rowStr = string.Empty;
                for(int col = 0; col < 5; col++)
                {
                    string selected = Board[row, col].Item2 ? "*" : " ";

                    if(row == 2 && col == 2)
                        rowStr += $"-- ({selected}) ";
                    else
                        rowStr += $"{Board[row, col].Item1:d2} ({selected}) ";
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

        /// <summary>
        /// Check to see if the card is a winner. Checks each row, column, and diagonal
        /// to see if all cells are selected. Updates the appropriate IsWinner* element.
        /// </summary>
        private void CheckWinningCard()
        {
            // Check if any rows are fully selected
            bool isWinnerHorizontally = false;
            for(int row = 0; isWinnerHorizontally == false && row < 5; row++)
            {
                bool rowIsSelected = true;
                for(int col = 0; col < 5; col++)
                    rowIsSelected = Board[row, col].Item2 && rowIsSelected;
                isWinnerHorizontally = rowIsSelected;
            }

            // And check if any columns are fully selected
            bool isWinnerVertically = false;
            for(int col = 0; isWinnerVertically == false && col < 5; col++)
            {
                bool colIsSelected = true;
                for (int row = 0; row < 5; row++)
                    colIsSelected = Board[row, col].Item2 && colIsSelected;
                isWinnerVertically = colIsSelected;
            }

            // Check the two diagonals. We'll just use the single variable i for this loop
            bool diag1 = true;
            bool diag2 = true;
            for(int i = 0; i < 5; i++)
            {
                diag1 = Board[i, i].Item2 && diag1;
                diag2 = Board[i, 4 - i].Item2 && diag2;
            }

            IsWinnerHorizontally = isWinnerHorizontally;
            IsWinnerVertically = isWinnerVertically;
            IsWinnerDiagonally = diag1 || diag2;
        }

        public static bool IsValid(int number)
        {
            return number >= 1 && number <= 75;
        }

        /// <summary>
        /// Bingo numbers range from 1-75.
        /// Values from 1-15 go in the B column (index 0)
        /// Values from 16-30 go in the I column (index 1)
        /// Values from 31-45 go in the N column (index 2)
        /// Values from 46-60 go in the G column (index 3)
        /// Values from 61-75 go in the O column (index 4)
        /// </summary>
        /// <param name="number">A number between 1 to 75</param>
        /// <returns>An index column, or null if the number's not in the valid range</returns>
        private static int? GetColumn(int number)
        {
            int? column = null;
            if(IsValid(number))
                column = (number - 1) / 15;
            return column;
        }

        /// <summary>
        /// Static construction method. Creates a random Card using the
        /// supplied Random object.
        /// For each column, a list of valid numbers for that column is generated
        /// and values are randomly chosen from that list.
        /// </summary>
        /// <param name="random">The Random object to get the random numbers from</param>
        /// <returns>A randomized Card</returns>
        public static Card CreateRandom(Random random)
        {
            Card card = new Card();

            for(int col = 0; col < 5; col++)
            {
                List<int> numsInCol = new List<int>(Enumerable.Range(col * 15 + 1, 15));

                for(int row = 0; row < 5; row++)
                {
                    int index = random.Next(numsInCol.Count);
                    int value = numsInCol[index];
                    numsInCol.RemoveAt(index);

                    card.Board[row,col] = (value, false);
                }

                // Use -1 to indicate the free space at the center of the card
                card.Board[2, 2] = (-1, true);
            }

            return card;
        }

        public static List<ICard> GetRandomCards(int numCards, Random random)
        {
            Stopwatch watch = Stopwatch.StartNew();
            List<ICard> cards = new List<ICard>(numCards);
            for (int i = 0; i < numCards; i++)
                cards.Add(Card.CreateRandom(random));
            Console.WriteLine($"GetRandomCards: Creating {numCards} took {watch.ElapsedMilliseconds} ms");
            return cards;
        }
    }
}
