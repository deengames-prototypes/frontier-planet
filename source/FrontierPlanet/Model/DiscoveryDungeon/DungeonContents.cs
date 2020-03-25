namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    public class DungeonContents
    {
        public static DungeonContents Stairs = new DungeonContents("Stairs");
        public static DungeonContents Alien = new DungeonContents("Alien");
        public static DungeonContents Bomb = new DungeonContents("Bomb");
        
        public string Sprite { get; private set; }

        public DungeonContents(string sprite)
        {
            this.Sprite = sprite;
        }
    }
}