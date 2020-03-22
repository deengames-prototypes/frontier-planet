using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;
using System.IO;

namespace DeenGames.FrontierPlanet.Map.UI
{
    // An image (background) with a bar (colour). The colour height changes to the current energy.
    // It also uses some math to look like it's decreasing top-down instead of bottom-up.
    class EnergyBar : Entity
    {
        internal const int WIDTH = 16;
        private const int PADDING = 16; // relative to the bottom-right of the parent window
        private const int BAR_PADDING = 4; // Relative to backing PNG
        private PlayerModel player;

        public EnergyBar(EventBus eventBus, PlayerModel player) : base(true)
        {
            this.player = player;
            
            // TODO: show/hide label on mouse over/out
            this.Sprite(Path.Combine("Content", "Images", "UI", "EnergyBar.png"));
            this.Colour(0xf4b41b, WIDTH, this.player.Energy, BAR_PADDING, this.CalculateOffsetY());
            this.Move(
                FrontierPlanetGame.LatestInstance.Width - PADDING - WIDTH,
                FrontierPlanetGame.LatestInstance.Height - PADDING - this.player.MaxEnergy);
            var text = this.Label("");

            eventBus.Subscribe(GlobalEvents.ConsumedEnergy, (amount) =>
            {
                var colour = this.Get<ColourComponent>();
                colour.Height = this.player.Energy;
                colour.OffsetY = this.CalculateOffsetY();
            });
        }

        private int CalculateOffsetY()
        {
            // Move the bar so that consuming energy diminishes it from the top of the bar
            var toReturn = BAR_PADDING + (this.player.MaxEnergy - this.player.Energy);
            System.Console.WriteLine($"Offset={toReturn}!");
            return toReturn;
        }
    }
}