using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private const int SnipeEnergyCost = 5;

        // UI
        private Entity healthIndicator;
        private Entity blackout;
        private DateTime? blackoutStart = null;
        private bool isGameOver = false;
        private bool snipeNextMonster = false;

        public DiscoveryDungeonScene(PlayerModel player)
        {
            this.dungeon = new DiscoveryDungeon(1, player);
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
            contentsTilemap.Define("Alien", 0, 1);
            contentsTilemap.Define("Bomb", 1, 1); 
            contentsTilemap.Define("Heal", 2, 1); 
            contentsTilemap.Define("EnergyBoost", 3, 1);
            contentsTilemap.Define("Monster", 0, 2);

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

            /////// UI
            this.Add(new Entity().Camera(Constants.GameZoom));
            
            // TODO: add progress bar
            this.healthIndicator = new Entity(true)
                .Label("")
                // One row of text
                .Move(8, FrontierPlanetGame.LatestInstance.Height - FrontierPlanetGame.DefaultFontSize);
            this.healthIndicator.Get<TextLabelComponent>().FontSize = FrontierPlanetGame.DefaultFontSize;
            
            this.Add(this.healthIndicator);

            var skillButton = new Entity(true).Sprite(Path.Combine("Content", "Images", "Sprites", "Dungeon-Snipe.png"))
                .Move(8, this.healthIndicator.Y - FrontierPlanetGame.DefaultFontSize);
            
            skillButton.Mouse(() =>
            {
                if (player.Energy >= SnipeEnergyCost)
                {
                    this.healthIndicator.Get<TextLabelComponent>().Text = "Click a monster to snipe it.";
                    this.snipeNextMonster = true;
                    player.SubtractEnergy(SnipeEnergyCost);

                    if (this.player.Energy <= 0)
                    {
                        this.TriggerGameOver("Out of energy!");
                    }
                }
                else
                {
                    this.UpdateHealthDisplay();
                    this.healthIndicator.Get<TextLabelComponent>().Text += "     Not enough energy to snipe.";
                }
            }, 32, 32);

            this.Add(skillButton);

            this.UpdateHealthDisplay();

            this.blackout = new Entity(true)
                .Colour(0x000000, FrontierPlanetGame.LatestInstance.Width, FrontierPlanetGame.LatestInstance.Height)
                .Label("", 500, 300);

            this.blackout.Get<TextLabelComponent>().FontSize = 72;
            this.blackout.Get<ColourComponent>().Alpha = 0;
            
            // Clear sight around start tile
            this.Reveal(startPosition.Item1, startPosition.Item2);
        }

        override public void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);

            if (blackoutStart != null)
            {
                var timeSinceBlackout = (float)(DateTime.Now - blackoutStart.Value).TotalSeconds;
                var alpha = timeSinceBlackout;
                this.blackout.Get<ColourComponent>().Alpha = alpha;
                
                // If it's at least 2s, we can quit/return.
            }
        }

        private void UpdateHealthDisplay()
        {
            this.healthIndicator.Get<TextLabelComponent>().Text = $"Health: {this.player.Health}/{this.player.MaxHealth} Energy: {this.player.Energy}/{this.player.MaxEnergy}";
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
            else
            {
                contentsTilemap.Set(x, y, null);
            }
        }

        private void OnTileClicked(int tileX, int tileY)
        {
            if (isGameOver)
            {
                return;
            }

            // You can't reveal tiles by clicking on stuff
            if (this.dungeon.IsVisible(tileX, tileY))
            {
                this.UpdateHealthDisplay(); // clear dead monster health

                if (this.dungeon.Contents(tileX, tileY) == null)
                {
                    // Reveal
                    bool revealedAnything = this.Reveal(tileX, tileY);
                    if (revealedAnything)
                    {
                        this.player.SubtractEnergy(InteractWithTileEnergyCost);
                        this.UpdateHealthDisplay();

                        if (this.player.Energy <= 0)
                        {
                            this.TriggerGameOver("You are out of energy!");
                        }
                    }
                }
                else
                {
                    // Interact
                    var contents = this.dungeon.Contents(tileX, tileY);
                    if (contents is DungeonMonster)
                    {
                        var monster = contents as DungeonMonster;
                        this.dungeon.AttackMonsterAt(tileX, tileY, snipeNextMonster);
                        this.UpdateHealthDisplay();
                        this.healthIndicator.Get<TextLabelComponent>().Text += $"     Monster: {monster.Health}/{monster.MaxHealth}";
                        
                        if (monster.Health <= 0)
                        {
                            this.contentsTilemap.Set(tileX, tileY, null);
                        }

                        if (this.player.Health <= 0)
                        {
                            this.TriggerGameOver("You Died!");
                        }

                        this.snipeNextMonster = false;
                    }
                    else
                    {
                        this.dungeon.ConsumeItemAt(tileX, tileY);
                        this.UpdateHealthDisplay();

                        // Bombs affect all visible tiles, so we need to update potentially everything.
                        if (this.contentsTilemap.Get(tileX, tileY) == "Bomb")
                        {
                            for (var y = 0; y < DiscoveryDungeon.TilesHigh; y++)
                            {
                                for (var x = 0; x < DiscoveryDungeon.TilesWide; x++)
                                {
                                    this.UpdateContentsDisplay(x, y);
                                }
                            }
                        }

                        this.contentsTilemap.Set(tileX, tileY, null);
                    }
                }
            }
        }

        private void TriggerGameOver(string message)
        {
            this.blackout.Get<TextLabelComponent>().Text = message;
            this.Add(this.blackout);
            blackoutStart = DateTime.Now;
            this.isGameOver = true;
        }

        private bool Reveal(int tileX, int tileY)
        {
            // Returns true if we revealed anything
            var adjacents = GetAdjacents(tileX, tileY);
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

            return adjacents.Any();
        }

        private List<(int, int)> GetAdjacents(int tileX, int tileY)
        {
            var toReturn = new List<(int, int)>();

            toReturn.Add((tileX, tileY));

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

            toReturn.RemoveAll((coordinates) => this.dungeon.IsVisible(coordinates.Item1, coordinates.Item2));

            return toReturn;
        }
    }
}