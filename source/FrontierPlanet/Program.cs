using System;

namespace DeenGames.FrontierPlanet
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new FrontierPlanetGame())
            {
                game.Run();
            }
        }
    }
}
