using System;

namespace connect4
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (connect4 game = new connect4())
            {
                game.Run();
            }
        }
    }
#endif
}

