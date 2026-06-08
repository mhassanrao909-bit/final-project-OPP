using FlappyBird.Interfaces;

namespace FlappyBird.Models
{
    /// <summary>
    /// Abstract base class for all game objects, providing common positional
    /// and dimensional properties with encapsulated state.
    /// </summary>
    public abstract class GameObject : IGameEntity
    {
        private float _x;
        private float _y;
        private int _width;
        private int _height;

        public float X
        {
            get => _x;
            protected set => _x = value;
        }

        public float Y
        {
            get => _y;
            protected set => _y = value;
        }

        public int Width
        {
            get => _width;
            protected set => _width = value;
        }

        public int Height
        {
            get => _height;
            protected set => _height = value;
        }

        /// <summary>
        /// Returns the bounding rectangle for collision detection.
        /// </summary>
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        protected GameObject(float x, float y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public abstract void Update();

        public abstract void Draw(char[,] buffer, ConsoleColor[,] fgBuffer, ConsoleColor[,] bgBuffer);
    }

    /// <summary>
    /// Lightweight rectangle struct for collision detection (replaces System.Drawing.RectangleF).
    /// </summary>
    public struct RectangleF
    {
        public float X, Y, Width, Height;

        public RectangleF(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public bool IntersectsWith(RectangleF other)
        {
            return X < other.X + other.Width &&
                   X + Width > other.X &&
                   Y < other.Y + other.Height &&
                   Y + Height > other.Y;
        }
    }
}
