using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using DeenGames.FrontierPlanet.Scenes;
using Microsoft.Xna.Framework.Input;
using Puffin.Infrastructure.MonoGame;
using System.Collections.Generic;

namespace DeenGames.FrontierPlanet
{
    public class FrontierPlanetGame : PuffinGame
    {
        public static FrontierPlanetGame LatestInstance { get; set; }

        // Assuming 32x32, this gives us 720p (1280x720) - well, 736, to be exact tiles
        public FrontierPlanetGame() : base(1280, 736)
        {
            FrontierPlanetGame.LatestInstance = this;
            this.BindCustomKeyboardActions();

            // this.showCollisionAreas = true;
        }

        override protected void Ready()
        {
            this.ShowScene(new MapScene(GameWorld.LatestInstance.AreaMap));
        }

        private void BindCustomKeyboardActions()
        {
            this.ActionToKeys[FrontierPlanetActions.Interact] = new List<Keys> { Keys.Space };
            this.ActionToKeys[FrontierPlanetActions.Cancel] = new List<Keys> { Keys.Escape };
            
            // Zero is unused thus far
            this.ActionToKeys[FrontierPlanetActions.Pressed1] = new List<Keys> { Keys.D0, Keys.NumPad0 };
            this.ActionToKeys[FrontierPlanetActions.Pressed1] = new List<Keys> { Keys.D1, Keys.NumPad1 };
            this.ActionToKeys[FrontierPlanetActions.Pressed2] = new List<Keys> { Keys.D2, Keys.NumPad2 };
            this.ActionToKeys[FrontierPlanetActions.Pressed3] = new List<Keys> { Keys.D3, Keys.NumPad3 };
            this.ActionToKeys[FrontierPlanetActions.Pressed4] = new List<Keys> { Keys.D4, Keys.NumPad4 };
            this.ActionToKeys[FrontierPlanetActions.Pressed5] = new List<Keys> { Keys.D5, Keys.NumPad5 };
            this.ActionToKeys[FrontierPlanetActions.Pressed6] = new List<Keys> { Keys.D6, Keys.NumPad6 };
            this.ActionToKeys[FrontierPlanetActions.Pressed7] = new List<Keys> { Keys.D7, Keys.NumPad7 };
            this.ActionToKeys[FrontierPlanetActions.Pressed8] = new List<Keys> { Keys.D8, Keys.NumPad8 };
            this.ActionToKeys[FrontierPlanetActions.Pressed9] = new List<Keys> { Keys.D9, Keys.NumPad9 };
        }
    }
}