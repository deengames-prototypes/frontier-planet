using System.IO;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Tiles;

namespace DeenGames.Sanctuary.Scenes
{
    public class MapScene : Scene
    {
        private const int MAP_WIDTH = 30;
        private const int MAP_HEIGHT = 20;
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
            this.Add(new Entity().Sprite("Content/Images/Characters/Imam.png").FourWayMovement(100).Move(100, 50));
        }
    }
}