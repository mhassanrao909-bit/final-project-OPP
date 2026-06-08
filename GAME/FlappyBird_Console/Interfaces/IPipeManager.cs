using FlappyBird.Models;

namespace FlappyBird.Interfaces
{
    /// <summary>
    /// Defines pipe lifecycle management contract.
    /// </summary>
    public interface IPipeManager
    {
        List<Pipe> GetPipesSnapshot();
        void Update();
        void SpawnIfNeeded(int screenWidth, int playAreaHeight);
        void RemoveOffScreen();
        void Clear();
    }
}
