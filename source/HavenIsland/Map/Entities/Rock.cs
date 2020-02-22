using DeenGames.HavenIsland.Events;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Rock : Entity
    {
        private bool isPlayerInInteractionRange = false;

        public Rock()
        {
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Rock.png"))
                .Collide(16, 16)
                .Overlap(32, 32, -8, -8,
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
                .Keyboard((data) => {
                    var action = (HavenIslandActions)data;
                    if (this.isPlayerInInteractionRange && action == HavenIslandActions.Interact)
                    {
                        EventBus.LatestInstance.Broadcast(MapEvents.InteractedWithRock, this);
                    }
                })
                .Mouse(() => {
                    if (this.isPlayerInInteractionRange)
                    {
                        EventBus.LatestInstance.Broadcast(MapEvents.InteractedWithRock, this);
                    }
                }, 27, 64);
        }
    }
}