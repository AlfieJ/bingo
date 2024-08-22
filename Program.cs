using System.Diagnostics;
using bingo;

int numCards = 100000;
int numPasses = 1000;

DoPass1(numCards, numPasses);
DoPass2(numCards, numPasses);

static void DoPass1(int numCards, int numPasses)
{
    Random random = new Random();
    Stopwatch watch = Stopwatch.StartNew();
    int horizontal = 0;
    int vertical = 0;
    int diagonal = 0;

    List<ICard> cards = new List<ICard>();
    for (int i = 0; i < numCards; i++)
        cards.Add(Card.CreateRandom(random));
    Console.WriteLine($"Pass1: Creating {numCards} Cards took {watch.ElapsedMilliseconds} ms");

    Stopwatch watch2 = Stopwatch.StartNew();
    for (int i = 0; i < numPasses; i++)
    {
        // watch.Restart();

        (int, int, int) pass = DoPass(i, cards, random);

        // Console.WriteLine($"Pass 1 {i + 1} took {watch.ElapsedMilliseconds} ms");

        if (pass.Item1 > 0)
            horizontal++;
        if (pass.Item2 > 0)
            vertical++;
        if (pass.Item3 > 0)
            diagonal++;
    }

    Console.WriteLine($"Pass1: Horizontal {horizontal}, Vertical {vertical}, Diagonal {diagonal} in {watch2.ElapsedMilliseconds} ms");
}


static void DoPass2(int numCards, int numPasses)
{
    Random random = new Random();
    Stopwatch watch = Stopwatch.StartNew();
    int horizontal = 0;
    int vertical = 0;
    int diagonal = 0;

    List<ICard> cards = new List<ICard>();
    for (int i = 0; i < numCards; i++)
    {
        Card2 card = Card2.CreateRandom(random);
        cards.Add(card);
        // Console.WriteLine(card.ToString());
    }
    Console.WriteLine($"Pass2: creating {numCards} Cards took {watch.ElapsedMilliseconds} ms");

    Stopwatch watch2 = Stopwatch.StartNew();
    for (int i = 0; i < numPasses; i++)
    {
        // watch.Restart();

        (int, int, int) pass = DoPass(i, cards, random);

        // Console.WriteLine($"Pass 1 {i + 1} took {watch.ElapsedMilliseconds} ms");

        if (pass.Item1 > 0)
            horizontal++;
        if (pass.Item2 > 0)
            vertical++;
        if (pass.Item3 > 0)
            diagonal++;
    }

    Console.WriteLine($"Pass2: Horizontal {horizontal}, Vertical {vertical}, Diagonal {diagonal} in {watch2.ElapsedMilliseconds} ms");
}

static (int, int, int) DoPass(int round, List<ICard> cards, Random random)
{
    Parallel.ForEach(cards, card => card.Reset());

    int horizontal = 0;
    int vertical = 0;
    int diagonal = 0;

    List<int> ballNumbers = GetRandomBalls(random);

    int pass = 1;
    List<ICard> winners = new List<ICard>();

    foreach (int ball in ballNumbers)
    {
        Parallel.ForEach(cards, card =>
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

static List<int> GetRandomBalls(Random random)
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
