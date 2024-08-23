using BenchmarkDotNet.Attributes;

namespace bingo
{
    public class CardBenchmarks
    {
        private static Random _random = new Random();
        private static List<ICard> _cards = Card.GetRandomCards(10000, _random);
        private static List<ICard> _cards2 = Card2.GetRandomCards(10000, _random);
        private static SingleRound _round = new SingleRound(_cards);
        private static SingleRound _round2 = new SingleRound(_cards2);
        // private static MultipleRounds _multiRound = new MultipleRounds(_cards);
        // private static MultipleRounds _multiRound2 = new MultipleRounds(_cards2);

        [Benchmark]
        public void DoSingleRound()
        {
            _round.DoRound(0, _random);
        }

        [Benchmark]
        public void DoSingleRound2()
        {
            _round2.DoRound(0, _random);
        }

        // [Benchmark]
        // public void DoMultipleRounds()
        // {
        //     _multiRound.DoRounds(100, _random);
        // }

        // [Benchmark]
        // public void DoMultipleRounds2()
        // {
        //     _multiRound2.DoRounds(100, _random);
        // }
    }
}