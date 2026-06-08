namespace FlappyBird.Interfaces
{
    /// <summary>
    /// Defines scoring logic contract.
    /// </summary>
    public interface IScoreManager
    {
        int Score { get; }
        int HighScore { get; }
        void IncrementScore();
        void UpdateHighScore();
        void Reset();
    }
}
