using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Map.Entities;
using DeenGames.FrontierPlanet.Map.UI;
using DeenGames.FrontierPlanet.Model;
using Puffin.Core;
using Puffin.Core.Ecs;
using Puffin.Core.Tiles;
using System;
using System.IO;

namespace DeenGames.FrontierPlanet.Scenes
{
    public class MapScene : Scene
    {
        private const int MapWidth = 40;
        private const int MapHeight = 23;
        private Entity player;
        private AreaMap map;

        public MapScene(AreaMap map)
        {
            this.map = map;
        }

        override public void Ready()
        {
            // Setup ground, trees, player, etc.
            var groundTileMap = new TileMap(MapWidth, MapHeight, Path.Join("Content", "Images", "Tilesets", "Outside.png"), 32, 32);
            groundTileMap.Define("Grass", 0, 0);
            for (var y = 0; y < MapHeight; y++)
            {
                for (var x = 0; x < MapWidth; x++)
                {
                    groundTileMap.Set(x, y, "Grass");
                }
            }

            this.Add(groundTileMap);

            foreach (var item in this.map.Contents)
            {
                if (item is TreeModel)
                {
                    this.Add(new Tree(this.EventBus, item as TreeModel).Move(item.X * Constants.TileWidth, item.Y * Constants.TileHeight));
                }
                else if (item is RockModel)
                {
                    this.Add(new Rock(this.EventBus, item as RockModel).Move(item.X * Constants.TileWidth, item.Y * Constants.TileHeight));
                }
                else if (item is PlayerModel)
                {
                    this.player = new Player(this.EventBus, item as PlayerModel).Move(item.X * Constants.TileWidth, item.Y * Constants.TileHeight);
                    this.Add(this.player);   
                }
            }

            // UI
            // TODO: probably backed by a PNG
            // TODO: show/hide label on mouse over/out
            this.Add(new EnergyBar(this.EventBus));

            // Event handlers
            this.EventBus.Subscribe(MapEvent.InteractedWithTree, (obj) => 
            {
                var tree = obj as Tree;
                if (GameWorld.LatestInstance.PlayerEnergy > PlayerModel.EnergyCost(MapEvent.InteractedWithTree))
                {
                    this.ShowSubScene(new MemoryChopTreeScene(this.map, tree.Model));
                }
            });

            this.EventBus.Subscribe(MapEvent.InteractedWithRock, (obj) => 
            {
                var rock = obj as Rock;
                var model = rock.Model;
                if (GameWorld.LatestInstance.PlayerEnergy > PlayerModel.EnergyCost(MapEvent.InteractedWithRock))
                {
                    this.ShowSubScene(new TriggerMineRockScene(this.map, rock.Model));
                }
            });

            this.EventBus.Subscribe(MapEvent.PlayerMoved, (obj) =>
            {
                (var dx, var dy) = obj as Tuple<int, int>;
                
                this.TweenPosition(
                    this.player, new System.Tuple<float, float>(this.player.X, this.player.Y),
                    new System.Tuple<float, float>(
                        this.player.X + (dx * Constants.TileWidth),
                        this.player.Y + (dy * Constants.TileHeight))
                        , Player.SecondsToMoveToTile,
                        () => (this.player as Player).IsMoving = false);
            });

            // Camera
            this.Add(new Entity().Camera(Constants.GameZoom));
        }
    }
}