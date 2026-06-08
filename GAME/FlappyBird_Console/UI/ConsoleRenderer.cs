using FlappyBird.Enums;
using FlappyBird.Models;

namespace FlappyBird.UI
{
    /// <summary>
    /// Handles all console-based rendering for the game.
    /// Uses a double-buffered char array approach for flicker-free output.
    /// </summary>
    public class ConsoleRenderer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _playAreaHeight;
        private readonly int _groundRow;

        // Double-buffer arrays
        private readonly char[,] _charBuffer;
        private readonly ConsoleColor[,] _fgBuffer;
        private readonly ConsoleColor[,] _bgBuffer;
        private readonly char[,] _prevCharBuffer;
        private readonly ConsoleColor[,] _prevFgBuffer;
        private readonly ConsoleColor[,] _prevBgBuffer;
        private bool _firstFrame;

        // Animation
        private int _frameCount;

        public int ScreenWidth => _width;
        public int ScreenHeight => _height;
        public int PlayAreaHeight => _playAreaHeight;

        public ConsoleRenderer(int width, int height)
        {
            _width = width;
            _height = height;
            _playAreaHeight = height - 2; // 2 rows for ground
            _groundRow = _playAreaHeight;

            _charBuffer = new char[height, width];
            _fgBuffer = new ConsoleColor[height, width];
            _bgBuffer = new ConsoleColor[height, width];
            _prevCharBuffer = new char[height, width];
            _prevFgBuffer = new ConsoleColor[height, width];
            _prevBgBuffer = new ConsoleColor[height, width];
            _firstFrame = true;
            _frameCount = 0;
        }

        /// <summary>
        /// Sets up the console window for game rendering.
        /// </summary>
        public void Initialize()
        {
            try { Console.CursorVisible = false; } catch { }
            try { Console.Title = "Flappy Bird - Console Edition"; } catch { }

            try
            {
                Console.SetWindowSize(Math.Min(_width + 1, Console.LargestWindowWidth),
                                       Math.Min(_height + 1, Console.LargestWindowHeight));
                Console.SetBufferSize(Math.Min(_width + 1, Console.LargestWindowWidth),
                                       Math.Min(_height + 1, Console.LargestWindowHeight));
            }
            catch { }

            try { Console.Clear(); } catch { }
        }

        /// <summary>
        /// Clears the buffer to sky-blue background.
        /// </summary>
        public void ClearBuffer()
        {
            for (int r = 0; r < _height; r++)
            {
                for (int c = 0; c < _width; c++)
                {
                    if (r < _playAreaHeight)
                    {
                        _charBuffer[r, c] = ' ';
                        _fgBuffer[r, c] = ConsoleColor.White;
                        _bgBuffer[r, c] = ConsoleColor.DarkCyan;
                    }
                    else
                    {
                        // Ground rows
                        _charBuffer[r, c] = ' ';
                        _fgBuffer[r, c] = ConsoleColor.White;
                        _bgBuffer[r, c] = ConsoleColor.DarkYellow;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the ground decoration into the buffer.
        /// </summary>
        public void DrawGround()
        {
            // Top ground row — grass
            for (int c = 0; c < _width; c++)
            {
                int shifted = (c + _frameCount / 2) % 4;
                _charBuffer[_groundRow, c] = shifted == 0 ? '▲' : '═';
                _fgBuffer[_groundRow, c] = ConsoleColor.Green;
                _bgBuffer[_groundRow, c] = ConsoleColor.DarkGreen;
            }

            // Bottom ground row — dirt
            if (_groundRow + 1 < _height)
            {
                for (int c = 0; c < _width; c++)
                {
                    _charBuffer[_groundRow + 1, c] = '░';
                    _fgBuffer[_groundRow + 1, c] = ConsoleColor.DarkYellow;
                    _bgBuffer[_groundRow + 1, c] = ConsoleColor.DarkRed;
                }
            }
        }

        /// <summary>
        /// Draws all game entities (pipes, bird) into the buffer.
        /// </summary>
        public void DrawEntities(Bird bird, List<Pipe> pipes)
        {
            foreach (var pipe in pipes)
            {
                pipe.Draw(_charBuffer, _fgBuffer, _bgBuffer);
            }
            bird.Draw(_charBuffer, _fgBuffer, _bgBuffer);
        }

        /// <summary>
        /// Draws the score HUD at the top of the screen.
        /// </summary>
        public void DrawScoreHUD(int score, int highScore)
        {
            string hud = $" Score: {score}  |  Best: {highScore} ";
            int startCol = (_width - hud.Length) / 2;
            if (startCol < 0) startCol = 0;

            for (int i = 0; i < hud.Length && startCol + i < _width; i++)
            {
                _charBuffer[0, startCol + i] = hud[i];
                _fgBuffer[0, startCol + i] = ConsoleColor.White;
                _bgBuffer[0, startCol + i] = ConsoleColor.DarkBlue;
            }
        }

        /// <summary>
        /// Renders the main menu screen.
        /// </summary>
        public void DrawMainMenu()
        {
            ClearBuffer();
            DrawGround();

            // Title
            string[] title = {
                "╔═══════════════════════════╗",
                "║     F L A P P Y  B I R D  ║",
                "╚═══════════════════════════╝"
            };

            int titleRow = 4;
            foreach (string line in title)
            {
                int col = (_width - line.Length) / 2;
                for (int i = 0; i < line.Length && col + i < _width; i++)
                {
                    if (col + i >= 0)
                    {
                        _charBuffer[titleRow, col + i] = line[i];
                        _fgBuffer[titleRow, col + i] = ConsoleColor.Yellow;
                        _bgBuffer[titleRow, col + i] = ConsoleColor.DarkCyan;
                    }
                }
                titleRow++;
            }

            // Bird art
            string[] birdArt = {
                @"    ___      ",
                @"   (o >     ",
                @"   / |  ~~  ",
                @"  /  | /  \ ",
                @" (_ _|/    )",
                @"      \___/ "
            };

            int artRow = 9;
            int artCol = (_width - 14) / 2;
            foreach (string line in birdArt)
            {
                for (int i = 0; i < line.Length && artCol + i < _width; i++)
                {
                    if (artCol + i >= 0 && line[i] != ' ')
                    {
                        _charBuffer[artRow, artCol + i] = line[i];
                        _fgBuffer[artRow, artCol + i] = ConsoleColor.Yellow;
                        _bgBuffer[artRow, artCol + i] = ConsoleColor.DarkCyan;
                    }
                }
                artRow++;
            }

            // Prompt (flashing)
            bool showPrompt = (_frameCount / 8) % 2 == 0;
            if (showPrompt)
            {
                string prompt = ">>> Press SPACE to Start <<<";
                int pCol = (_width - prompt.Length) / 2;
                for (int i = 0; i < prompt.Length && pCol + i < _width; i++)
                {
                    if (pCol + i >= 0)
                    {
                        _charBuffer[17, pCol + i] = prompt[i];
                        _fgBuffer[17, pCol + i] = ConsoleColor.White;
                        _bgBuffer[17, pCol + i] = ConsoleColor.DarkCyan;
                    }
                }
            }

            // Controls
            string controls = "SPACE = Flap  |  ESC = Quit";
            int cCol = (_width - controls.Length) / 2;
            for (int i = 0; i < controls.Length && cCol + i < _width; i++)
            {
                if (cCol + i >= 0)
                {
                    _charBuffer[19, cCol + i] = controls[i];
                    _fgBuffer[19, cCol + i] = ConsoleColor.Gray;
                    _bgBuffer[19, cCol + i] = ConsoleColor.DarkCyan;
                }
            }
        }

        /// <summary>
        /// Renders the game-over overlay.
        /// </summary>
        public void DrawGameOver(int score, int highScore, Bird bird, List<Pipe> pipes)
        {
            ClearBuffer();
            DrawGround();
            DrawEntities(bird, pipes);

            // Dim the play area slightly by overriding background
            for (int r = 2; r < _playAreaHeight; r++)
            {
                for (int c = 0; c < _width; c++)
                {
                    _bgBuffer[r, c] = ConsoleColor.DarkGray;
                    if (_charBuffer[r, c] == ' ')
                        _fgBuffer[r, c] = ConsoleColor.DarkGray;
                }
            }

            // Game Over box
            string[] box = {
                "╔════════════════════════╗",
                "║      G A M E  O V E R  ║",
                "╠════════════════════════╣",
                $"║   Score:  {score,-13}║",
                $"║   Best:   {highScore,-13}║",
                "╠════════════════════════╣",
                "║  Press SPACE to retry  ║",
                "╚════════════════════════╝"
            };

            // Medal
            string medal = score >= 40 ? "★ GOLD ★" : score >= 20 ? "☆ SILVER ☆" : score >= 5 ? "● BRONZE ●" : "";
            if (medal.Length > 0)
            {
                box = box.Take(5)
                    .Append($"║   Medal: {medal,-13}║")
                    .Concat(box.Skip(5))
                    .ToArray();
            }

            int boxRow = (_playAreaHeight - box.Length) / 2;
            foreach (string line in box)
            {
                int col = (_width - line.Length) / 2;
                for (int i = 0; i < line.Length && col + i < _width; i++)
                {
                    if (col + i >= 0)
                    {
                        _charBuffer[boxRow, col + i] = line[i];
                        _fgBuffer[boxRow, col + i] = ConsoleColor.White;
                        _bgBuffer[boxRow, col + i] = ConsoleColor.DarkRed;
                    }
                }
                boxRow++;
            }
        }

        /// <summary>
        /// Renders the playing state: entities + ground + HUD.
        /// </summary>
        public void DrawPlaying(Bird bird, List<Pipe> pipes, int score, int highScore)
        {
            ClearBuffer();
            DrawGround();
            DrawEntities(bird, pipes);
            DrawScoreHUD(score, highScore);
        }

        /// <summary>
        /// Flushes the buffer to the console using differential update
        /// (only writes cells that changed since last frame).
        /// </summary>
        public void Flush()
        {
            _frameCount++;

            if (_firstFrame)
            {
                // Full render on first frame
                Console.SetCursorPosition(0, 0);
                for (int r = 0; r < _height; r++)
                {
                    for (int c = 0; c < _width; c++)
                    {
                        Console.ForegroundColor = _fgBuffer[r, c];
                        Console.BackgroundColor = _bgBuffer[r, c];
                        Console.Write(_charBuffer[r, c]);
                    }
                    if (r < _height - 1)
                        Console.WriteLine();
                }
                _firstFrame = false;
            }
            else
            {
                // Differential update — only write changed cells
                ConsoleColor lastFg = ConsoleColor.White;
                ConsoleColor lastBg = ConsoleColor.Black;

                for (int r = 0; r < _height; r++)
                {
                    for (int c = 0; c < _width; c++)
                    {
                        if (_charBuffer[r, c] != _prevCharBuffer[r, c] ||
                            _fgBuffer[r, c] != _prevFgBuffer[r, c] ||
                            _bgBuffer[r, c] != _prevBgBuffer[r, c])
                        {
                            Console.SetCursorPosition(c, r);

                            if (_fgBuffer[r, c] != lastFg)
                            {
                                Console.ForegroundColor = _fgBuffer[r, c];
                                lastFg = _fgBuffer[r, c];
                            }
                            if (_bgBuffer[r, c] != lastBg)
                            {
                                Console.BackgroundColor = _bgBuffer[r, c];
                                lastBg = _bgBuffer[r, c];
                            }

                            Console.Write(_charBuffer[r, c]);
                        }
                    }
                }
            }

            // Save current buffer as previous for next diff
            Array.Copy(_charBuffer, _prevCharBuffer, _charBuffer.Length);
            Array.Copy(_fgBuffer, _prevFgBuffer, _fgBuffer.Length);
            Array.Copy(_bgBuffer, _prevBgBuffer, _bgBuffer.Length);

            Console.ResetColor();
        }

        /// <summary>
        /// Provides the internal buffers for entity drawing.
        /// </summary>
        public (char[,] chars, ConsoleColor[,] fg, ConsoleColor[,] bg) GetBuffers()
        {
            return (_charBuffer, _fgBuffer, _bgBuffer);
        }
    }
}
