using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Map.UI;
using DeenGames.FrontierPlanet.Model;
using DeenGames.FrontierPlanet.Model.Maps;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.UI.Controls;
using System;
using System.IO;

namespace DeenGames.FrontierPlanet.Scenes
{
    public class TriggerMineRockScene : Scene
    {
        private const int FontSize = 48;
        private readonly int ArrowVelocity = 200 / Options.RockMiningSpeedMultiplier;

        private const int RocksGainedOnMiss = 1;
        private const int RocksGainedOnHit = 2;
        private const int RocksGainedOnBonus = 3;
        private const int EnergyPerClick = 2;
        private int integrityLeft; // higher on harder/bigger rocks

        private int maxArrowX;
        private int minArrowX;
        private bool isOnStreak = false;
        private int rocksGained = 0;
        
        private Entity rocksGainedLabel;
        private Entity integrityLabel;

        // Trigger bar
        private Entity triggerBar = new Entity();
        private Entity hitArea = new Entity();
        private Entity triggerArrow = new Entity();
        private bool arrowMovingRight = true;

        private AreaMap map;
        private RockModel model;
        private PlayerModel player;

        public TriggerMineRockScene(AreaMap map, PlayerModel player, RockModel model)
        {
            this.map = map;
            this.model = model;
            this.player = player;
            this.integrityLeft = new Random().Next(4, 6);
        }

        override public void Ready()
        {
            base.Ready();
            var random = new Random();

            this.BackgroundColour = 0x397b44;
            this.Add(new EnergyBar(this.EventBus, this.player));

            this.rocksGainedLabel = new Entity(true).Label("");
            this.rocksGainedLabel.Get<TextLabelComponent>().FontSize = FontSize;
            this.Add(this.rocksGainedLabel);
            this.rocksGainedLabel.Move(300, 50);

            this.integrityLabel = new Entity(true).Label("");
            this.integrityLabel.Get<TextLabelComponent>().FontSize = FontSize;
            this.Add(this.integrityLabel);
            this.integrityLabel.Move(this.rocksGainedLabel.X, this.rocksGainedLabel.Y + FontSize);

            this.triggerBar = new Entity().Sprite(Path.Combine("Content", "Images", "Sprites", "Trigger-Bar.png"))
                .Move(this.integrityLabel.X, this.integrityLabel.Y + 100);
            // Starts roughly in the middle of the bar
            this.hitArea = new Entity().Sprite(Path.Combine("Content", "Images", "Sprites", "Trigger-Bar-Hit-Area.png"))
                .Move(this.triggerBar.X + 250, this.triggerBar.Y);
            this.triggerArrow = new Entity().Sprite(Path.Combine("Content", "Images", "Sprites", "Trigger-Bar-Arrow.png"))
                .Move(this.triggerBar.X, this.triggerBar.Y - 32);

            this.Add(this.triggerBar);
            this.Add(this.hitArea);
            this.Add(this.triggerArrow);

            maxArrowX = (int)(this.triggerBar.X + this.triggerBar.Get<SpriteComponent>().Width - this.triggerArrow.Get<SpriteComponent>().Width);
            minArrowX = (int)(this.triggerBar.X - this.triggerArrow.Get<SpriteComponent>().Width);

            // Cancel if you hit escape.
            this.OnActionPressed = (data) =>
            {
                if (data is FrontierPlanetActions)
                {
                    var action = (FrontierPlanetActions)data;
                    if (action == FrontierPlanetActions.Cancel)
                    {
                        FrontierPlanetGame.LatestInstance.ShowScene(new MapScene(this.map, this.player));
                    }
                    else if (action == FrontierPlanetActions.Interact)
                    {
                        this.CheckTrigger();
                    }
                }
            };
            
            this.UpdateGainedLabel();
            this.OnMouseClick = () => this.CheckTrigger();

            var cancelButton = new Button("", () => FrontierPlanetGame.LatestInstance.ShowScene(new MapScene(this.map, this.player)))
                .Sprite(Path.Join("Content", "Images", "UI", "X-Button.png"));
            this.Add(cancelButton);
            
            cancelButton.Move(FrontierPlanetGame.LatestInstance.Width - cancelButton.Get<SpriteComponent>().Width - 16, 16);
        }

        override public void Update(float elapsedSeconds)
        {
            base.Update(elapsedSeconds);
            if (this.arrowMovingRight && this.triggerArrow.X < maxArrowX)
            {
                this.triggerArrow.X += ArrowVelocity * elapsedSeconds;
                if (this.triggerArrow.X >= maxArrowX)
                {
                    this.arrowMovingRight = !this.arrowMovingRight;
                    this.triggerArrow.X = maxArrowX;
                }
            }
            else if (!this.arrowMovingRight && this.triggerArrow.X > minArrowX)
            {
                this.triggerArrow.X -= ArrowVelocity * elapsedSeconds;
                if (this.triggerArrow.X <= minArrowX)
                {
                    this.arrowMovingRight = !this.arrowMovingRight;
                    this.triggerArrow.X = minArrowX;
                }
            }
        }

        private void CheckTrigger()
        {
            this.player.SubtractEnergy(EnergyPerClick);
            this.EventBus.Broadcast(GlobalEvents.ConsumedEnergy, EnergyPerClick);
            this.integrityLeft--;

            var minCorrectX = this.hitArea.X;
            var maxCorrectX = this.hitArea.X + this.hitArea.Get<SpriteComponent>().Width - this.triggerArrow.Get<SpriteComponent>().Width;

            var isSuccess = this.triggerArrow.X >= minCorrectX && this.triggerArrow.X <= maxCorrectX;
            if (isSuccess)
            {
                if (this.isOnStreak)
                {
                    this.rocksGained += RocksGainedOnBonus;
                }
                else
                {
                    this.rocksGained += RocksGainedOnHit;
                }
                
                this.isOnStreak = true;
            }
            else
            {
                this.rocksGained += RocksGainedOnMiss;
                this.isOnStreak = false;
            }

            this.UpdateGainedLabel();

            // Center hit area on the arrow
            this.hitArea.X = this.triggerArrow.X + (this.triggerArrow.Get<SpriteComponent>().Width / 2) - (this.hitArea.Get<SpriteComponent>().Width / 2);
            
            var maxX = this.triggerBar.X + this.triggerBar.Get<SpriteComponent>().Width - this.hitArea.Get<SpriteComponent>().Width;
            
            if (this.hitArea.X < this.triggerBar.X)
            {
                this.hitArea.X = this.triggerBar.X;
            }
            else if (this.hitArea.X > maxX)
            {
                this.hitArea.X = maxX;
            }

            // Reset position
            this.triggerArrow.X = this.triggerBar.X;

            if  (this.integrityLeft <= 0)
            {
                // Done
                GameWorld.LatestInstance.AreaMap.Contents.Remove(this.model);
                FrontierPlanetGame.LatestInstance.ShowScene(new MapScene(this.map, this.player));
            }
        }

        private void UpdateGainedLabel()
        {
            this.rocksGainedLabel.Get<TextLabelComponent>().Text = $"Mined {this.rocksGained} rocks";
            this.integrityLabel.Get<TextLabelComponent>().Text = $"Rock integrity: {this.integrityLeft}";
        }
    }
}