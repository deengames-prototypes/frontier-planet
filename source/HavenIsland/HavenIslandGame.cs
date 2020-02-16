using DeenGames.HavenIsland.Scenes;
using Puffin.Infrastructure.MonoGame;

namespace DeenGames.HavenIsland
{
    public class HavenIslandGame : PuffinGame
    {
        public HavenIslandGame() : base(960, 540)
        {
        }

        override protected void Ready()
        {
            this.ShowScene(new MapScene());
        }
    }
}