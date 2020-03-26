using System;
using DeenGames.FrontierPlanet.Model.Maps;

namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    public class DungeonMonster : DungeonContents
    {
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }

        public readonly int Strength;

        public DungeonMonster(string name, int health, int strength) : base(name)
        {
            this.MaxHealth = health;
            this.Health = health;
            this.Strength = strength;
        }

        internal void TakeDamageFrom(PlayerModel player)
        {
            // TODO: take into account player strength/equipment
            this.TakeDamage(player.DamagePerHit);
        }

        internal void TakeDamage(int damage)
        {
            this.Health = Math.Max(this.Health - damage, 0);
        }
    }
}