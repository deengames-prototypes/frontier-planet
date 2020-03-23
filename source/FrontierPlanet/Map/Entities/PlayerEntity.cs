using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using DeenGames.FrontierPlanet.Model.Maps;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.IO;

namespace DeenGames.FrontierPlanet.Map.Entities
{
    public class PlayerEntity : Entity
    {
        internal static PlayerEntity LatestInstance { get; private set; }
        internal const float SecondsToMoveToTile = 0.5f;
        
        internal bool IsMoving = false;

        private PlayerModel model;
        private EventBus eventBus;

        public PlayerEntity(EventBus eventBus, PlayerModel model)
        {
            PlayerEntity.LatestInstance = this;
            this.model = model;
            this.eventBus = eventBus;
            
            this.Spritesheet(Path.Combine("Content", "Images", "Characters", "Protagonist.png"), 26, 32);

            this.Keyboard(onActionDown: (data) =>
            {
                if (!this.IsMoving && data is PuffinAction)
                {
                    var moveKey = (PuffinAction)data;
                    if (moveKey == PuffinAction.Up && GameWorld.LatestInstance.AreaMap.TryToMovePlayerBy(0, -1))
                    {
                        this.OnMove(0, -1);
                    }
                    else if (moveKey == PuffinAction.Down && GameWorld.LatestInstance.AreaMap.TryToMovePlayerBy(0, 1))
                    {
                        this.OnMove(0, 1);
                    }
                    else if (moveKey == PuffinAction.Left && GameWorld.LatestInstance.AreaMap.TryToMovePlayerBy(-1, 0))
                    {
                        this.OnMove(-1, 0);
                    }
                    else if (moveKey == PuffinAction.Right && GameWorld.LatestInstance.AreaMap.TryToMovePlayerBy(1, 0))
                    {
                        this.OnMove(1, 0);
                    }
                }
            });
        }

        private void OnMove(int dx, int dy)
        {
            this.IsMoving = true;
            this.eventBus.Broadcast(MapEvent.PlayerMoved, new Tuple<int, int>(dx, dy));
        }
    }
}