using DeenGames.FrontierPlanet.Model;
using Puffin.Core;

namespace DeenGames.FrontierPlanet.Scenes
{
    class DiscoveryDungeonScene : Scene
    {
        private DiscoveryDungeon dungeon;

        public DiscoveryDungeonScene(PlayerModel player)
        {
            this.dungeon = new DiscoveryDungeon(1, 8, 8);
        }

        override public void Ready()
        {
            base.Ready();

            // TODO: FOV

        }
    }
}