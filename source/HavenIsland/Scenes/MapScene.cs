using System.IO;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Tiles;

namespace DeenGames.HavenIsland.Scenes
{
    public class MapScene : Scene
    {
        private const int MAP_WIDTH = 40;
        private const int MAP_HEIGHT = 23;

        public MapScene()
        {
            var groundTileMap = new TileMap(MAP_WIDTH, MAP_HEIGHT, Path.Join("Content", "Images", "Tilesets", "Outside.png"), 32, 32);
            groundTileMap.Define("Grass", 0, 0);
            for (var y = 0; y < MAP_HEIGHT; y++)
            {
                for (var x = 0; x < MAP_WIDTH; x++)
                {
                    groundTileMap.Set(x, y, "Grass");
                }
            }

            this.Add(groundTileMap);

            var objectsTileMap = new TileMap(MAP_WIDTH, MAP_HEIGHT, Path.Join("Content", "Images", "Tilesets", "Tree.png"), 27, 32);
            objectsTileMap.Define("TreeTop", 0, 0, true);
            objectsTileMap.Define("TreeTrunk", 0, 1, true);
            objectsTileMap.Set(20, 10, "TreeTop");
            objectsTileMap.Set(20, 11, "TreeTrunk");
            
            this.Add(objectsTileMap);

            this.Add(new Entity()
                .Spritesheet(Path.Combine("Content", "Images", "Characters", "Protagonist.png"), 26, 32)
                .Move(400, 300).FourWayMovement(100)
                .Collide(26, 32, true));
        }
    }
}