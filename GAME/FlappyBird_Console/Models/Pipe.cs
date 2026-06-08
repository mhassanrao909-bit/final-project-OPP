namespace FlappyBird.Models
{
    /// <summary>
    /// Represents a pair of pipes (top and bottom) that the bird must navigate through.
    /// Movement speed and gap configuration are strictly private.
    /// </summary>
    public class Pipe : GameObject
    {
        // Strictly private movement and configuration variables
        private readonly float _speed;
        private readonly int _gapSize;
        private readonly int _gapY;
        private readonly int _playAreaHeight;
        private bool _scored;

        /// <summary>
        /// Whether the bird has already scored by passing this pipe pair.
        /// </summary>
        public bool HasScored
        {
            get => _scored;
            private set => _scored = value;
        }

        /// <summary>
        /// Whether this pipe has scrolled off the left side of the screen.
        /// </summary>
        public bool IsOffScreen => X + Width < 0;

        /// <summary>
        /// The Y-coordinate of the top of the gap.
        /// </summary>
        public int GapY => _gapY;

        /// <summary>
        /// The size (height) of the gap.
        /// </summary>
        public int GapSize => _gapSize;

        public Pipe(float x, int playAreaHeight, int gapSize, float speed)
            : base(x, 0, 4, playAreaHeight)
        {
            _speed = speed;
            _gapSize = gapSize;
            _playAreaHeight = playAreaHeight;
            _scored = false;

            // Randomize gap position within safe margins
            var rng = new Random();
            int margin = 3;
            _gapY = rng.Next(margin, playAreaHeight - gapSize - margin);
        }

        /// <summary>
        /// Marks this pipe as scored.
        /// </summary>
        public void MarkScored()
        {
            HasScored = true;
        }

        /// <summary>
        /// Updates pipe position by scrolling left.
        /// </summary>
        public override void Update()
        {
            X -= _speed;
        }

        /// <summary>
        /// Returns the bounding rectangle for the top pipe segment.
        /// </summary>
        public RectangleF GetTopPipeBounds()
        {
            return new RectangleF(X, 0, Width, _gapY);
        }

        /// <summary>
        /// Returns the bounding rectangle for the bottom pipe segment.
        /// </summary>
        public RectangleF GetBottomPipeBounds()
        {
            float bottomY = _gapY + _gapSize;
            return new RectangleF(X, bottomY, Width, _playAreaHeight - bottomY);
        }

        /// <summary>
        /// Draws the pipe pair into the console buffer using block characters.
        /// </summary>
        public override void Draw(char[,] buffer, ConsoleColor[,] fgBuffer, ConsoleColor[,] bgBuffer)
        {
            int maxRow = buffer.GetLength(0);
            int maxCol = buffer.GetLength(1);
            int startCol = (int)Math.Round(X);

            for (int row = 0; row < _playAreaHeight; row++)
            {
                // Skip the gap
                if (row >= _gapY && row < _gapY + _gapSize)
                    continue;

                bool isCapEdge = (row == _gapY - 1) || (row == _gapY + _gapSize);

                for (int i = 0; i < Width; i++)
                {
                    int col = startCol + i;
                    if (col < 0 || col >= maxCol || row < 0 || row >= maxRow)
                        continue;

                    if (isCapEdge)
                    {
                        // Pipe cap — wider visual with different char
                        buffer[row, col] = '▓';
                        fgBuffer[row, col] = ConsoleColor.Green;
                        bgBuffer[row, col] = ConsoleColor.DarkGreen;
                    }
                    else if (i == 0 || i == Width - 1)
                    {
                        // Pipe edges
                        buffer[row, col] = '║';
                        fgBuffer[row, col] = ConsoleColor.DarkGreen;
                        bgBuffer[row, col] = ConsoleColor.Black;
                    }
                    else
                    {
                        // Pipe body fill
                        buffer[row, col] = '█';
                        fgBuffer[row, col] = ConsoleColor.Green;
                        bgBuffer[row, col] = ConsoleColor.DarkGreen;
                    }
                }
            }
        }
    }
}
