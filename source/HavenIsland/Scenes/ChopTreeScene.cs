using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Map.Entities;
using DeenGames.HavenIsland.Map.UI;
using DeenGames.HavenIsland.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Scenes
{
    public class ChopTreeScene : Scene
    {
        private const int GRID_WIDTH = 5;
        private const int GRID_HEIGHT = 9;
        private const int TILE_WIDTH = 60;
        private const int TILE_HEIGHT = 60;
        private const int FONT_SIZE = 36;
        private const int GRID_TILES_X_OFFSET = 300;
        private const int GRID_TILES_Y_OFFSET = 100;

        private int integrityLeft;
        private Entity label;
        private TreeModel model;

        public ChopTreeScene(TreeModel model)
        {
            this.model = model;
        }

        override public void Ready()
        {
            base.Ready();
            var random = new Random();

            this.BackgroundColour = 0x397b44;
            this.Add(new EnergyBar());

            // Model concerns
            this.integrityLeft = 20 + random.Next(11); // 20-30

            this.label = new Entity(true).Label($"Integrity left: {integrityLeft}");
            this.label.Get<TextLabelComponent>().FontSize = 48;
            this.Add(this.label);
            this.label.Move(GRID_TILES_X_OFFSET + 30, GRID_TILES_Y_OFFSET - 48 - 16);
            int index = 0;

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    index++;

                    var gridTile = new TreeTile(index)
                        .Move(
                            GRID_TILES_X_OFFSET + (x * TILE_WIDTH),
                            GRID_TILES_Y_OFFSET + (y * TILE_HEIGHT));

                    gridTile
                        .Mouse(() => {
                            this.OnTileSelected(gridTile as TreeTile);
                        }, TILE_WIDTH, TILE_HEIGHT)
                        .Keyboard((action) => {
                            // TODO: selected tile
                        });

                    this.Add(gridTile);
                }
            }

            // Cancel if you hit escape.
            // TODO: communicate that the rock wasn't destroyed (update model)
            this.OnActionPressed = (data) =>
            {
                if ((HavenIslandActions)data == HavenIslandActions.Cancel)
                {
                    HavenIslandGame.LatestInstance.ShowScene(new MapScene());
                }
            };
        }

        private void OnTileSelected(TreeTile gridTile)
        {
            this.integrityLeft -= gridTile.Integrity;
            EventBus.LatestInstance.Broadcast(GlobalEvents.ConsumedEnergy, gridTile.Integrity);
            this.label.Get<TextLabelComponent>().Text = $"Integrity left: {integrityLeft}";
            this.Remove(gridTile);

            if (this.integrityLeft <= 0)
            {
                GameWorld.LatestInstance.AreaMap.Contents.Remove(this.model);                
                HavenIslandGame.LatestInstance.ShowScene(new MapScene());
            }
        }

        class TreeTile : Entity
        {
            private static Random random = new Random();
            public int Index { get; private set; }
            public int Integrity { get; private set; }

            public TreeTile(int index) : base()
            {
                this.Index = index;
                this.Integrity = 3 + random.Next(5); // 3-7
                this.Sprite(Path.Join("Content", "Images", "Sprites", "Tree-Texture.png"))
                    .Label($"{this.Integrity}", 10, -10);
                this.Get<TextLabelComponent>().FontSize = FONT_SIZE * 2;
            }
        }
    }
}