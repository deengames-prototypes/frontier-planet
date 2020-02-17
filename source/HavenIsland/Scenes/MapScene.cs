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
            var tilemap = new TileMap(MAP_WIDTH, MAP_HEIGHT, Path.Join("Content", "Images", "Tilesets", "Outside.png"), 32, 32);
            tilemap.Define("Grass", 0, 0);
            for (var y = 0; y < MAP_HEIGHT; y++)
            {
                for (var x = 0; x < MAP_WIDTH; x++)
                {
                    tilemap.Set(x, y, "Grass");
                }
            }

            this.Add(tilemap);

            this.Add(new Entity().Spritesheet(Path.Combine("Content", "Images", "Characters", "Protagonist.png"), 26, 32).Move(400, 300).FourWayMovement(200)); 
        }
    }
}