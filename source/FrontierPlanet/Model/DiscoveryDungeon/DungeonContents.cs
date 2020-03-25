namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    public class DungeonContents
    {
        public static DungeonContents Stairs = new DungeonContents("Stairs");
        public static DungeonContents Alien = new DungeonContents("Alien");
        public static DungeonContents Bomb = new DungeonContents("Bomb");
        public static DungeonContents Heal = new DungeonContents("Heal");
        public static DungeonContents EnergyBoost = new DungeonContents("EnergyBoost");
        
        public const int EnergyBoostAmount = 25;
        public const int HealPercent = 40;
        public const int BombDamage = 25;
        
        public string Sprite { get; private set; }

        public DungeonContents(string sprite)
        {
            this.Sprite = sprite;
        }
    }
}