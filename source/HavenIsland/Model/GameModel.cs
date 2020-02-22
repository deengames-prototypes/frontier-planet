namespace DeenGames.HavenIsland.Model
{
    public class GameModel
    {
        public static GameModel Instance = new GameModel();

        public int PlayerEnergy { get; set; }
        public int PlayerMaxEnergy { get; set; }

        private GameModel()
        {
            this.PlayerEnergy = 100;
            this.PlayerMaxEnergy = 100;
        }
    }
}