namespace DeenGames.FrontierPlanet.Model
{
    public class MapObject
    {
        public bool IsWalkable { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public MapObject(int x, int y, bool isWalkable = false)
        {
            this.X = x;
            this.Y = y;
            this.IsWalkable = isWalkable;
        }
    }

    public class RockModel : MapObject
    {
        public int Size { get; private set; }
        public RockModel(int size, int x, int y) : base(x, y)
        {
            this.Size = size;
        }
    }

    public class TreeModel : MapObject
    {
        public int Height { get; private set; }
        public TreeModel(int height, int x, int y) : base(x, y)
        {
            this.Height = height;
        }
    }
}