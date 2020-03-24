using System;
using DeenGames.FrontierPlanet.Model.Maps;

namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    public class DungeonContents
    {
        public static DungeonContents Stairs = new DungeonContents("Stairs");
        public static DungeonContents Alien = new DungeonContents("Alien");
        
        public string Sprite { get; private set; }

        protected DungeonContents(string sprite)
        {
            this.Sprite = sprite;
        }

        // TODO: accept stats
        public static DungeonContents CreateMonster()
        {
            return new DungeonMonster(20, 5);
        }

        // Generic could work for now
        public static DungeonContents CreateItem()
        {
            return new DungeonContents("Item");
        }

        // Could be generic for now
        public static DungeonContents CreateTreasure()
        {
            return new DungeonContents("Treasure");
        }
    }
}