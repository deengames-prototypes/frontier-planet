using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using Puffin.Core.IO;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Player : Entity
    {
        internal static Player LatestInstance { get; private set; }
        internal bool IsMoving = false;
        private PlayerModel model;

        public Player(PlayerModel model)
        {
            Player.LatestInstance = this;
            this.model = model;
            
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
            EventBus.LatestInstance.Broadcast(MapEvents.PlayerMoved, new Tuple<int, int>(dx, dy));
        }
    }
}