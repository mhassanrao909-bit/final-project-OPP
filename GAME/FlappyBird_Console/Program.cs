using FlappyBird.Engine;
using FlappyBird.UI;

namespace FlappyBird
{
    internal static class Program
    {
        private const int ScreenWidth = 60;
        private const int ScreenHeight = 22;

        /// <summary>
        /// Main entry point. Runs the main loop on the primary thread:
        /// - Reads keyboard input (non-blocking)
        /// - Triggers rendering
        /// The game engine runs physics on a separate background thread.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Verify we have a real console
            if (!HasConsole())
            {
                Console.WriteLine("ERROR: Flappy Bird must be run in a real terminal window.");
                Console.WriteLine();
                Console.WriteLine("Please open one of the following and run:");
                Console.WriteLine("  Windows Terminal, cmd.exe, or PowerShell");
                Console.WriteLine();
                Console.WriteLine("  cd \"" + AppContext.BaseDirectory + "\"");
                Console.WriteLine("  dotnet run");
                return;
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var renderer = new ConsoleRenderer(ScreenWidth, ScreenHeight);
            renderer.Initialize();

            using var engine = new GameEngine(renderer);
            engine.StartGameLoop();

            // Main thread: input + render loop
            bool running = true;
            while (running)
            {
                // Non-blocking keyboard input
                try
                {
                    while (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);
                        switch (key.Key)
                        {
                            case ConsoleKey.Spacebar:
                            case ConsoleKey.UpArrow:
                                engine.OnPlayerInput();
                                break;
                            case ConsoleKey.Escape:
                                running = false;
                                break;
                        }
                    }
                }
                catch (InvalidOperationException) { }

                // Render current frame
                engine.Render();

                // Small sleep to prevent CPU spin
                Thread.Sleep(16);
            }

            engine.StopGameLoop();
            try
            {
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = true;
            }
            catch { }
            Console.WriteLine("Thanks for playing Flappy Bird!");
        }

        /// <summary>
        /// Checks whether a real interactive console is available.
        /// </summary>
        private static bool HasConsole()
        {
            try
            {
                _ = Console.KeyAvailable;
                return true;
            }
            catch
            {
                return !Console.IsInputRedirected;
            }
        }
    }
}
