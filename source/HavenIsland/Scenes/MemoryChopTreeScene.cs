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
        private const int GRID_WIDTH = 5;
        private const int GRID_HEIGHT = 5;
        private const int TILE_WIDTH = 60;
        private const int TILE_HEIGHT = 60;
        private const int FONT_SIZE = 36;
        private const int GRID_TILES_X_OFFSET = 300;
        private const int GRID_TILES_Y_OFFSET = 100;
        private const int EnergyPerClick = 3;
        private const int ShowTilesSeconds = 1;

        private TreeModel model;
        private AreaMap map;
        private MemoryTreeTile[,] gridTiles = new MemoryTreeTile[GRID_WIDTH, GRID_HEIGHT];
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

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var gridTile = new MemoryTreeTile(x, y)
                        .Move(GRID_TILES_X_OFFSET + (x * TILE_WIDTH), GRID_TILES_Y_OFFSET + (y * TILE_HEIGHT));

                    gridTile.Mouse(() =>
                    {
                        if (canPlayerInteract)
                        {
                            this.OnTileSelected(gridTile as MemoryTreeTile);
                        }
                    }, TILE_WIDTH, TILE_HEIGHT);

                    this.gridTiles[x, y] = gridTile as MemoryTreeTile;
                    this.Add(gridTile);
                }
            }
            
            // Make sure each column has at least one number overlapping with the adjacent one
            for (var x = 0; x < GRID_WIDTH -1; x++)
            {
                var currentColumn = new List<MemoryTreeTile>();
                var adjacentColumn = new List<MemoryTreeTile>();

                for (var y = 0; y < GRID_HEIGHT; y++)
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
                        else if (puff == PuffinAction.Down && this.cursorTile.TileY < GRID_HEIGHT - 1)
                        {
                            this.MoveCursorTo(this.gridTiles[this.cursorTile.TileX, this.cursorTile.TileY + 1]);
                        }
                        else if (puff == PuffinAction.Left && this.cursorTile.TileX > 0)
                        {
                            this.MoveCursorTo(this.gridTiles[this.cursorTile.TileX - 1, this.cursorTile.TileY]);
                        }
                        else if (puff == PuffinAction.Right && this.cursorTile.TileX < GRID_WIDTH - 1)
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

            this.progressBar = new HorizontalProgressBar(Path.Join("Content", "Images", "UI", "ProgressBar.png"), 0xF4B41B, 300, 8, 8);
            this.progressBar.Move(GRID_TILES_X_OFFSET - 10, GRID_TILES_Y_OFFSET - 72);
            this.progressBar.Value = 0;
            
            this.Add(this.progressBar);

            this.cursor = new Entity(true)
                .Sprite(Path.Combine("Content", "Images", "UI", "TileCursor.png"));
            this.Add(this.cursor);

            this.cursor.Get<SpriteComponent>().IsVisible = false;

            // Draw, sleep 1s, then hide the grid
            var showTimer = new Timer(ShowTilesSeconds * 1000);
            showTimer.Elapsed += (e, args) => {
                this.HideAllTiles();
                this.cursor.Get<SpriteComponent>().IsVisible = true;
                this.MoveCursorTo(this.gridTiles[GRID_WIDTH - 1, 0]);
                this.canPlayerInteract = true;
            };
            showTimer.AutoReset = false;
            showTimer.Start();
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
                return;
            }
            else
            {
                // Don't do anything if we didn't pick two adjacent rows
                if (Math.Abs(gridTile.TileX - lastClicked.TileX) == 1)
                {
                    gridTile.Show();
                    // Second click: match?
                    if (gridTile.Number == this.lastClicked.Number)
                    {
                        // Remove both rows
                        var earlierRow = Math.Min(gridTile.TileX, this.lastClicked.TileX);
                        for (var y = 0; y < GRID_HEIGHT; y++)
                        {
                            this.Remove(this.gridTiles[earlierRow, y]);
                            this.Remove(this.gridTiles[earlierRow + 1, y]);
                        }

                        // Five tiles wide, so progress is 5-x for tile X we clicked on.
                        var currentProgress = GRID_WIDTH - (earlierRow * TILE_WIDTH);
                        this.progressBar.Value = currentProgress;

                        GameWorld.LatestInstance.PlayerEnergy -= EnergyPerClick;
                        this.EventBus.Broadcast(GlobalEvents.ConsumedEnergy, gridTile.Number);

                        if (false)
                        {
                            // Done
                            // TODO: should probably use events for this
                            GameWorld.LatestInstance.AreaMap.Contents.Remove(this.model);
                            HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map));
                        }
                    }
                    else
                    {
                        this.canPlayerInteract = false;
                        var timer = new Timer(ShowTilesSeconds * 1000) { AutoReset = false };
                        timer.Elapsed += (e, args) => {
                            this.HideAllTiles();
                            this.canPlayerInteract = true;
                        };
                        timer.Start();
                    }

                    this.lastClicked = null;
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
                this.Get<TextLabelComponent>().FontSize = FONT_SIZE * 2;
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