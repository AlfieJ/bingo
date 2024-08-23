using System.Diagnostics;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftAntimalwareAMFilter;

namespace bingo
{
    public class MultipleRounds
    {
        private List<ICard> _cards;

        public MultipleRounds(List<ICard> cards)
        {
            _cards = cards;
        }
        
        public void DoRounds(int numRounds, Random rand)
        {
            Stopwatch watch = Stopwatch.StartNew();
            int horizontal = 0;
            int vertical = 0;
            int diagonal = 0;

            SingleRound round = new SingleRound(_cards);

            for (int i = 0; i < numRounds; i++)
            {
                // Stopwatch watch2 = Stopwatch.StartNew();

                (int, int, int) pass = round.DoRound(i, rand);

                // Console.WriteLine($"Pass 1 {i + 1} took {watch2.ElapsedMilliseconds} ms");

                if (pass.Item1 > 0)
                    horizontal++;
                if (pass.Item2 > 0)
                    vertical++;
                if (pass.Item3 > 0)
                    diagonal++;
            }

            Console.WriteLine($"DoRounds: Horizontal {horizontal}, Vertical {vertical}, Diagonal {diagonal} in {watch.ElapsedMilliseconds} ms");
        }
    }
}