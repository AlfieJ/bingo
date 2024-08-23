namespace bingo
{
    public class SingleRound
    {
        private List<ICard> _cards;

        public SingleRound(List<ICard> cards)
        {
            _cards = cards;
        }

        /// <summary>
        /// Does the actual round using the specified cards. Calculates how many of
        /// the cards are winners in the horizontal, vertical, and diagonal directions.
        /// Each card is independent, so Parallel.ForEach is used to maximize the
        /// available CPUs.
        /// </summary>
        /// <param name="round">Which round this is...most likely it's done in a loop</param>
        /// <param name="random">The Random generator to use</param>
        /// <returns>A tuple of how many cards were a winner (horizontally, vertically, diagonally)</returns>
        public (int, int, int) DoRound(int round, Random random)
        {
            Parallel.ForEach(_cards, card => card.Reset());

            int horizontal = 0;
            int vertical = 0;
            int diagonal = 0;
            int pass = 1;

            List<int> ballNumbers = GetRandomBalls(random);
            List<ICard> winners = new List<ICard>();

            foreach (int ball in ballNumbers)
            {
                Parallel.ForEach(_cards, card =>
                {
                    card.SelectNumber(ball);
                    // Console.WriteLine($"{ball}:\n{card}");

                    if (card.IsWinner)
                    {
                        lock(winners)
                            winners.Add(card);
                    }
                });

                pass++;

                if (winners.Count > 0)
                {
                    winners.ForEach(winner =>
                    {
                        // Console.WriteLine(winner.ToString());

                        if (winner.IsWinnerHorizontally)
                            horizontal++;
                        if (winner.IsWinnerVertically)
                            vertical++;
                        if (winner.IsWinnerDiagonally)
                            diagonal++;
                    });

                    // Console.WriteLine($"Round {round} -- found winner after {pass} balls, {horizontal} h, {vertical} v, {diagonal} d");
                    
                    break;
                }
            };

            return (horizontal, vertical, diagonal);
        }


        public static List<int> GetRandomBalls(Random random)
        {
            List<int> ballNumbers = new List<int>(75);
            List<int> ordered = new List<int>(Enumerable.Range(1, 75));

            while (ordered.Count > 0)
            {
                int index = random.Next(ordered.Count);
                ballNumbers.Add(ordered[index]);
                ordered.RemoveAt(index);
            }

            return ballNumbers;
        }
    }
}