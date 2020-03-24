namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    class DungeonContents
    {
        public static DungeonContents Stairs = new DungeonContents("Stairs");
        public static DungeonContents Alien = new DungeonContents("Alien");
        
        public string Sprite { get; private set; }

        private DungeonContents(string sprite)
        {
            this.Sprite = sprite;
        }

        // TODO: accept stats
        public static DungeonContents CreateMonster()
        {
            return new DungeonContents("Monster");
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