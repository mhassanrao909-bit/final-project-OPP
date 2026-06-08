using FlappyBird.BL;
using FlappyBird.DL;
using FlappyBird.Enums;
using FlappyBird.Interfaces;
using FlappyBird.Models;
using FlappyBird.UI;

namespace FlappyBird.Engine
{
    /// <summary>
    /// Core game engine orchestrator for console-based Flappy Bird.
    /// Runs the physics/game logic on a background thread.
    /// Rendering is performed on the main thread via a render callback.
    /// </summary>
    public class GameEngine : IDisposable
    {
        // Game state
        private GameState _currentState;
        private readonly object _stateLock = new object();

        // Game entities
        private Bird _bird;

        // BL layer services
        private readonly IPipeManager _pipeManager;
        private readonly IScoreManager _scoreManager;
        private readonly ICollisionDetector _collisionDetector;

        // Console renderer
        private readonly ConsoleRenderer _renderer;

        // Game loop threading
        private CancellationTokenSource? _gameLoopCts;
        private Task? _gameLoopTask;
        private const int TargetFrameTimeMs = 80; // ~12 FPS for console
        private readonly object _updateLock = new object();

        // Input queue (thread-safe)
        private volatile bool _flapRequested;

        /// <summary>
        /// Current game state (thread-safe).
        /// </summary>
        public GameState CurrentState
        {
            get { lock (_stateLock) return _currentState; }
            private set { lock (_stateLock) _currentState = value; }
        }

        public int Score => _scoreManager.Score;
        public int HighScore => _scoreManager.HighScore;
        public int PlayAreaHeight => _renderer.PlayAreaHeight;

        public GameEngine(ConsoleRenderer renderer)
        {
            _renderer = renderer;
            _currentState = GameState.MainMenu;
            _flapRequested = false;

            _bird = new Bird(8, renderer.PlayAreaHeight / 2f);

            // Wire up BL and DL layers
            IScoreRepository scoreRepository = new ScoreRepository();
            _scoreManager = new ScoreManager(scoreRepository);
            _pipeManager = new PipeManager();
            _collisionDetector = new CollisionDetector();
        }

        /// <summary>
        /// Starts the game loop on a background thread.
        /// </summary>
        public void StartGameLoop()
        {
            StopGameLoop();
            _gameLoopCts = new CancellationTokenSource();
            var token = _gameLoopCts.Token;

            _gameLoopTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var frameStart = DateTime.UtcNow;

                    lock (_updateLock)
                    {
                        UpdateFrame();
                    }

                    var elapsed = (DateTime.UtcNow - frameStart).TotalMilliseconds;
                    int sleepTime = Math.Max(1, TargetFrameTimeMs - (int)elapsed);
                    try { await Task.Delay(sleepTime, token).ConfigureAwait(false); }
                    catch (TaskCanceledException) { break; }
                }
            }, token);
        }

        /// <summary>
        /// Stops the background game loop.
        /// </summary>
        public void StopGameLoop()
        {
            if (_gameLoopCts != null)
            {
                _gameLoopCts.Cancel();
                try { _gameLoopTask?.Wait(500); } catch { }
                _gameLoopCts.Dispose();
                _gameLoopCts = null;
                _gameLoopTask = null;
            }
        }

        /// <summary>
        /// Handles player input. Thread-safe — can be called from the input thread.
        /// </summary>
        public void OnPlayerInput()
        {
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    StartPlaying();
                    break;
                case GameState.Playing:
                    _flapRequested = true;
                    break;
                case GameState.GameOver:
                    ResetGame();
                    break;
            }
        }

        /// <summary>
        /// Renders the current frame to the console. Called from the main thread.
        /// </summary>
        public void Render()
        {
            lock (_updateLock)
            {
                switch (CurrentState)
                {
                    case GameState.MainMenu:
                        _renderer.DrawMainMenu();
                        break;
                    case GameState.Playing:
                        _renderer.DrawPlaying(_bird, _pipeManager.GetPipesSnapshot(),
                            Score, HighScore);
                        break;
                    case GameState.GameOver:
                        _renderer.DrawGameOver(Score, HighScore, _bird,
                            _pipeManager.GetPipesSnapshot());
                        break;
                }
            }
            _renderer.Flush();
        }

        private void StartPlaying()
        {
            CurrentState = GameState.Playing;
            _scoreManager.Reset();
            _pipeManager.Clear();
            _bird.Reset(8, _renderer.PlayAreaHeight / 2f);
            _bird.Flap();
        }

        private void ResetGame()
        {
            CurrentState = GameState.MainMenu;
            _scoreManager.Reset();
            _pipeManager.Clear();
            _bird.Reset(8, _renderer.PlayAreaHeight / 2f);
        }

        /// <summary>
        /// Core frame update — runs on the background thread.
        /// </summary>
        private void UpdateFrame()
        {
            if (CurrentState != GameState.Playing) return;

            // Process queued flap input
            if (_flapRequested)
            {
                _bird.Flap();
                _flapRequested = false;
            }

            // Update bird physics
            _bird.Update();

            // Delegate pipe management to BL
            _pipeManager.Update();
            _pipeManager.RemoveOffScreen();
            _pipeManager.SpawnIfNeeded(_renderer.ScreenWidth, PlayAreaHeight);

            // Scoring
            var pipes = _pipeManager.GetPipesSnapshot();
            foreach (var pipe in pipes)
            {
                if (!pipe.HasScored && pipe.X + pipe.Width < _bird.X)
                {
                    pipe.MarkScored();
                    _scoreManager.IncrementScore();
                }
            }

            // Collision detection
            if (_collisionDetector.CheckCollisions(_bird, pipes, PlayAreaHeight))
            {
                TriggerGameOver();
            }
        }

        private void TriggerGameOver()
        {
            _bird.Kill();
            _scoreManager.UpdateHighScore();
            CurrentState = GameState.GameOver;
        }

        public void Dispose()
        {
            StopGameLoop();
        }
    }
}
