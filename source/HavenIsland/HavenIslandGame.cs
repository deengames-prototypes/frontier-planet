using DeenGames.HavenIsland.Scenes;
using Puffin.Infrastructure.MonoGame;

namespace DeenGames.HavenIsland
{
    public class HavenIslandGame : PuffinGame
    {
        // Assuming 32x32, this gives us 720p (1280x720) - well, 736, to be exact tiles
        public HavenIslandGame() : base(1280, 736)
        {
        }

        override protected void Ready()
        {
            this.ShowScene(new MapScene());
        }
    }
}