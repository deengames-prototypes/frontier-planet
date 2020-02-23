using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Player : Entity
    {
        internal static Player LatestInstance { get; private set; }

        public Player()
        {
            Player.LatestInstance = this;

            this.Spritesheet(Path.Combine("Content", "Images", "Characters", "Protagonist.png"), 26, 32)
                .FourWayMovement(100)
                .Collide(26, 16, true, 0, 16)
                .Overlap(26, 16, 0, 8);

            EventBus.LatestInstance.Subscribe(MapEvents.ChoppedDownTree, (obj) => this.SubtractEnergy(MapEvents.ChoppedDownTree));
            EventBus.LatestInstance.Subscribe(MapEvents.MinedRock, (obj) => this.SubtractEnergy(MapEvents.MinedRock));
        }

        public void SubtractEnergy(MapEvents m)
        {
            var cost = EnergyCost(m);
            this.SubtractEnergy(cost);
        }

        public void SubtractEnergy(int cost)
        {
            GameWorld.Instance.PlayerEnergy -= cost;
            EventBus.LatestInstance.Broadcast(GlobalEvents.ConsumedEnergy, cost);
        }

        public static int EnergyCost(MapEvents m)
        {
            switch (m)
            {
                case MapEvents.ChoppedDownTree:
                    return 10;
                case MapEvents.MinedRock:
                    return 10;
                default:
                    return 0;
            }
        }
    }
}