using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Scenes;
using Microsoft.Xna.Framework.Input;
using Puffin.Infrastructure.MonoGame;
using System.Collections.Generic;

namespace DeenGames.HavenIsland
{
    public class HavenIslandGame : PuffinGame
    {
        public static HavenIslandGame LatestInstance { get; set; }

        // Assuming 32x32, this gives us 720p (1280x720) - well, 736, to be exact tiles
        public HavenIslandGame() : base(1280, 736)
        {
            HavenIslandGame.LatestInstance = this;

            this.ActionToKeys[HavenIslandActions.Interact] = new List<Keys> { Keys.Space };

            // this.showCollisionAreas = true;
        }

        override protected void Ready()
        {
            this.ShowScene(new MapScene());
        }
    }
}