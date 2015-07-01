using System;
//Tetris Assault
//Last Updated 3/25/2010
//Created By Javier Marquez

namespace TetrisAssault
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

