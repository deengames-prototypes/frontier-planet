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
            this.BindCustomKeyboardActions();

            // this.showCollisionAreas = true;
        }

        override protected void Ready()
        {
            this.ShowScene(new MapScene());
        }

        private void BindCustomKeyboardActions()
        {
            this.ActionToKeys[HavenIslandActions.Interact] = new List<Keys> { Keys.Space };
            
            this.ActionToKeys[HavenIslandActions.Pressed1] = new List<Keys> { Keys.NumPad1 };
            this.ActionToKeys[HavenIslandActions.Pressed2] = new List<Keys> { Keys.NumPad2 };
            this.ActionToKeys[HavenIslandActions.Pressed3] = new List<Keys> { Keys.NumPad3 };
            this.ActionToKeys[HavenIslandActions.Pressed4] = new List<Keys> { Keys.NumPad4 };
            this.ActionToKeys[HavenIslandActions.Pressed5] = new List<Keys> { Keys.NumPad5 };
            this.ActionToKeys[HavenIslandActions.Pressed6] = new List<Keys> { Keys.NumPad6 };
            this.ActionToKeys[HavenIslandActions.Pressed7] = new List<Keys> { Keys.NumPad7 };
            this.ActionToKeys[HavenIslandActions.Pressed8] = new List<Keys> { Keys.NumPad8 };
            this.ActionToKeys[HavenIslandActions.Pressed9] = new List<Keys> { Keys.NumPad9 };
        }
    }
}