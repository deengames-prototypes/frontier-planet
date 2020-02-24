using DeenGames.HavenIsland.Events;
using DeenGames.HavenIsland.Model;
using NUnit.Framework;
using Puffin.Core.Events;

namespace DeenGames.HavenIsland.UnitTests.Model
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

        [TestCase(MapEvents.ChoppedDownTree)]
        [TestCase(MapEvents.MinedRock)]
        public void SubtractEnergySubtractsEnergy(MapEvents m)
        {
            var model = new PlayerModel(0, 0);
            model.SubtractEnergy(m);
            Assert.That(GameWorld.LatestInstance.PlayerEnergy, Is.LessThan(GameWorld.LatestInstance.PlayerMaxEnergy));
        }

        [Test]
        public void SubtractEnergySubtractsEnergyAmount()
        {
            var model = new PlayerModel(0, 0);
            model.SubtractEnergy(37);
            Assert.That(GameWorld.LatestInstance.PlayerEnergy, Is.LessThan(GameWorld.LatestInstance.PlayerMaxEnergy - 37));
        }

        [TestCase(MapEvents.ChoppedDownTree)]
        [TestCase(MapEvents.MinedRock)]
        public void EnergyCostReturnsNonZeroValuesForSomeEvents(MapEvents m)
        {
            Assert.That(PlayerModel.EnergyCost(m), Is.GreaterThan(0));
        }

        [TestCase(MapEvents.ChoppedDownTree)]
        [TestCase(MapEvents.MinedRock)]
        public void BroadcastingAppropriateMapEventSubtractsEnergy(MapEvents m)
        {
            var eventBus = new EventBus();
            var model = new PlayerModel(0, 0);
            eventBus.Broadcast(m);
            Assert.That(GameWorld.LatestInstance.PlayerEnergy, Is.LessThan(GameWorld.LatestInstance.PlayerMaxEnergy));
        }
    }
}