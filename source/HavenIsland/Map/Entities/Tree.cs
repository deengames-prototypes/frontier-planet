using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using Puffin.Core.Ecs;
using Puffin.Core.Events;
using System;
using System.IO;

namespace DeenGames.HavenIsland.Map.Entities
{
    public class Tree : Entity
    {
        public Tree(TreeModel model)
        {
            this.Sprite(Path.Join("Content", "Images", "Tilesets", "Tree.png"));
        }
    }
}