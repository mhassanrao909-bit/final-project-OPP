namespace FlappyBird.Interfaces
{
    /// <summary>
    /// Defines the contract for all game entities that can be updated and drawn.
    /// </summary>
    public interface IGameEntity
    {
        /// <summary>
        /// Updates the entity's state each frame.
        /// </summary>
        void Update();

        /// <summary>
        /// Draws the entity into the provided screen buffer.
        /// </summary>
        /// <param name="buffer">The character screen buffer to draw on.</param>
        void Draw(char[,] buffer, ConsoleColor[,] fgBuffer, ConsoleColor[,] bgBuffer);
    }
}
