using DeenGames.HavenIsland.Events;
using Puffin.Core.Events;

namespace DeenGames.HavenIsland.Model
{
    public class PlayerModel : MapObject
    {
        private EventBus eventBus;
        
        public PlayerModel(EventBus eventBus, int x, int y) : base(x, y)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe(MapEvent.ChoppedDownTree, (obj) => this.SubtractEnergy(MapEvent.ChoppedDownTree));
            eventBus.Subscribe(MapEvent.MinedRock, (obj) => this.SubtractEnergy(MapEvent.MinedRock));
        }
        
        public void SubtractEnergy(MapEvent m)
        {
            var cost = EnergyCost(m);
            this.SubtractEnergy(cost);
        }

        public void SubtractEnergy(int cost)
        {
            GameWorld.LatestInstance.PlayerEnergy -= cost;
            this.eventBus.Broadcast(GlobalEvents.ConsumedEnergy, cost);
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