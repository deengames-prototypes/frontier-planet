using DeenGames.FrontierPlanet.Model.Maps;

namespace DeenGames.FrontierPlanet.Model
{
    // Represents a single save-game world.
    public class GameWorld
    {
        public static GameWorld LatestInstance = new GameWorld();

        public PlayerModel Player { get; set; }
        
        public AreaMap AreaMap;

        public GameWorld()
        {
            this.Player = new PlayerModel();
            
            // TODO: generate properly
            this.AreaMap = new AreaMap();
            this.AreaMap.Contents.Add(new TreeModel(0, 9, 6));
            this.AreaMap.Contents.Add(new RockModel(0, 15, 5));
            // Doesn't make sense: passing in a new event bus here
            this.AreaMap.Contents.Add(new PlayerModel(15, 7));
        }
    }
}
