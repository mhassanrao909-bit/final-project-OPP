namespace FlappyBird.Interfaces
{
    /// <summary>
    /// Data layer contract for persisting scores.
    /// </summary>
    public interface IScoreRepository
    {
        int LoadHighScore();
        void SaveHighScore(int score);
    }
}
