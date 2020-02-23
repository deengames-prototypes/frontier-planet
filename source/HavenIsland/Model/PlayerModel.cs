using DeenGames.HavenIsland.Events;
using Puffin.Core.Events;

namespace DeenGames.HavenIsland.Model
{
    public class PlayerModel : MapObject
    {
        public PlayerModel(int x, int y) : base(x, y)
        {
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