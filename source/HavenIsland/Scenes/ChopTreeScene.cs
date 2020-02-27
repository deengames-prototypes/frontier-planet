using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Map.Entities;
using DeenGames.HavenIsland.Map.UI;
using DeenGames.HavenIsland.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeenGames.HavenIsland.Scenes
{
    public class ChopTreeScene : Scene
    {
        private const int GRID_WIDTH = 5;
        private const int GRID_HEIGHT = 5;
        private const int TILE_WIDTH = 60;
        private const int TILE_HEIGHT = 60;
        private const int FONT_SIZE = 36;
        private const int GRID_TILES_X_OFFSET = 300;
        private const int GRID_TILES_Y_OFFSET = 100;

        private Entity label;
        private TreeModel model;
        private TreeTile[,] gridTiles = new TreeTile[GRID_WIDTH, GRID_HEIGHT];

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

            this.label = new Entity(true).Label("");
            this.label.Get<TextLabelComponent>().FontSize = 48;
            this.Add(this.label);
            this.label.Move(GRID_TILES_X_OFFSET + 30, GRID_TILES_Y_OFFSET - 48 - 16);

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var gridTile = new TreeTile(x, y)
                        .Move(GRID_TILES_X_OFFSET + (x * TILE_WIDTH), GRID_TILES_Y_OFFSET + (y * TILE_HEIGHT));

                    gridTile
                        .Mouse(() => {
                            this.OnTileSelected(gridTile as TreeTile);
                        }, TILE_WIDTH, TILE_HEIGHT)
                        .Keyboard((action) => {
                            // TODO: selected tile
                        });

                    this.gridTiles[x, y] = gridTile as TreeTile;
                    this.Add(gridTile);
                }
            }

            // Cancel if you hit escape.
            this.OnActionPressed = (data) =>
            {
                if ((HavenIslandActions)data == HavenIslandActions.Cancel)
                {
                    HavenIslandGame.LatestInstance.ShowScene(new MapScene());
                }
            };

            var cancelButton = new Button("", () => HavenIslandGame.LatestInstance.ShowScene(new MapScene()))
                .Sprite(Path.Join("Content", "Images", "UI", "X-Button.png"));
            
            cancelButton.Move(HavenIslandGame.LatestInstance.Width - 40 - 16, 16);
            this.Add(cancelButton);
        }

        private void OnTileSelected(TreeTile gridTile)
        {
            this.Remove(gridTile);
            EventBus.LatestInstance.Broadcast(GlobalEvents.ConsumedEnergy, gridTile.Integrity);
        }

        class TreeTile : Entity
        {
            private static Random random = new Random();
            
            public int Integrity { get; set; }
            public readonly Tuple<int, int> Coordinates;
            public bool IsDiscovered = false;

            public TreeTile(int x, int y) : base()
            {
                this.Coordinates = new Tuple<int, int>(x, y);
                this.Integrity = 3 + random.Next(5); // 3-7
                this.Sprite(Path.Join("Content", "Images", "Sprites", "Tree-Texture.png"))
                    .Label($"{this.Integrity}", 10, -10);
                this.Get<TextLabelComponent>().FontSize = FONT_SIZE * 2;
            }
        }
    }
}