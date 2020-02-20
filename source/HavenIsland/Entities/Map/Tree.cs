using DeenGames.HavenIsland.Events;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Entities.Map
{
    public class Tree : Entity
    {
        private bool isPlayerInInteractionRange = false;

        public Tree()
        {
            this.Spritesheet(Path.Join("Content", "Images", "Tilesets", "Tree.png"), 27, 64)
                .Collide(27, 64)
                .Overlap(43, 32, -8, 48,
                (e) => {
                    if (!this.isPlayerInInteractionRange && e == Player.LatestInstance)
                    {
                        this.isPlayerInInteractionRange = true;
                    }
                },
                (e) => {
                    if (this.isPlayerInInteractionRange && e == Player.LatestInstance) {
                        this.isPlayerInInteractionRange = false;
                    }
                })
                .Mouse(27, 64, () => {
                    if (this.isPlayerInInteractionRange)
                    {
                        EventBus.LatestInstance.Broadcast(MapEvent.InteractedWithTree, this);
                    }
                });
        }
    }
}