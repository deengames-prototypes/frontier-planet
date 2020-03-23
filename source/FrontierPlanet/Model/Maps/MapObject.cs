namespace DeenGames.FrontierPlanet.Model.Maps
{
    public class MapObject
    {
        public bool IsWalkable { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int TilesWide { get; set; } = 1;
        public int TilesHigh { get; set; } = 1;

        public MapObject()
        {
        }

        public MapObject(int x, int y, bool isWalkable = false)
        {
            this.X = x;
            this.Y = y;
            this.IsWalkable = isWalkable;
        }
    }

    public class RockModel : MapObject
    {
        public int Difficulty { get; private set; }

        public RockModel(int difficulty, int x, int y) : base(x, y)
        {
            this.Difficulty = difficulty;
        }
    }

    public class TreeModel : MapObject
    {
        public TreeModel(int difficulty, int x, int y) : base(x, y)
        {
            this.TilesHigh = 2;
        }
    }
}