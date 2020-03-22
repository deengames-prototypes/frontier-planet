using DeenGames.FrontierPlanet.Model;
using NUnit.Framework;
using Puffin.Core.Events;

namespace DeenGames.FrontierPlanet.UnitTests.Model
{
    [TestFixture]
    public class AreaMapTests
    {
        [Test]
        public void TryToMovePlayerByReturnsFalseAndDoesntMovePlayerIfSpotIsOccupied()
        {
            var eventBus = new EventBus();
            var map = new AreaMap();
            map.Contents.Add(new PlayerModel(3, 3));
            map.Contents.Add(new RockModel(0, 3, 4));
            var actual = map.TryToMovePlayerBy(0, 1);
            Assert.That(actual, Is.False);
            Assert.That(map.Player.X, Is.EqualTo(3));
            Assert.That(map.Player.Y, Is.EqualTo(3));
        }

        [Test]
        public void TryToMovePlayerByReturnsTrueAndMovesPlayerIfSpotIsEmpty()
        {
            var eventBus = new EventBus();
            var map = new AreaMap();
            map.Contents.Add(new PlayerModel(3, 3));
            var actual = map.TryToMovePlayerBy(-1, 0);
            Assert.That(actual, Is.True);
            Assert.That(map.Player.X, Is.EqualTo(2));
            Assert.That(map.Player.Y, Is.EqualTo(3));
        }

        [Test]
        public void TryToMovePlayerByReturnsTrueAndMovesPlayerIfSpotIsOccupiedBySomethingWeCanWalkOn()
        {
            var eventBus = new EventBus();
            var map = new AreaMap();
            map.Contents.Add(new PlayerModel(3, 3));
            map.Contents.Add(new Feather(3, 4));
            var actual = map.TryToMovePlayerBy(0, 1);
            Assert.That(actual, Is.True);
            Assert.That(map.Player.X, Is.EqualTo(3));
            Assert.That(map.Player.Y, Is.EqualTo(4));
        }

        class Feather : MapObject
        {
            public Feather(int x, int y) : base(x, y, true)
            {

            }
        }
    }
}