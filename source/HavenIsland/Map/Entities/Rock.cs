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
        private RockModel model;

        public Rock(RockModel model)
        {
            this.model = model;
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Rock.png"))
                .Keyboard((data) => {
                    if (data is HavenIslandActions)
                    {
                        var action = (HavenIslandActions)data;
                        var player = GameWorld.Instance.AreaMap.Player;
                        var distance = Math.Sqrt(Math.Pow(this.model.X - player.X, 2) + Math.Pow(this.model.Y - player.Y, 2));
                        if (distance == 1 && action == HavenIslandActions.Interact)
                        {
                            EventBus.LatestInstance.Broadcast(MapEvents.InteractedWithRock, this);
                        }
                    }
                });
        }
    }
}