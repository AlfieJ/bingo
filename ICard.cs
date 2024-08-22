namespace bingo
{
    public interface ICard
    {
        public bool IsWinner { get; }
        public bool IsWinnerHorizontally { get; set; }
        public bool IsWinnerVertically { get; set; }
        public bool IsWinnerDiagonally { get; set; }

        public void Reset();
        public bool SelectNumber(int number);
    }
}