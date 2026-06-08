using FlappyBird.Interfaces;

namespace FlappyBird.DL
{
    /// <summary>
    /// Data layer: persists the high score to a local file.
    /// </summary>
    public class ScoreRepository : IScoreRepository
    {
        private readonly string _filePath;

        public ScoreRepository()
        {
            string appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FlappyBird");
            Directory.CreateDirectory(appData);
            _filePath = Path.Combine(appData, "highscore.dat");
        }

        /// <summary>
        /// Loads the persisted high score from disk. Returns 0 if no file exists.
        /// </summary>
        public int LoadHighScore()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string content = File.ReadAllText(_filePath).Trim();
                    if (int.TryParse(content, out int score))
                        return score;
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Saves the high score to disk.
        /// </summary>
        public void SaveHighScore(int score)
        {
            try
            {
                File.WriteAllText(_filePath, score.ToString());
            }
            catch { }
        }
    }
}
