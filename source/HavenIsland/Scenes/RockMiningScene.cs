using DeenGames.HavenIsland.Map.Entities;
using DeenGames.HavenIsland.Map.UI;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using System;

namespace DeenGames.HavenIsland.Scenes
{
    public class RockMiningScene : Scene
    {
        private const int GRID_WIDTH = 3;
        private const int GRID_HEIGHT = 3;
        private const int TILE_WIDTH = 150;
        private const int TILE_HEIGHT = 150;
        
        override public void Ready()
        {
            base.Ready();
            var random = new Random();

            this.BackgroundColour = 0x397b44;
            this.Add(new EnergyBar());

            // Model concerns
            var integrityLeft = 20 + random.Next(11); // 20-30

            var label = new Entity(true).Label($"Integrity left: {integrityLeft}");
            this.Add(label);

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var gridTile = new RockTile()
                        .Move(300 + (x * (TILE_WIDTH + 10)), 100 + (y * (TILE_HEIGHT + 10)));

                    gridTile.Mouse(() => {
                        var tile = gridTile as RockTile;
                        
                        integrityLeft -= tile.Integrity;
                        Player.LatestInstance.SubtractEnergy(tile.Integrity);
                        label.Get<TextLabelComponent>().Text = $"Integrity left: {integrityLeft}";
                        this.Remove(tile);

                        if (integrityLeft <= 0)
                        {
                            // Communicate: success or failure, and which rock to smash
                            HavenIslandGame.LatestInstance.ShowScene(new MapScene());
                        }
                    }, TILE_WIDTH, TILE_HEIGHT);
                    this.Add(gridTile);
                }
            }
        }

        class RockTile : Entity
        {
            private static Random random = new Random();

            public int Integrity { get; private set; }
            private int[] colours = new int[] { 0x302c2e, 0x5a5353, 0x7d7071 };

            public RockTile() : base()
            {
                var colour = colours[random.Next(colours.Length)];
                this.Integrity = 3 + random.Next(5); // 3-7
                this.Colour(colour, TILE_WIDTH, TILE_HEIGHT)
                    .Label($"{this.Integrity}");
            }
        }
    }
}