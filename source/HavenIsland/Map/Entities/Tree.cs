using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Tree : Entity
    {
        internal TreeModel Model;
        
        public Tree(TreeModel model)
        {
            this.Model = model;
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Tree.png"))
                .Keyboard((data) => {
                    if (data is HavenIslandActions)
                    {
                        var action = (HavenIslandActions)data;
                        var player = GameWorld.LatestInstance.AreaMap.Player;
                        var distance = Math.Sqrt(Math.Pow(this.Model.X - player.X, 2) + Math.Pow(this.Model.Y - player.Y, 2));
                        if (distance == 1 && action == HavenIslandActions.Interact)
                        {
                            EventBus.LatestInstance.Broadcast(MapEvent.InteractedWithTree, this);
                        }
                    }
                });
        }
    }
}