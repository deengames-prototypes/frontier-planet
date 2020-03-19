using System.Collections.Generic;
using System.Linq;

namespace DeenGames.FrontierPlanet.Model
{
    public class AreaMap
    {
        public List<MapObject> Contents = new List<MapObject>();
        public PlayerModel Player { get { return this.Contents.Single(c => c is PlayerModel) as PlayerModel; } }

        public bool TryToMovePlayerBy(int dx, int dy)
        {
            bool isOccupied = this.Contents.Any(c => c.X == this.Player.X + dx && c.Y == this.Player.Y + dy && c.IsWalkable == false);
            if (!isOccupied)
            {
                this.Player.X += dx;
                this.Player.Y += dy;
            }

            return !isOccupied;
        }
    }
}