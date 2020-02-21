using DeenGames.HavenIsland.Events;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Player : Entity
    {
        internal static Player LatestInstance { get; private set; }

        public int Energy { get; private set; }
        public readonly int MaxEnergy = 100;
        
        public Player()
        {
            Player.LatestInstance = this;
            this.Energy = this.MaxEnergy;

            this.Spritesheet(Path.Combine("Content", "Images", "Characters", "Protagonist.png"), 26, 32)
                .FourWayMovement(100)
                .Collide(26, 16, true, 0, 16)
                .Overlap(26, 16, 0, 8);

            EventBus.LatestInstance.Subscribe(
                MapEvent.ChoppedDownTree, 
                (obj) => this.SubtractEnergy(MapEvent.ChoppedDownTree));
        }

        public void SubtractEnergy(MapEvent m)
        {
            switch (m)
            {
                case MapEvent.ChoppedDownTree:
                    this.Energy -= 10;
                    break;
                default:
                    break;
            }
        }

        public static int EnergyCost(MapEvent m)
        {
            switch (m)
            {
                case MapEvent.ChoppedDownTree:
                    return 10;
                default:
                    return 0;
            }
        }
    }
}