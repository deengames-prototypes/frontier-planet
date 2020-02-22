using Puffin.Core;
using Puffin.Core.Ecs;
using System;

namespace DeenGames.HavenIsland.Scenes
{
    public class RockMiningScene : Scene
    {
        private const int GRID_WIDTH = 3;
        private const int GRID_HEIGHT = 3;
        private Random random = new Random();

        public RockMiningScene() : base()
        {
            this.BackgroundColour = 0x397b44;

            // Model concerns
            var integrityLeft = 5 + random.Next(11); // 5-15
            var colours = new int[] { 0x302c2e, 0x5a5353, 0x7d7071 };

            var label = new Entity(true).Label($"Integrity left: {integrityLeft}");

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var colour = colours[random.Next(colours.Length)];
                    var gridTile = new Entity().Colour(colour, 150, 150).Move(300 + (x * 160), 100 + (y * 160));
                    this.Add(gridTile);
                }
            }
        }
    }
}