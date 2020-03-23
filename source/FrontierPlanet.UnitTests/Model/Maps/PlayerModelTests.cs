using DeenGames.FrontierPlanet.Events;
using DeenGames.FrontierPlanet.Model.Maps;
using NUnit.Framework;

namespace DeenGames.FrontierPlanet.UnitTests.Model.Maps
{
    [TestFixture]
    public class PlayerModelTests
    {
        [TestCase(MapEvent.ChoppedDownTree)]
        [TestCase(MapEvent.MinedRock)]
        public void SubtractEnergySubtractsEnergy(MapEvent m)
        {
            var model = new PlayerModel();
            Assert.That(model.EnergyCost(m), Is.GreaterThan(0), $"Can't test SubtractEnergy({m}) because the cost is zero");
            model.SubtractEnergy(m);
            Assert.That(model.Energy, Is.LessThan(model.MaxEnergy));
        }

        [Test]
        public void SubtractEnergySubtractsEnergyAmount()
        {
            var model = new PlayerModel();
            model.SubtractEnergy(37);
            Assert.That(model.Energy, Is.EqualTo(model.MaxEnergy - 37));
        }

        [Test]
        public void HasEnergyToReturnsTrueIfPlayerHasEnoughEnergy()
        {
            const MapEvent m = MapEvent.ChoppedDownTree;

            // boundary conditions
            var player = new PlayerModel();
            
            // case 1: full energy = true
            Assert.That(player.HasEnergyTo(m), Is.True);

            // case 2: exact energy = true
            var cost = player.EnergyCost(m);
            player.SubtractEnergy(player.MaxEnergy - cost);
            Assert.That(player.HasEnergyTo(m), Is.True);

            // case 3: exact - 1 = false
            player.SubtractEnergy(1);
            Assert.That(player.HasEnergyTo(m), Is.False);

            // case 4: 0 energy = false
            player.SubtractEnergy(99999);
            Assert.That(player.HasEnergyTo(m), Is.False);
        }
    }
}