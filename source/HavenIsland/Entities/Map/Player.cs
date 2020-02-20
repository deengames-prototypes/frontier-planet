using Puffin.Core.Ecs;
using System.IO;

namespace DeenGames.HavenIsland.Entities.Map
{
    public class Player : Entity
    {
        internal static Player LatestInstance { get; private set; }
        
        public Player()
        {
            Player.LatestInstance = this;

            this.Spritesheet(Path.Combine("Content", "Images", "Characters", "Protagonist.png"), 26, 32)
                .FourWayMovement(100)
                .Collide(26, 16, true, 0, 16)
                .Overlap(26, 16, 0, 8);
        }
    }
}