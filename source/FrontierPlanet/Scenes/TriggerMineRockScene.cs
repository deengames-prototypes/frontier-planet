using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Map.Entities;
using DeenGames.FrontierPlanet.Map.UI;
using DeenGames.FrontierPlanet.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using Puffin.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeenGames.FrontierPlanet.Scenes
{
    public class TriggerMineRockScene : Scene
    {
        private const int FontSize = 48;
        private const int ArrowVelocity = 500;
        private int maxArrowX;
        private int minArrowX;
        private bool isOnStreak = false;
        private int rocksGained = 0;
        
        private Entity rocksGainedLabel;

        // Trigger bar
        private Entity triggerBar = new Entity();
        private Entity hitArea = new Entity();
        private Entity triggerArrow = new Entity();
        private bool arrowMovingRight = true;

        private AreaMap map;
        private RockModel model;

        public TriggerMineRockScene(AreaMap map, RockModel model)
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

            this.rocksGainedLabel = new Entity(true).Label("Mined 0 rocks");
            this.rocksGainedLabel.Get<TextLabelComponent>().FontSize = FontSize;
            this.Add(this.rocksGainedLabel);
            this.rocksGainedLabel.Move(300, 50);

            this.triggerBar = new Entity().Sprite(Path.Combine("Content", "Images", "Sprites", "Trigger-Bar.png"))
                .Move(this.rocksGainedLabel.X, this.rocksGainedLabel.Y + 100);
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
                if ((FrontierPlanetActions)data == FrontierPlanetActions.Cancel)
                {
                    FrontierPlanetGame.LatestInstance.ShowScene(new MapScene(this.map));
                }
            };

            var cancelButton = new Button("", () => FrontierPlanetGame.LatestInstance.ShowScene(new MapScene(this.map)))
                .Sprite(Path.Join("Content", "Images", "UI", "X-Button.png"));
            
            cancelButton.Move(FrontierPlanetGame.LatestInstance.Width - 40 - 16, 16);
            this.Add(cancelButton);
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

        private void UpdateGainedLabel()
        {
            this.rocksGainedLabel.Get<TextLabelComponent>().Text = $"Mined {this.rocksGained} rocks";
        }
    }
}