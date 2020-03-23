using System.IO;
using DeenGames.FrontierPlanet.Model.DiscoveryDungeon;
using DeenGames.FrontierPlanet.Model.Maps;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Tiles;

namespace DeenGames.FrontierPlanet.Scenes
{
    class DiscoveryDungeonScene : Scene
    {
        // At 2x zoom, 8x8 dungeon barely fits on-screen.
        private const int TileSize = 32;

        private DiscoveryDungeon dungeon;

        public DiscoveryDungeonScene(PlayerModel player)
        {
            this.dungeon = new DiscoveryDungeon(1);
        }

        override public void Ready()
        {
            base.Ready();

            var groundAndFogTilemap = new TileMap(DiscoveryDungeon.TilesWide, DiscoveryDungeon.TilesHigh, Path.Combine("Content", "Images", "Tilesets", "Dungeon.png"), TileSize, TileSize);

            this.Add(groundAndFogTilemap);
            groundAndFogTilemap.Define("Fog", 1, 0);
            groundAndFogTilemap.Define("Floor", 0, 1);
            groundAndFogTilemap.Define("Stairs", 1, 1);

            var dungeonWidth = (DiscoveryDungeon.TilesWide * TileSize);
            var dungeonHeight = (DiscoveryDungeon.TilesHigh * TileSize);
            var screenWidth = (FrontierPlanetGame.LatestInstance.Width);
            var screenHeight = (FrontierPlanetGame.LatestInstance.Height);

            var centerX =  (screenWidth - dungeonWidth) / (2 * Constants.GameZoom);
            var centerY = (screenHeight - dungeonHeight) / (2 * Constants.GameZoom);
            // Spent about an hour on this and couldn't figure out the right formula
            // These are derived by using GIMP to measure the left/right space, divide by 2,
            // then divide by 2 again to get the right answer (second divide by 2 = zoom);
            groundAndFogTilemap.X = 192;//(int)(centerX);
            groundAndFogTilemap.Y = 56; // (int)(centerY);

            for (var y = 0; y < DiscoveryDungeon.TilesHigh; y++)
            {
                for (var x = 0; x < DiscoveryDungeon.TilesWide; x++)
                {
                    var tile = dungeon.IsVisible(x, y) ? "Floor" : "Fog";
                    groundAndFogTilemap.Set(x, y, tile);
                }   
            }

            this.Add(new Entity().Camera(Constants.GameZoom));
        }
    }
}