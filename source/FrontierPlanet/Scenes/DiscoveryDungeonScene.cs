using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeenGames.FrontierPlanet.Model.DiscoveryDungeon;
using DeenGames.FrontierPlanet.Model.Maps;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Tiles;

namespace DeenGames.FrontierPlanet.Scenes
{
    class DiscoveryDungeonScene : Scene
    {
        // At 2x zoom, 8x8 dungeon barely fits on-screen.
        private const int TileSize = 32;
        TileMap fogTilemap;
        TileMap contentsTilemap;

        private DiscoveryDungeon dungeon;
        private PlayerModel player;
        private const int InteractWithTileEnergyCost = 2;

        private Entity healthIndicator;        

        public DiscoveryDungeonScene(PlayerModel player)
        {
            this.dungeon = new DiscoveryDungeon(1);
            this.player = player;
        }

        override public void Ready()
        {
            base.Ready();

            var dungeonWidth = (DiscoveryDungeon.TilesWide * TileSize);
            var dungeonHeight = (DiscoveryDungeon.TilesHigh * TileSize);
            var screenWidth = (FrontierPlanetGame.LatestInstance.Width);
            var screenHeight = (FrontierPlanetGame.LatestInstance.Height);

            var centerX =  (int)((screenWidth - dungeonWidth) / (2 * Constants.GameZoom));
            var centerY = (int)((screenHeight - dungeonHeight) / (2 * Constants.GameZoom));
            // Spent about an hour on this and couldn't figure out the right formula
            // These are derived by using GIMP to measure the left/right space, divide by 2,
            // then divide by 2 again to get the right answer (second divide by 2 = zoom);
            centerX = 192;
            centerY = 56;

            var groundTilemap = new TileMap(DiscoveryDungeon.TilesWide, DiscoveryDungeon.TilesHigh, Path.Combine("Content", "Images", "Tilesets", "Dungeon.png"), TileSize, TileSize) { X = centerX, Y = centerY };
            contentsTilemap = new TileMap(DiscoveryDungeon.TilesWide, DiscoveryDungeon.TilesHigh, Path.Combine("Content", "Images", "Tilesets", "Dungeon.png"), TileSize, TileSize) { X = centerX, Y = centerY };
            fogTilemap = new TileMap(DiscoveryDungeon.TilesWide, DiscoveryDungeon.TilesHigh, Path.Combine("Content", "Images", "Tilesets", "Dungeon.png"), TileSize, TileSize) { X = centerX, Y = centerY };

            this.Add(groundTilemap);
            this.Add(contentsTilemap);
            this.Add(fogTilemap);

            fogTilemap.Define("Fog", 1, 0);

            groundTilemap.Define("Floor", 2, 0);

            contentsTilemap.Define("Stairs", 3, 0);
            contentsTilemap.Define("Monster", 0, 1); // generic monster
            contentsTilemap.Define("Treasure", 1, 1); 
            contentsTilemap.Define("Item", 2, 1); 
            contentsTilemap.Define("Alien", 3, 1); // alien

            (int, int) startPosition = (-1, -1);
            for (var y = 0; y < DiscoveryDungeon.TilesHigh; y++)
            {
                for (var x = 0; x < DiscoveryDungeon.TilesWide; x++)
                {
                    groundTilemap.Set(x, y, "Floor");
                    this.UpdateContentsDisplay(x, y);
                    if (dungeon.IsVisible(x, y))
                    {
                        startPosition = (x, y);
                    }
                }   
            }

            this.OnTileClicked(startPosition.Item1, startPosition.Item2);

            this.OnMouseClick = () => {
                // Math.Floor used here to prevent (-1/32) => 0
                // 0d + is here because Math.Floor is ambiguous
                var tileX = (int)Math.Floor((0d + this.MouseCoordinates.Item1 - fogTilemap.X) / TileSize);
                var tileY = (int)Math.Floor((0d + this.MouseCoordinates.Item2 - fogTilemap.Y) / TileSize);
                if (tileX >= 0 && tileX < DiscoveryDungeon.TilesWide && tileY >= 0 && tileY < DiscoveryDungeon.TilesHigh)
                {
                    this.OnTileClicked(tileX, tileY);
                }
            };

            this.Add(new Entity().Camera(Constants.GameZoom));

            // TODO: add progress bar
            this.healthIndicator = new Entity(true)
                .Label($"Health: {this.player.Health}/{this.player.MaxHealth}")
                // 2x => 2 rows of text
                .Move(8, FrontierPlanetGame.LatestInstance.Height - (2 * FrontierPlanetGame.DefaultFontSize));
            this.healthIndicator.Get<TextLabelComponent>().FontSize = FrontierPlanetGame.DefaultFontSize;
            
            this.Add(this.healthIndicator);
        }

        private void UpdateContentsDisplay(int x, int y)
        {
            if (!dungeon.IsVisible(x, y))
            {
                fogTilemap.Set(x, y, "Fog");
            }
            else
            {
                fogTilemap.Set(x, y, null);
            }

            var contents = dungeon.Contents(x, y);
            if (contents != null)
            {
                contentsTilemap.Set(x, y, contents.Sprite);
            }
        }

        private void OnTileClicked(int tileX, int tileY)
        {
            // You can't reveal tiles by clicking on stuff
            if (this.dungeon.IsVisible(tileX, tileY))
            {
                if (this.dungeon.Contents(tileX, tileY) == null)
                {
                    // Reveal
                    var adjacents = GetAdjacents(tileX, tileY);
                    if (adjacents.Any())
                    {
                        this.player.SubtractEnergy(InteractWithTileEnergyCost);
                        foreach (var revealed in adjacents)
                        {
                            var x = revealed.Item1;
                            var y = revealed.Item2;
                            if (!this.dungeon.IsVisible(x, y))
                            {
                                this.dungeon.Reveal(x, y);
                                this.UpdateContentsDisplay(x, y);
                            }
                        }
                    }
                }
                else
                {
                    // Interact
                }
            }
        }

        private List<(int, int)> GetAdjacents(int tileX, int tileY)
        {
            var toReturn = new List<(int, int)>();

            if (tileX > 0)
            {
                toReturn.Add((tileX - 1, tileY));
            }
            if (tileX < DiscoveryDungeon.TilesWide - 1)
            {
                toReturn.Add((tileX + 1, tileY));
            }

            if (tileY > 0)
            {
                toReturn.Add((tileX, tileY - 1));
            }
            if (tileY < DiscoveryDungeon.TilesHigh - 1)
            {
                toReturn.Add((tileX, tileY + 1));
            }

            toReturn.RemoveAll((coordinates)  => this.dungeon.IsVisible(coordinates.Item1, coordinates.Item2));

            return toReturn;
        }
    }
}