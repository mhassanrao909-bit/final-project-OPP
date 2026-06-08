using FlappyBird.Interfaces;
using FlappyBird.Models;

namespace FlappyBird.BL
{
    /// <summary>
    /// Business logic: handles all collision detection between the bird
    /// and game boundaries or pipe obstacles.
    /// </summary>
    public class CollisionDetector : ICollisionDetector
    {
        private const float HitboxShrink = 0.3f;

        /// <summary>
        /// Checks for collisions. Returns true if a fatal collision occurred.
        /// Also clamps the bird to boundaries.
        /// </summary>
        public bool CheckCollisions(Bird bird, List<Pipe> pipes, int playAreaHeight)
        {
            // Ground collision — fatal
            if (bird.Y + bird.Height >= playAreaHeight)
            {
                bird.ClampY(playAreaHeight - bird.Height);
                return true;
            }

            // Ceiling clamp — not fatal
            if (bird.Y <= 0)
            {
                bird.ClampY(0);
            }

            // Pipe collision — slightly smaller hitbox for fairness
            RectangleF birdBounds = bird.Bounds;
            RectangleF fairBounds = new RectangleF(
                birdBounds.X + HitboxShrink,
                birdBounds.Y + HitboxShrink,
                birdBounds.Width - HitboxShrink * 2,
                birdBounds.Height - HitboxShrink * 2);

            foreach (var pipe in pipes)
            {
                if (fairBounds.IntersectsWith(pipe.GetTopPipeBounds()) ||
                    fairBounds.IntersectsWith(pipe.GetBottomPipeBounds()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
