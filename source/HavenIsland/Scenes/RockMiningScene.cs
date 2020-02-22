using DeenGames.HavenIsland.Map.Entities;
using DeenGames.HavenIsland.Map.UI;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Scenes
{
    public class RockMiningScene : Scene
    {
        private const int GRID_WIDTH = 3;
        private const int GRID_HEIGHT = 3;
        private const int TILE_WIDTH = 150;
        private const int TILE_HEIGHT = 150;
        private const int FONT_SIZE = 72;
        private const int GRID_TILES_X_OFFSET = 300;
        private const int GRID_TILES_Y_OFFSET = 100;
        
        override public void Ready()
        {
            base.Ready();
            var random = new Random();

            this.BackgroundColour = 0x397b44;
            this.Add(new EnergyBar());

            // Model concerns
            var integrityLeft = 20 + random.Next(11); // 20-30

            var label = new Entity(true).Label($"Integrity left: {integrityLeft}");
            label.Get<TextLabelComponent>().FontSize = FONT_SIZE;
            this.Add(label);
            label.Move(GRID_TILES_X_OFFSET + 30, GRID_TILES_Y_OFFSET - FONT_SIZE - 16);

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var gridTile = new RockTile()
                        .Move(
                            GRID_TILES_X_OFFSET + (x * (TILE_WIDTH + 10)),
                            GRID_TILES_Y_OFFSET + (y * (TILE_HEIGHT + 10)));

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

            public RockTile() : base()
            {
                this.Integrity = 3 + random.Next(5); // 3-7
                this.Sprite(Path.Join("Content", "Images", "Sprites", "Rock-Texture.png"))
                    .Label($"{this.Integrity}", 40, -10);
                this.Get<TextLabelComponent>().FontSize = FONT_SIZE * 2;
            }
        }
    }
}