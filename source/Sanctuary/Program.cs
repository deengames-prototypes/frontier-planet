using System;

namespace DeenGames.Sanctuary
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new SanctuaryGame())
            {
                game.Run();
            }
        }
    }
}
