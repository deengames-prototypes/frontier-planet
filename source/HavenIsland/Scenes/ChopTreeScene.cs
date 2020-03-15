using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Map.UI;
using DeenGames.HavenIsland.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.UI.Controls;
using System;
using System.IO;

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

        private TreeModel model;
        private AreaMap map;
        private TreeTile[,] gridTiles = new TreeTile[GRID_WIDTH, GRID_HEIGHT];
        private TreeTile lastClicked;
        private HorizontalProgressBar progressBar;
        private Entity cursor;
        private TreeTile cursorTile;

        public ChopTreeScene(AreaMap map, TreeModel model)
        {
            this.map = map;
            this.model = model;
        }

        override public void Ready()
        {
            base.Ready();
            var random = new Random();

            this.BackgroundColour = 0x397b44;
            this.Add(new EnergyBar(this.EventBus));

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var gridTile = new TreeTile(x, y)
                        .Move(GRID_TILES_X_OFFSET + (x * TILE_WIDTH), GRID_TILES_Y_OFFSET + (y * TILE_HEIGHT));

                    gridTile
                        .Mouse(() => {
                            this.OnTileSelected(gridTile as TreeTile);
                        }, TILE_WIDTH, TILE_HEIGHT);

                    this.gridTiles[x, y] = gridTile as TreeTile;
                    this.Add(gridTile);

                    if (x == GRID_WIDTH - 1)
                    {
                        (gridTile as TreeTile).Show();
                    }
                }
            }

            // Cancel if you hit escape.
            this.OnActionPressed = (data) =>
            {
                if (data is HavenIslandActions)
                {
                    var havenAction = (HavenIslandActions)data;
                    if (havenAction == HavenIslandActions.Cancel)
                    {
                        HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map));
                    }
                    else if (havenAction == HavenIslandActions.Interact)
                    {
                        this.OnTileSelected(this.cursorTile);
                    }
                }
                else if (data is PuffinAction)
                {
                    var puff = (PuffinAction)data;
                    if (puff == PuffinAction.Up && this.cursorTile.TileIndicies.Item2 > 0 && this.gridTiles[this.cursorTile.TileIndicies.Item1, this.cursorTile.TileIndicies.Item2 - 1].IsDiscovered)
                    {
                        this.MoveCursorTo(this.gridTiles[this.cursorTile.TileIndicies.Item1, this.cursorTile.TileIndicies.Item2 - 1]);
                    }
                    else if (puff == PuffinAction.Down && this.cursorTile.TileIndicies.Item2 < GRID_HEIGHT - 1 && this.gridTiles[this.cursorTile.TileIndicies.Item1, this.cursorTile.TileIndicies.Item2 + 1].IsDiscovered)
                    {
                        this.MoveCursorTo(this.gridTiles[this.cursorTile.TileIndicies.Item1, this.cursorTile.TileIndicies.Item2 + 1]);
                    }
                    else if (puff == PuffinAction.Left && this.cursorTile.TileIndicies.Item1 > 0 && this.gridTiles[this.cursorTile.TileIndicies.Item1 - 1, this.cursorTile.TileIndicies.Item2].IsDiscovered)
                    {
                        this.MoveCursorTo(this.gridTiles[this.cursorTile.TileIndicies.Item1 - 1, this.cursorTile.TileIndicies.Item2]);
                    }
                    else if (puff == PuffinAction.Right && this.cursorTile.TileIndicies.Item1 < GRID_WIDTH - 1 && this.gridTiles[this.cursorTile.TileIndicies.Item1 + 1, this.cursorTile.TileIndicies.Item2].IsDiscovered)
                    {
                        this.MoveCursorTo(this.gridTiles[this.cursorTile.TileIndicies.Item1 + 1, this.cursorTile.TileIndicies.Item2]);
                    }
                }
            };

            var cancelButton = new Button("", () => HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map)))
                .Sprite(Path.Join("Content", "Images", "UI", "X-Button.png"));
            
            cancelButton.Move(HavenIslandGame.LatestInstance.Width - 40 - 16, 16);
            this.Add(cancelButton);

            this.progressBar = new HorizontalProgressBar(Path.Join("Content", "Images", "UI", "ProgressBar.png"), 0xF4B41B, 300, 8, 8);
            this.progressBar.Move(GRID_TILES_X_OFFSET - 10, GRID_TILES_Y_OFFSET - 72);
            this.progressBar.Value = 0;
            
            this.Add(this.progressBar);

            this.cursor = new Entity(true)
                .Sprite(Path.Combine("Content", "Images", "UI", "TileCursor.png"));
            this.Add(this.cursor);
            this.MoveCursorToFirstVisibleTile();
        }

        public void MoveCursorToFirstVisibleTile()
        {
            var tile = this.FirstGridTile(t => t.IsDiscovered);
            this.MoveCursorTo(tile);
        }

        private void MoveCursorTo(TreeTile tile)
        {
            this.cursor.Move(tile.X, tile.Y);
            this.cursorTile = tile;
        }

        private TreeTile FirstGridTile(Func<TreeTile, bool> lambda)
        {
            for (var y = 0; y < GRID_HEIGHT; y++)
            {
                for (var x = 0; x < GRID_WIDTH; x++)
                {
                    var currentTile = gridTiles[x, y];
                    if (lambda.Invoke(currentTile) == true)
                    {
                        return currentTile;
                    }
                }
            }

            return null;
        }

        private void OnTileSelected(TreeTile gridTile)
        {
            if (gridTile.IsDiscovered)
            {
                this.lastClicked = gridTile;
                this.Remove(gridTile);
                
                // Five tiles wide, so progress is 5-x for tile X we clicked on.
                var currentProgress = (GRID_WIDTH - this.lastClicked.TileIndicies.Item1) * TILE_WIDTH;
                this.progressBar.Value = currentProgress;

                GameWorld.LatestInstance.PlayerEnergy -= gridTile.Integrity;
                this.EventBus.Broadcast(GlobalEvents.ConsumedEnergy, gridTile.Integrity);

                if (gridTile.TileIndicies.Item1 == 0)
                {
                    // Done
                    // TODO: should probably use events for this
                    GameWorld.LatestInstance.AreaMap.Contents.Remove(this.model);
                    HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map));
                }
                else
                {
                    this.ShowHideGridTiles();
                    this.MoveCursorToFirstVisibleTile();
                }
            }
        }

        private void ShowHideGridTiles()
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    this.gridTiles[x, y].Hide();
                    var tileX = this.lastClicked.TileIndicies.Item1;
                    var tileY = this.lastClicked.TileIndicies.Item2;
                    if (x == tileX - 1 && Math.Abs(y - tileY) <= 1)
                    {
                        this.gridTiles[x, y].Show();
                    }
                }
            }
        }

        class TreeTile : Entity
        {
            private static Random random = new Random();
            
            public int Integrity { get; set; }
            public bool IsDiscovered { get; private set; }
            public readonly Tuple<int, int> TileIndicies;

            public TreeTile(int x, int y) : base()
            {
                this.TileIndicies = new Tuple<int, int>(x, y);
                this.Integrity = 3 + random.Next(5); // 3-7
                this.Sprite(Path.Join("Content", "Images", "Sprites", "Tree-Texture.png"))
                    .Label($"", 10, -10);
                this.Get<TextLabelComponent>().FontSize = FONT_SIZE * 2;
            }

            public void Show()
            {
                this.IsDiscovered = true;
                this.Get<TextLabelComponent>().Text = $"{this.Integrity}";
            }

            public void Hide()
            {
                this.IsDiscovered = false;
                this.Get<TextLabelComponent>().Text = "";
            }
        }
    }
}