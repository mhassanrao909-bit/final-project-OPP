using FlappyBird.Models;

namespace FlappyBird.Interfaces
{
    /// <summary>
    /// Defines collision detection contract.
    /// </summary>
    public interface ICollisionDetector
    {
        /// <summary>
        /// Checks if the bird collides with any boundary or pipe.
        /// Returns true if a collision occurred.
        /// </summary>
        bool CheckCollisions(Bird bird, List<Pipe> pipes, int playAreaHeight);
    }
}
