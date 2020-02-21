using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Map.Entities;
using Puffin.Core.Ecs;
using Puffin.Core.Events;

namespace DeenGames.HavenIsland.Map.UI
{
    class EnergyBar : Entity
    {
        internal readonly int Width = 8;
        internal int Height { get; set; } = Player.LatestInstance.Energy;

        public EnergyBar() : base(true)
        {
            // TODO: probably backed by a PNG
            // TODO: show/hide label on mouse over/out
            this.Colour(0xf4b41b, Width, Height);
            EventBus.LatestInstance.Subscribe(MapEvent.ChoppedDownTree, (obj) =>
            {
                this.Height -= Player.EnergyCost(MapEvent.ChoppedDownTree);
                this.Colour(0xf4b41b, Width, Height);
            });
        }
    }
}