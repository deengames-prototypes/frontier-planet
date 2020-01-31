using Puffin.Core;
using Puffin.Core.Ecs;

namespace DeenGames.Sanctuary.Scenes
{
    public class MapScene : Scene
    {
        public MapScene()
        {
            this.Add(new Entity().Label("HELLO!"));
        }
    }
}