namespace FlappyBird.Models
{
    /// <summary>
    /// Represents the player-controlled bird entity for console rendering.
    /// Physics variables are strictly private; state exposed through controlled properties.
    /// </summary>
    public class Bird : GameObject
    {
        // Strictly private physics variables
        private float _velocity;
        private readonly float _gravity;
        private readonly float _lift;
        private readonly float _maxVelocity;

        /// <summary>
        /// Read-only access to the bird's current vertical velocity.
        /// </summary>
        public float Velocity => _velocity;

        /// <summary>
        /// Whether the bird is currently alive.
        /// </summary>
        public bool IsAlive { get; private set; }

        public Bird(float x, float y)
            : base(x, y, 3, 1)
        {
            _velocity = 0f;
            _gravity = 0.35f;
            _lift = -1.8f;
            _maxVelocity = 2.0f;
            IsAlive = true;
        }

        /// <summary>
        /// Applies an upward lift force, simulating a wing flap.
        /// </summary>
        public void Flap()
        {
            if (IsAlive)
            {
                _velocity = _lift;
            }
        }

        /// <summary>
        /// Marks the bird as dead after a collision.
        /// </summary>
        public void Kill()
        {
            IsAlive = false;
        }

        /// <summary>
        /// Clamps the bird's Y position (called by engine for boundary enforcement).
        /// </summary>
        public void ClampY(float y)
        {
            Y = y;
        }

        /// <summary>
        /// Resets the bird to its initial state for a new game.
        /// </summary>
        public void Reset(float x, float y)
        {
            X = x;
            Y = y;
            _velocity = 0f;
            IsAlive = true;
        }

        /// <summary>
        /// Updates bird physics: gravity, velocity clamping.
        /// </summary>
        public override void Update()
        {
            if (!IsAlive) return;

            _velocity += _gravity;
            _velocity = Math.Clamp(_velocity, -_maxVelocity, _maxVelocity);
            Y += _velocity;
        }

        /// <summary>
        /// Draws the bird as a colored character in the console buffer.
        /// Bird looks like: ═►
        /// </summary>
        public override void Draw(char[,] buffer, ConsoleColor[,] fgBuffer, ConsoleColor[,] bgBuffer)
        {
            int row = (int)Math.Round(Y);
            int col = (int)X;
            int maxRow = buffer.GetLength(0);
            int maxCol = buffer.GetLength(1);

            if (row < 0 || row >= maxRow) return;

            // Bird body characters: (●>
            char[] birdChars = { '(', '●', '>' };
            ConsoleColor[] birdColors = { ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.Red };

            for (int i = 0; i < birdChars.Length; i++)
            {
                int c = col + i;
                if (c >= 0 && c < maxCol)
                {
                    buffer[row, c] = birdChars[i];
                    fgBuffer[row, c] = birdColors[i];
                    bgBuffer[row, c] = ConsoleColor.DarkYellow;
                }
            }
        }
    }
}
