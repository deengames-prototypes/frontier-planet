using DeenGames.HavenIsland.Events;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Tree : Entity
    {
        private bool isPlayerInInteractionRange = false;

        public Tree()
        {
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Tree.png"))
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
                .Mouse(() => {
                    if (this.isPlayerInInteractionRange)
                    {
                        EventBus.LatestInstance.Broadcast(MapEvents.InteractedWithTree, this);
                    }
                }, 27, 64);
        }
    }
}