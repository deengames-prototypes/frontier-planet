namespace DeenGames.HavenIsland.Model
{
    // Represents a single save-game world. Maybe it should be called GameWorld.
    public class GameWorld
    {
        public static GameWorld LatestInstance = new GameWorld();

        public int PlayerEnergy { get; set; }
        public int PlayerMaxEnergy { get; set; }
        
        public AreaMap AreaMap;

        public GameWorld()
        {
            this.PlayerEnergy = 100;
            this.PlayerMaxEnergy = 100;

            // TODO: generate properly
            this.AreaMap = new AreaMap();
            this.AreaMap.Contents.Add(new TreeModel(-1, 9, 6));
            this.AreaMap.Contents.Add(new RockModel(-1, 15, 5));
            this.AreaMap.Contents.Add(new PlayerModel(15, 8));
        }
    }
}
