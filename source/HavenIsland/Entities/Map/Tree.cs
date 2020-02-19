using Puffin.Core.Ecs;
using System.IO;

namespace DeenGames.HavenIsland.Entities.Map
{
    public class Tree : Entity
    {
        public Tree()
        {
            this.Spritesheet(Path.Join("Content", "Images", "Tilesets", "Tree.png"), 27, 64);
            this.Collide(27, 24, false, 0, 40);
        }
    }
}