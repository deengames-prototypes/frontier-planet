using System;
using DeenGames.FrontierPlanet.Model.Maps;

namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    public class DungeonMonster : DungeonContents
    {
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }

        public readonly int Strength;
        private const int DamagePerHit = 25;

        public DungeonMonster(int health, int strength) : base("Monster")
        {
            this.MaxHealth = health;
            this.Health = health;
            this.Strength = strength;
        }

        internal void TakeDamageFrom(PlayerModel player)
        {
            // TODO: take into account player strength/equipment
            this.TakeDamage(DamagePerHit);
        }

        internal void TakeDamage(int damage)
        {
            this.Health = Math.Max(this.Health - damage, 0);
        }
    }
}