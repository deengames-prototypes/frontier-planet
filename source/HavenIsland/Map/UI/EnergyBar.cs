using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using DeenGames.HavenIsland.Map.Entities;
using Puffin.Core;
using Puffin.Core.Ecs;
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
            this.Move(HavenIslandGame.LatestInstance.Width - PADDING - WIDTH, HavenIslandGame.LatestInstance.Height - this.Height - PADDING);

            EventBus.LatestInstance.Subscribe(GlobalEvents.ConsumedEnergy, (amount) =>
            {
                this.Height -= (int)amount;
                this.Colour(0xf4b41b, WIDTH, Height);
            });
        }
    }
}