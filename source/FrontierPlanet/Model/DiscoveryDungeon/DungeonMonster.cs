using System;
using DeenGames.FrontierPlanet.Model.Maps;

namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    public class DungeonMonster : DungeonContents
    {
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }

        public readonly int Strength;
        private const int DamagePerHit = 5;

        public DungeonMonster(int health, int strength) : base("Monster")
        {
            this.MaxHealth = health;
            this.Health = health;
            this.Strength = strength;
        }

        internal void TakeDamageFrom(PlayerModel player)
        {
            // TODO: take into account player strength/equipment
            this.Health -= DamagePerHit;
            this.Health = Math.Max(this.Health, 0);
        }
    }
}