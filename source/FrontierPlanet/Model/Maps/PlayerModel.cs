using System;
using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model.DiscoveryDungeon;

namespace DeenGames.FrontierPlanet.Model.Maps
{
    public class PlayerModel : MapObject
    {
        public int Energy { get; private set; }
        public int MaxEnergy { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        private const int NewGameMaxEnergy = 100;
        private const int NewGameMaxHealth = 250;

        // Used in testing, deserialization, etc.
        public PlayerModel() : this(0, 0)
        {

        }        
    
        public PlayerModel(int x, int y) : base(x, y)
        {
            this.MaxEnergy = NewGameMaxEnergy;
            this.Energy = this.MaxEnergy;

            this.MaxHealth = NewGameMaxHealth;
            this.Health = this.MaxHealth;
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

        public void TakeDamageFrom(DungeonMonster monster)
        {
            // TODO: take into account my equipment
            this.Health = Math.Max(this.Health - monster.Strength, 0);
            // If DEAD?!
        }

        public void Heal(int amount)
        {
            this.Health = Math.Min(this.Health + amount, this.MaxHealth);
        }

        public void RecoverEnergy(int amount)
        {
            this.Energy = Math.Min(this.Energy + amount, this.MaxEnergy);
        }
        
        internal int EnergyCost(MapEvent m)
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