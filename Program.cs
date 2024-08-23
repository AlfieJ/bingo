using System.Diagnostics;
using BenchmarkDotNet.Running;
using bingo;

internal class Program
{
    private static void Main(string[] args)
    {
        Random rand = new Random();
        int numCards = 100000;
        int numPasses = 100;

        List<ICard> cards = Card.GetRandomCards(numCards, rand);
        MultipleRounds cardsRound = new MultipleRounds(cards);
        cardsRound.DoRounds(numPasses, rand);

        // List<ICard> cards2 = Card2.GetRandomCards(numCards, rand);
        // MultipleRounds cards2Rounds = new MultipleRounds(cards2);
        // cards2Rounds.DoRounds(numPasses, rand);

        // BenchmarkRunner.Run<CardBenchmarks>();
        // BenchmarkRunner.Run<CellBenchmarks>();
    }
}
