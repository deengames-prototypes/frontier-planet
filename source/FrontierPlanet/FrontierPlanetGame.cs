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
            //this.ShowScene(new MapScene(GameWorld.LatestInstance.AreaMap, GameWorld.LatestInstance.Player));
            this.ShowScene(new DiscoveryDungeonScene(GameWorld.LatestInstance.Player));
        }

        private void BindCustomKeyboardActions()
        {
            this.ActionToKeys[FrontierPlanetActions.Interact] = new List<Keys> { Keys.Space };
            this.ActionToKeys[FrontierPlanetActions.Cancel] = new List<Keys> { Keys.Escape };
        }
    }
}