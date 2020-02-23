using System.Collections.Generic;
using System.Linq;

namespace DeenGames.HavenIsland.Model
{
    public class AreaMap
    {
        public List<MapObject> Contents = new List<MapObject>();
        public PlayerModel Player { get { return this.Contents.Single(c => c is PlayerModel) as PlayerModel; } }

        public bool TryToMovePlayerBy(int dx, int dy)
        {
            bool isOccupied = this.Contents.Any(c => c.X == this.Player.X + dx && c.Y == this.Player.Y+ dy);
            if (!isOccupied)
            {
                this.Player.X += dx;
                this.Player.Y += dy;
            }

            return !isOccupied;
        }
    }

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