using System;

namespace BrokenCycleGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WasteGame game = new WasteGame())
            {
               game.Run();
            }
        }
    }
#endif
}

