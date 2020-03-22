using System;
using DeenGames.FrontierPlanet.Events;

namespace DeenGames.FrontierPlanet.Model
{
    public class PlayerModel : MapObject
    {
        public int Energy { get; private set; }
        public int MaxEnergy { get; private set; }
        
        public PlayerModel() : base()
        {
            this.MaxEnergy = 100;
            this.Energy = this.MaxEnergy;
        }

        public PlayerModel(int x, int y) : base(x, y)
        {
            
        }
        
        public void SubtractEnergy(MapEvent m)
        {
            var cost = EnergyCost(m);
            this.SubtractEnergy(cost);
        }

        public void SubtractEnergy(int cost)
        {
            this.Energy -= cost;
            this.Energy = Math.Max(this.Energy, 0);
        }

        public bool HasEnergyTo(MapEvent m)
        {
            return this.Energy >= this.EnergyCost(m);
        }
        
        private int EnergyCost(MapEvent m)
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