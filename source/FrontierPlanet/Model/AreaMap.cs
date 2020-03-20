using System.Collections.Generic;
using System.Linq;

namespace DeenGames.FrontierPlanet.Model
{
    public class AreaMap
    {
        public List<MapObject> Contents = new List<MapObject>();
        public PlayerModel Player { get { return this.Contents.Single(c => c is PlayerModel) as PlayerModel; } }

        // Returns true if player moves
        public bool TryToMovePlayerBy(int dx, int dy)
        {
            bool isOccupied = false;
            
            foreach (var contents in this.Contents)
            {
                for (var y = 0; y < contents.TilesHigh; y++)
                {
                   for (var x = 0; x < contents.TilesWide; x++)
                   {
                        if (!contents.IsWalkable && contents.X + x == this.Player.X + dx && contents.Y + y == this.Player.Y + dy)
                        {
                            isOccupied = true;
                            break;
                        }
                   } 
                }
            }

            if (!isOccupied)
            {
                this.Player.X += dx;
                this.Player.Y += dy;
            }

            return !isOccupied;
        }
    }
}