using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Map.UI;
using DeenGames.HavenIsland.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.IO;
using Puffin.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace DeenGames.HavenIsland.Scenes
{
    public class MemoryChopTreeScene : Scene
    {
        // Easy: 4x4. Hard: 8x8.
        private const int GridWidth = 4;
        private const int GridHeight = 4;
        private const int TileWidth = 60;
        private const int TileHeight = 60;
        private const int FontSize = 36;
        private const int GridTilesXOffset = 300;
        private const int GridTilesYOffset = 100;
        private const int EnergyPerClick = 2;
        private const int ShowTilesSeconds = 1;
        // Fix bar as 300px wide
        private const int ProgressBarWidth = 300;
        private const float TileToProgressBarConstant = ProgressBarWidth * 1.0f / GridWidth;

        private TreeModel model;
        private AreaMap map;
        private MemoryTreeTile[,] gridTiles = new MemoryTreeTile[GridWidth, GridHeight];
        private MemoryTreeTile lastClicked;
        private HorizontalProgressBar progressBar;
        private Entity cursor;
        private MemoryTreeTile cursorTile;
        private bool canPlayerInteract = false;

        public MemoryChopTreeScene(AreaMap map, TreeModel model)
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
                    var gridTile = new MemoryTreeTile(x, y)
                        .Move(GridTilesXOffset + (x * TileWidth), GridTilesYOffset + (y * TileHeight));

                    gridTile.Mouse(() =>
                    {
                        if (canPlayerInteract)
                        {
                            this.OnTileSelected(gridTile as MemoryTreeTile);
                        }
                    }, TileWidth, TileHeight);

                    this.gridTiles[x, y] = gridTile as MemoryTreeTile;
                    this.Add(gridTile);
                }
            }
            
            // Make sure each column has at least one number overlapping with the adjacent one
            for (var x = 0; x < GridWidth -1; x++)
            {
                var currentColumn = new List<MemoryTreeTile>();
                var adjacentColumn = new List<MemoryTreeTile>();

                for (var y = 0; y < GridHeight; y++)
                {
                    currentColumn.Add(this.gridTiles[x, y]);
                    adjacentColumn.Add(this.gridTiles[x + 1, y]);
                }

                var isOverlap = currentColumn.Any(t => adjacentColumn.Any(t2 => t.Number == t2.Number));
                if (!isOverlap)
                {
                    // Extremely unlikely, but possible.
                    var alpha = currentColumn[random.Next(currentColumn.Count)];
                    var target = adjacentColumn[random.Next(adjacentColumn.Count)];
                    target.Number = alpha.Number;
                }
            }

            // Cancel if you hit escape.
            this.OnActionPressed = (data) =>
            {
                if (canPlayerInteract)
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
                        if (puff == PuffinAction.Up && this.cursorTile.TileY > 0)
                        {
                            this.MoveCursorTo(this.gridTiles[this.cursorTile.TileX, this.cursorTile.TileY - 1]);
                        }
                        else if (puff == PuffinAction.Down && this.cursorTile.TileY < GridHeight - 1)
                        {
                            this.MoveCursorTo(this.gridTiles[this.cursorTile.TileX, this.cursorTile.TileY + 1]);
                        }
                        else if (puff == PuffinAction.Left && this.cursorTile.TileX > 0)
                        {
                            this.MoveCursorTo(this.gridTiles[this.cursorTile.TileX - 1, this.cursorTile.TileY]);
                        }
                        else if (puff == PuffinAction.Right && this.cursorTile.TileX < GridWidth - 1)
                        {
                            this.MoveCursorTo(this.gridTiles[this.cursorTile.TileX + 1, this.cursorTile.TileY]);
                        }
                        
                    }
                }
            };

            var cancelButton = new Button("", () => HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map)))
                .Sprite(Path.Join("Content", "Images", "UI", "X-Button.png"));
            
            cancelButton.Move(HavenIslandGame.LatestInstance.Width - 40 - 16, 16);
            this.Add(cancelButton);

            this.progressBar = new HorizontalProgressBar(Path.Join("Content", "Images", "UI", "ProgressBar.png"), 0xF4B41B, ProgressBarWidth, 8, 8);
            this.progressBar.Move(GridTilesXOffset - 10, GridTilesYOffset - 72);
            this.progressBar.Value = 0;
            
            this.Add(this.progressBar);

            this.cursor = new Entity(true)
                .Sprite(Path.Combine("Content", "Images", "UI", "TileCursor.png"));
            this.Add(this.cursor);

            this.cursor.Get<SpriteComponent>().IsVisible = false;

            // Draw, sleep 1s, then hide the grid
            this.After(ShowTilesSeconds * 2, () => {
                this.HideAllTiles();
                this.cursor.Get<SpriteComponent>().IsVisible = true;
                this.MoveCursorTo(this.gridTiles[GridWidth - 1, 0]);
                this.canPlayerInteract = true;
            });
        }

        private void MoveCursorTo(MemoryTreeTile tile)
        {
            this.cursor.Move(tile.X, tile.Y);
            this.cursorTile = tile;
        }

        private void OnTileSelected(MemoryTreeTile gridTile)
        {            
            // First or second click?
            if (this.lastClicked == null)
            {
                gridTile.Show();
                this.lastClicked = gridTile;

                GameWorld.LatestInstance.PlayerEnergy -= EnergyPerClick;
                this.EventBus.Broadcast(GlobalEvents.ConsumedEnergy, gridTile.Number);

                return;
            }
            else
            {
                // Don't do anything if we didn't pick two adjacent rows
                if (Math.Abs(gridTile.TileX - lastClicked.TileX) == 1)
                {
                    gridTile.Show();
                    GameWorld.LatestInstance.PlayerEnergy -= EnergyPerClick;
                    this.EventBus.Broadcast(GlobalEvents.ConsumedEnergy, gridTile.Number);

                    // Second click: match?
                    if (gridTile.Number == this.lastClicked.Number)
                    {
                        this.After(ShowTilesSeconds, () => {
                            // Remove both rows
                            var earlierRow = Math.Min(gridTile.TileX, this.lastClicked.TileX);
                            for (var y = 0; y < GridHeight; y++)
                            {
                                this.Remove(this.gridTiles[earlierRow, y]);
                                this.Remove(this.gridTiles[earlierRow + 1, y]);
                            }

                            // Six tiles wide, so progress is 6-x for tile X we clicked on.
                            var currentProgress = (GridWidth - earlierRow) * TileToProgressBarConstant;
                            this.progressBar.Value = (int)currentProgress;

                            if (earlierRow == 0)
                            {
                                // Done
                                // TODO: should probably use events for this
                                GameWorld.LatestInstance.AreaMap.Contents.Remove(this.model);
                                HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map));
                            }
                            else
                            {
                                this.MoveCursorTo(this.gridTiles[earlierRow - 1, 0]);
                            }
                            
                            this.lastClicked = null;
                        });
                    }
                    else
                    {
                        this.lastClicked = null;
                        this.canPlayerInteract = false;
                        this.After(ShowTilesSeconds, () => {
                            this.HideAllTiles();
                            this.canPlayerInteract = true;
                        });
                    }
                }
            }
        }

        private void HideAllTiles()
        {
            foreach (var tile in this.gridTiles)
            {
                tile.Hide();
            }
        }

        private void After(int seconds, Action callback)
        {
            var timer = new Timer(seconds * 1000) { AutoReset = false };
            timer.Elapsed += (e, args) => callback.Invoke();
            timer.Start();
        }

        class MemoryTreeTile : Entity
        {
            private static Random random = new Random();
            
            public int Number { get; set; }
            public int TileX { get; private set; }
            public int TileY { get; private set; }

            public MemoryTreeTile(int tileX, int tileY) : base()
            {
                this.Number = random.Next(8) + 1; // 1-8
                this.TileX = tileX;
                this.TileY = tileY;
                this.Sprite(Path.Join("Content", "Images", "Sprites", "Tree-Texture.png"))
                    .Label($"{this.Number}", 10, -10);
                this.Get<TextLabelComponent>().FontSize = FontSize * 2;
            }

            public void Show()
            {
                this.Get<TextLabelComponent>().Text = $"{this.Number}";
            }

            public void Hide()
            {
                this.Get<TextLabelComponent>().Text = "";
            }
        }
    }
}