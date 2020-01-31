using DeenGames.Sanctuary.Scenes;
using Puffin.Infrastructure.MonoGame;

namespace DeenGames.Sanctuary
{
    public class SanctuaryGame : PuffinGame
    {
        public SanctuaryGame() : base(960, 540)
        {
        }

        override protected void Ready()
        {
            this.ShowScene(new MapScene());
        }
    }
}