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
            var model = new PlayerModel(new EventBus(), 0, 0);
            model.SubtractEnergy(m);
            Assert.That(GameWorld.LatestInstance.PlayerEnergy, Is.LessThan(GameWorld.LatestInstance.PlayerMaxEnergy));
        }

        [Test]
        public void SubtractEnergySubtractsEnergyAmount()
        {
            var model = new PlayerModel(new EventBus(), 0, 0);
            model.SubtractEnergy(37);
            Assert.That(GameWorld.LatestInstance.PlayerEnergy, Is.LessThan(GameWorld.LatestInstance.PlayerMaxEnergy - 37));
        }

        [TestCase(MapEvent.ChoppedDownTree)]
        [TestCase(MapEvent.MinedRock)]
        public void EnergyCostReturnsNonZeroValuesForSomeEvents(MapEvent m)
        {
            Assert.That(PlayerModel.EnergyCost(m), Is.GreaterThan(0));
        }

        [TestCase(MapEvent.ChoppedDownTree)]
        [TestCase(MapEvent.MinedRock)]
        public void BroadcastingAppropriateMapEventSubtractsEnergy(MapEvent m)
        {
            var eventBus = new EventBus();
            var model = new PlayerModel(eventBus, 0, 0);
            eventBus.Broadcast(m);
            Assert.That(GameWorld.LatestInstance.PlayerEnergy, Is.LessThan(GameWorld.LatestInstance.PlayerMaxEnergy));
        }
    }
}