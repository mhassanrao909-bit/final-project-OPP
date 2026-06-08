using FlappyBird.Interfaces;

namespace FlappyBird.BL
{
    /// <summary>
    /// Business logic: manages current score and high score tracking.
    /// Delegates persistence to the data layer via IScoreRepository.
    /// </summary>
    public class ScoreManager : IScoreManager
    {
        private int _score;
        private int _highScore;
        private readonly IScoreRepository _repository;
        private readonly object _lock = new object();

        public int Score
        {
            get { lock (_lock) return _score; }
        }

        public int HighScore
        {
            get { lock (_lock) return _highScore; }
        }

        public ScoreManager(IScoreRepository repository)
        {
            _repository = repository;
            _highScore = _repository.LoadHighScore();
            _score = 0;
        }

        /// <summary>
        /// Increments the current score by 1.
        /// </summary>
        public void IncrementScore()
        {
            lock (_lock) _score++;
        }

        /// <summary>
        /// Updates the high score if current score exceeds it, and persists to disk.
        /// </summary>
        public void UpdateHighScore()
        {
            lock (_lock)
            {
                if (_score > _highScore)
                {
                    _highScore = _score;
                    _repository.SaveHighScore(_highScore);
                }
            }
        }

        /// <summary>
        /// Resets the current score to zero for a new game.
        /// </summary>
        public void Reset()
        {
            lock (_lock) _score = 0;
        }
    }
}
