using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Map.Entities;
using DeenGames.HavenIsland.Map.UI;
using DeenGames.HavenIsland.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using Puffin.Core.Tiles;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Scenes
{
    public class MapScene : Scene
    {
        private const int MAP_WIDTH = 40;
        private const int MAP_HEIGHT = 23;
        private Entity player;

        override public void Ready()
        {
            var map = GameWorld.LatestInstance.AreaMap;

            // Setup ground, trees, player, etc.
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

            foreach (var item in map.Contents)
            {
                if (item is TreeModel)
                {
                    this.Add(new Tree(item as TreeModel).Move(item.X * Constants.TILE_WIDTH, item.Y * Constants.TILE_HEIGHT));
                }
                else if (item is RockModel)
                {
                    this.Add(new Rock(item as RockModel).Move(item.X * Constants.TILE_WIDTH, item.Y * Constants.TILE_HEIGHT));
                }
                else if (item is PlayerModel)
                {
                    this.player = new Player(item as PlayerModel).Move(item.X * Constants.TILE_WIDTH, item.Y * Constants.TILE_HEIGHT);
                    this.Add(this.player);   
                }
            }

            // UI
            // TODO: probably backed by a PNG
            // TODO: show/hide label on mouse over/out
            this.Add(new EnergyBar());

            // Event handlers
            EventBus.LatestInstance.Subscribe(MapEvent.InteractedWithTree, (obj) => 
            {
                var tree = obj as Tree;
                if (GameWorld.LatestInstance.PlayerEnergy > PlayerModel.EnergyCost(MapEvent.InteractedWithTree))
                {
                    HavenIslandGame.LatestInstance.ShowScene(new ChopTreeScene(tree.Model));
                }
            });

            EventBus.LatestInstance.Subscribe(MapEvent.InteractedWithRock, (obj) => 
            {
                var rock = obj as Rock;
                var model = rock.Model;
                if (GameWorld.LatestInstance.PlayerEnergy > PlayerModel.EnergyCost(MapEvent.InteractedWithRock))
                {
                    HavenIslandGame.LatestInstance.ShowScene(new MineRockScene(rock.Model));
                }
            });

            EventBus.LatestInstance.Subscribe(MapEvent.PlayerMoved, (obj) =>
            {
                (var dx, var dy) = obj as Tuple<int, int>;
                
                this.TweenPosition(
                    this.player, new System.Tuple<float, float>(this.player.X, this.player.Y),
                    new System.Tuple<float, float>(
                        this.player.X + (dx * Constants.TILE_WIDTH),
                        this.player.Y + (dy * Constants.TILE_HEIGHT))
                        , Player.SecondsToMoveToTile,
                        () => (this.player as Player).IsMoving = false);
            });

            // Camera
            this.Add(new Entity().Camera(Constants.GAME_ZOOM));
        }
    }
}