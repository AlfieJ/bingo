using BenchmarkDotNet.Attributes;

namespace bingo
{
    public class CellBenchmarks
    {
        private static Random _random = new Random();
        private static List<ICard> _cards = Card.GetRandomCards(1000, _random);
        private static List<ICard> _cards2 = Card2.GetRandomCards(1000, _random);
        private List<int> _balls = SingleRound.GetRandomBalls(_random);

        [Benchmark]
        public void CardSelectNumber()
        {
            _balls.ForEach(ball =>
            {
                _cards.ForEach(card => card.SelectNumber(ball));
            });
        }

        [Benchmark]
        public void Card2SelectNumber()
        {
            _balls.ForEach(ball =>
            {
                _cards2.ForEach(card => card.SelectNumber(ball));
            });
        }

        [Benchmark]
        public void CardReset()
        {
            _cards.ForEach(card => card.Reset());
        }

        [Benchmark]
        public void Card2Reset()
        {
            _cards2.ForEach(card => card.Reset());
        }

        [Benchmark]
        public void CardGetCell()
        {
            _cards.ForEach(card => card.GetCell(11));
        }

        [Benchmark]
        public void Card2GetCell()
        {
            _cards2.ForEach(card => card.GetCell(11));
        }
    }
}