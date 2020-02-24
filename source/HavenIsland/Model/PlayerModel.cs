using DeenGames.HavenIsland.Events;
using Puffin.Core.Events;

namespace DeenGames.HavenIsland.Model
{
    public class PlayerModel : MapObject
    {
        public PlayerModel(int x, int y) : base(x, y)
        {
            EventBus.LatestInstance.Subscribe(MapEvent.ChoppedDownTree, (obj) => this.SubtractEnergy(MapEvent.ChoppedDownTree));
            EventBus.LatestInstance.Subscribe(MapEvent.MinedRock, (obj) => this.SubtractEnergy(MapEvent.MinedRock));
        }
        
        public void SubtractEnergy(MapEvent m)
        {
            var cost = EnergyCost(m);
            this.SubtractEnergy(cost);
        }

        public void SubtractEnergy(int cost)
        {
            GameWorld.LatestInstance.PlayerEnergy -= cost;
            EventBus.LatestInstance.Broadcast(GlobalEvents.ConsumedEnergy, cost);
        }

        public static int EnergyCost(MapEvent m)
        {
            switch (m)
            {
                case MapEvent.ChoppedDownTree:
                    return 10;
                case MapEvent.MinedRock:
                    return 10;
                default:
                    return 0;
            }
        }
    }
}