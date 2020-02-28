using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using DeenGames.HavenIsland.Map.Entities;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Ecs.Components;
using Puffin.Core.Events;

namespace DeenGames.HavenIsland.Map.UI
{
    class EnergyBar : Entity
    {
        internal const int WIDTH = 16;
        internal int Height { get; set; } = GameWorld.LatestInstance.PlayerEnergy;
        private const int PADDING = 16;

        public EnergyBar() : base(true)
        {
            // TODO: probably backed by a PNG
            // TODO: show/hide label on mouse over/out
            this.Colour(0xf4b41b, WIDTH, Height);            
            var text = this.Label("");
            this.UpdatePosition();

            EventBus.LatestInstance.Subscribe(GlobalEvents.ConsumedEnergy, (amount) =>
            {
                this.Height = GameWorld.LatestInstance.PlayerEnergy;
                this.UpdatePosition();
                this.Colour(0xf4b41b, WIDTH, this.Height);
            });
        }

        private void UpdatePosition()
        {
            var energyDiff = (GameWorld.LatestInstance.PlayerMaxEnergy - GameWorld.LatestInstance.PlayerEnergy);
            this.Move(HavenIslandGame.LatestInstance.Width - PADDING - WIDTH,
                HavenIslandGame.LatestInstance.Height - (GameWorld.LatestInstance.PlayerMaxEnergy - energyDiff) - PADDING);
        }
    }
}