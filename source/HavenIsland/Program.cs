using System;

namespace DeenGames.HavenIsland
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new HavenIslandGame())
            {
                game.Run();
            }
        }
    }
}
