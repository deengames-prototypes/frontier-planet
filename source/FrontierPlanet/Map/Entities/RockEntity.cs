using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.FrontierPlanet.Map.Entities
{
    public class RockEntity : Entity
    {
        internal RockModel Model;
        private EventBus eventBus;

        public RockEntity(EventBus eventBus, RockModel model)
        {
            this.Model = model;
            this.eventBus = eventBus;
            
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Rock.png"))
                .Keyboard((data) => {
                    if (data is FrontierPlanetActions)
                    {
                        var action = (FrontierPlanetActions)data;
                        var player = GameWorld.LatestInstance.AreaMap.Player;
                        var distance = Math.Sqrt(Math.Pow(this.Model.X - player.X, 2) + Math.Pow(this.Model.Y - player.Y, 2));
                        if (distance == 1 && action == FrontierPlanetActions.Interact)
                        {
                            this.eventBus.Broadcast(MapEvent.InteractedWithRock, this);
                        }
                    }
                });
        }
    }
}