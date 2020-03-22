using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeenGames.FrontierPlanet.Map.Entities
{
    public class TreeEntity : Entity
    {
        internal TreeModel Model;
        private EventBus eventBus;
        
        public TreeEntity(EventBus eventBus, TreeModel model)
        {
            this.eventBus = eventBus;
            this.Model = model;
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Tree.png"))
                .Keyboard((data) => {
                    if (data is FrontierPlanetActions)
                    {
                        var action = (FrontierPlanetActions)data;
                        if (action == FrontierPlanetActions.Interact)
                        {
                            var player = GameWorld.LatestInstance.AreaMap.Player;
                            // Simpler algorithm: we occupy a set of tiles, na? So, get all adjacent tiles, and check if player is in 'em.
                            var tilesToCheck = new List<Tuple<int, int>>();
                            for (var x  = this.Model.X - 1; x < this.Model.X + this.Model.TilesWide + 1; x++)
                            {
                                for (var y  = this.Model.Y - 1; y < this.Model.Y + this.Model.TilesHigh + 1; y++)
                                {
                                    tilesToCheck.Add(new Tuple<int, int>(x, y));
                                }
                            }

                            if (tilesToCheck.Any(t => t.Item1 == player.X && t.Item2 == player.Y))
                            {
                                this.eventBus.Broadcast(MapEvent.InteractedWithTree, this);
                            }
                        }
                    }
                });
        }
    }
}