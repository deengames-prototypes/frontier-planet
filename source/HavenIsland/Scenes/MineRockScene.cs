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
    public class MineRockScene : Scene
    {
        private const int GRID_WIDTH = 3;
        private const int GRID_HEIGHT = 3;
        private const int TILE_WIDTH = 150;
        private const int TILE_HEIGHT = 150;
        private const int FONT_SIZE = 72;
        private const int GRID_TILES_X_OFFSET = 200;
        private const int GRID_TILES_Y_OFFSET = 100;
        private const int HIT_TILE_ENERGY_COST = 3;

        private int targetNumber;
        private int currentCorrectStreak = 0;

        private Entity label;
        private Entity streakLabel;
        private AreaMap map;
        private RockModel model;
        private RockTile[,] gridTiles = new RockTile[GRID_WIDTH, GRID_HEIGHT];

        public MineRockScene(AreaMap map, RockModel model)
        {
            this.map = map;
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

            this.streakLabel = new Entity(true).Label("");
            this.streakLabel.Get<TextLabelComponent>().FontSize = 48;
            this.Add(this.streakLabel);
            this.streakLabel.Move((int)this.label.X - 64, HavenIslandGame.LatestInstance.Height - 72);

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    var gridTile = new RockTile(x, y)
                        .Move(GRID_TILES_X_OFFSET + (x * TILE_WIDTH), GRID_TILES_Y_OFFSET + (y * TILE_HEIGHT));

                    gridTile
                        .Mouse(() => {
                            this.OnTileSelected(gridTile as RockTile);
                        }, TILE_WIDTH, TILE_HEIGHT)
                        .Keyboard((action) => {
                            // TODO: selected tile
                        });

                    this.gridTiles[x, y] = gridTile as RockTile;
                    this.Add(gridTile);
                }
            }

            // Cancel if you hit escape.
            this.OnActionPressed = (data) =>
            {
                if ((HavenIslandActions)data == HavenIslandActions.Cancel)
                {
                    HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map));
                }
            };

            var cancelButton = new Button("", () => HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map)))
                .Sprite(Path.Join("Content", "Images", "UI", "X-Button.png"));
            
            cancelButton.Move(HavenIslandGame.LatestInstance.Width - 40 - 16, 16);
            this.Add(cancelButton);

            this.PickTargetNumber();
            this.UpdateTargetLabel();
        }

        private void PickTargetNumber()
        {
            this.targetNumber = 0;
            while (this.targetNumber <= 0)
            {
                // Pick a random tile on-board.
                // Source: https://stackoverflow.com/questions/15884285/get-a-random-value-from-a-two-dimensional-array
                int values = this.gridTiles.GetLength(0) * this.gridTiles.GetLength(1);
                int index = new Random().Next(values);
                var tile = this.gridTiles[index / this.gridTiles.GetLength(0), index % this.gridTiles.GetLength(0)];
                
                this.targetNumber = tile.Integrity;
            }
            this.UpdateTargetLabel();
        }

        private void OnTileSelected(RockTile gridTile)
        {
            // Integrity decreases in the ForEach below
            if (gridTile.Integrity == this.targetNumber)
            {
                this.currentCorrectStreak++;
                if (this.currentCorrectStreak > 1)
                {
                    int bonus = this.currentCorrectStreak - 1;
                    gridTile.Integrity -= bonus;
                    this.streakLabel.Get<TextLabelComponent>().Text = $"Precise mining bonus - {this.currentCorrectStreak} in a row!";
                }
            }
            else
            {
                this.currentCorrectStreak = 0;
                this.streakLabel.Get<TextLabelComponent>().Text = "";
            }

            foreach (var tile in this.GetNonDeadTilesAround(gridTile))
            {
                // -2 integrity for correct tiles
                if (tile.Integrity == targetNumber)
                {
                    tile.Integrity -= 1;
                }

                // All tiles in area get hurt
                tile.Integrity -= 1;
                tile.Get<TextLabelComponent>().Text = $"{tile.Integrity}";
            }

            var tilesLeft = 0;
            for (var y = 0; y < GRID_HEIGHT; y++)
            {
                for (var x = 0; x < GRID_WIDTH; x++)
                {
                    var tile = this.gridTiles[x, y];
                    if (tile.Integrity <= 0)
                    {
                        this.Remove(tile);
                    }
                    else
                    {
                        tilesLeft++;
                    }
                }
            }

            EventBus.LatestInstance.Broadcast(GlobalEvents.ConsumedEnergy, HIT_TILE_ENERGY_COST);
            
            if (tilesLeft <= 0)
            {
                GameWorld.LatestInstance.AreaMap.Contents.Remove(this.model);                
                HavenIslandGame.LatestInstance.ShowScene(new MapScene(this.map));
            }
            else
            {
                this.PickTargetNumber();
            }
        }

        private void UpdateTargetLabel()
        {
            this.label.Get<TextLabelComponent>().Text = $"Weak spots: {this.targetNumber}";
        }

        private List<RockTile> GetNonDeadTilesAround(RockTile root)
        {
            // Includes root
            var x = root.Coordinates.Item1;
            var y = root.Coordinates.Item2;

            var toReturn = new List<RockTile> { root };
            
            if (x > 0)
            {
                toReturn.Add(this.gridTiles[x - 1, y]);
            }
            if (x < GRID_WIDTH - 1)
            {
                toReturn.Add(this.gridTiles[x + 1, y]);
            }

            if (y > 0)
            {
                toReturn.Add(this.gridTiles[x, y - 1]);
            }
            if (y < GRID_HEIGHT - 1)
            {
                toReturn.Add(this.gridTiles[x, y + 1]);
            }
            
            return toReturn.Where(t => t.Integrity > 0).ToList();
        }

        class RockTile : Entity
        {
            private static Random random = new Random();
            public int Integrity { get; set; }
            public readonly Tuple<int, int> Coordinates;

            public RockTile(int x, int y) : base()
            {
                this.Coordinates = new Tuple<int, int>(x, y);
                this.Integrity = 3 + random.Next(5); // 3-7
                this.Sprite(Path.Join("Content", "Images", "Sprites", "Rock-Texture.png"))
                    .Label($"{this.Integrity}", 40, 0);
                this.Get<TextLabelComponent>().FontSize = FONT_SIZE * 2;
            }
        }
    }
}