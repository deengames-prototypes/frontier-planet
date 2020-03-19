using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Map.UI;
using DeenGames.FrontierPlanet.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.Core.IO;
using Puffin.UI.Controls;
using System;
using System.IO;

namespace DeenGames.FrontierPlanet.Scenes
{
    public class ChopTreeScene : Scene
    {
        private const int GridWidth = 5;
        private const int GridHeight = 5;
        private const int TileWidth = 60;
        private const int TileHeight = 60;
        private const int FontSize = 36;
        private const int GridTilesXOffset = 300;
        private const int GridTilesYOffset = 100;

        private TreeModel model;
        private AreaMap map;
        private TreeTile[,] gridTiles = new TreeTile[GridWidth, GridHeight];
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

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    var gridTile = new TreeTile(x, y)
                        .Move(GridTilesXOffset + (x * TileWidth), GridTilesYOffset + (y * TileHeight));

                    gridTile
                        .Mouse(() => {
                            this.OnTileSelected(gridTile as TreeTile);
                        }, TileWidth, TileHeight);

                    this.gridTiles[x, y] = gridTile as TreeTile;
                    this.Add(gridTile);

                    if (x == GridWidth - 1)
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
                    else if (puff == PuffinAction.Down && this.cursorTile.TileIndicies.Item2 < GridHeight - 1 && this.gridTiles[this.cursorTile.TileIndicies.Item1, this.cursorTile.TileIndicies.Item2 + 1].IsDiscovered)
                    {
                        this.MoveCursorTo(this.gridTiles[this.cursorTile.TileIndicies.Item1, this.cursorTile.TileIndicies.Item2 + 1]);
                    }
                    else if (puff == PuffinAction.Left && this.cursorTile.TileIndicies.Item1 > 0 && this.gridTiles[this.cursorTile.TileIndicies.Item1 - 1, this.cursorTile.TileIndicies.Item2].IsDiscovered)
                    {
                        this.MoveCursorTo(this.gridTiles[this.cursorTile.TileIndicies.Item1 - 1, this.cursorTile.TileIndicies.Item2]);
                    }
                    else if (puff == PuffinAction.Right && this.cursorTile.TileIndicies.Item1 < GridWidth - 1 && this.gridTiles[this.cursorTile.TileIndicies.Item1 + 1, this.cursorTile.TileIndicies.Item2].IsDiscovered)
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
            this.progressBar.Move(GridTilesXOffset - 10, GridTilesYOffset - 72);
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
            for (var y = 0; y < GridHeight; y++)
            {
                for (var x = 0; x < GridWidth; x++)
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
                var currentProgress = (GridWidth - this.lastClicked.TileIndicies.Item1) * TileWidth;
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
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
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
                this.Get<TextLabelComponent>().FontSize = FontSize * 2;
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