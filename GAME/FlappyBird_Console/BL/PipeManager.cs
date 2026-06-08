using FlappyBird.Interfaces;
using FlappyBird.Models;

namespace FlappyBird.BL
{
    /// <summary>
    /// Business logic: manages the lifecycle of pipe obstacles —
    /// spawning, updating, removing off-screen pipes.
    /// </summary>
    public class PipeManager : IPipeManager
    {
        private readonly List<Pipe> _pipes = new List<Pipe>();
        private readonly object _lock = new object();

        private int _framesSinceLastSpawn;
        private const int SpawnInterval = 28;
        private const int GapSize = 7;
        private const float Speed = 0.8f;

        public PipeManager()
        {
            _framesSinceLastSpawn = SpawnInterval - 10;
        }

        public List<Pipe> GetPipesSnapshot()
        {
            lock (_lock) return new List<Pipe>(_pipes);
        }

        public void Update()
        {
            lock (_lock)
            {
                foreach (var pipe in _pipes)
                    pipe.Update();
            }
        }

        public void SpawnIfNeeded(int screenWidth, int playAreaHeight)
        {
            _framesSinceLastSpawn++;
            if (_framesSinceLastSpawn >= SpawnInterval)
            {
                lock (_lock)
                {
                    _pipes.Add(new Pipe(screenWidth, playAreaHeight, GapSize, Speed));
                }
                _framesSinceLastSpawn = 0;
            }
        }

        public void RemoveOffScreen()
        {
            lock (_lock) _pipes.RemoveAll(p => p.IsOffScreen);
        }

        public void Clear()
        {
            lock (_lock)
            {
                _pipes.Clear();
                _framesSinceLastSpawn = SpawnInterval - 10;
            }
        }
    }
}
