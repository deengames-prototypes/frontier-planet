using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using DeenGames.FrontierPlanet.Map.Entities;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using System.IO;
using System;

namespace DeenGames.FrontierPlanet.Map.UI
{
    class EnergyBar : Entity
    {
        internal const int WIDTH = 16;
        internal int Height { get; set; } = GameWorld.LatestInstance.PlayerEnergy;
        private const int PADDING = 16; // relative to the bottom-right of the parent window
        private const int BAR_PADDING = 4; // Relative to backing PNG

        public EnergyBar(EventBus eventBus) : base(true)
        {
            // TODO: show/hide label on mouse over/out
            this.Sprite(Path.Combine("Content", "Images", "UI", "EnergyBar.png"));
            this.Colour(0xf4b41b, WIDTH, Height, BAR_PADDING, this.CalculateOffsetY());
            this.Move(
                FrontierPlanetGame.LatestInstance.Width - PADDING - WIDTH,
                FrontierPlanetGame.LatestInstance.Height - PADDING - GameWorld.LatestInstance.PlayerMaxEnergy);
            var text = this.Label("");

            eventBus.Subscribe(GlobalEvents.ConsumedEnergy, (amount) =>
            {
                var colour = this.Get<ColourComponent>();
                colour.Height = GameWorld.LatestInstance.PlayerEnergy;
                colour.OffsetY = this.CalculateOffsetY();
            });
        }

        private int CalculateOffsetY()
        {
            // Move the bar so that consuming energy diminishes it from the top of the bar
            return BAR_PADDING + (GameWorld.LatestInstance.PlayerMaxEnergy - GameWorld.LatestInstance.PlayerEnergy);
        }
    }
}