using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model;
using NUnit.Framework;
using Puffin.Core.Events;

namespace DeenGames.FrontierPlanet.UnitTests.Model
{
    [TestFixture]
    public class PlayerModelTests
    {
        [SetUp]
        [TearDown]
        public void ResetWorldAndEventBus()
        {
            new GameWorld();
            new EventBus();
        }

        [TestCase(MapEvent.ChoppedDownTree)]
        [TestCase(MapEvent.MinedRock)]
        public void SubtractEnergySubtractsEnergy(MapEvent m)
        {
            var model = new PlayerModel(0, 0);
            model.SubtractEnergy(m);
            Assert.That(model.Energy, Is.LessThan(model.MaxEnergy));
        }

        [Test]
        public void SubtractEnergySubtractsEnergyAmount()
        {
            var model = new PlayerModel(0, 0);
            model.SubtractEnergy(37);
            Assert.That(model.Energy, Is.LessThan(model.MaxEnergy - 37));
        }
    }
}