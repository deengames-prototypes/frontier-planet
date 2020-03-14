using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Rock : Entity
    {
        internal RockModel Model;
        private EventBus eventBus;

        public Rock(EventBus eventBus, RockModel model)
        {
            this.Model = model;
            this.eventBus = eventBus;
            
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Rock.png"))
                .Keyboard((data) => {
                    if (data is HavenIslandActions)
                    {
                        var action = (HavenIslandActions)data;
                        var player = GameWorld.LatestInstance.AreaMap.Player;
                        var distance = Math.Sqrt(Math.Pow(this.Model.X - player.X, 2) + Math.Pow(this.Model.Y - player.Y, 2));
                        if (distance == 1 && action == HavenIslandActions.Interact)
                        {
                            this.eventBus.Broadcast(MapEvent.InteractedWithRock, this);
                        }
                    }
                });
        }
    }
}