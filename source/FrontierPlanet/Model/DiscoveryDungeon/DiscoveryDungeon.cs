using System;
using DeenGames.FrontierPlanet.Model.Maps;

namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    class DiscoveryDungeon
    {
        // At 2x zoom, barely fits on-screen.
        internal const int TilesWide = 8;
        internal const int TilesHigh = 8;
        
        // On a 4x4, I expect 4 monsters, 2 items, 1-2 chests
        // Given that each tile is independent: 25% monster change, 12.5% item/chest chance
        // NB: this assumes no wall tiles.
        private const float MonsterProbability = 1/4f;
        private const float HealProbability = 1/8f;
        private const float EnergyBoostProbability = 1/8f;
        private const float BombProbability = 1/48f;
        private const float AlienProbability = 1/32f; // one on every other floor

        private const int StartVisibleSize = 2; // 2 = 2x2 visible on start
        private bool[,] isVisible;
        
        private DungeonContents[,] contents;
        private PlayerModel player;
        private int floorNum = 1;
        private Random random = new Random();

        public DiscoveryDungeon(int floorNum, PlayerModel player)
        {
            this.floorNum = floorNum;
            this.player = player;
            this.isVisible = new bool[TilesWide, TilesHigh];
            this.contents = new DungeonContents[TilesWide, TilesHigh];

            this.GenerateFloor();
        }

        public bool IsVisible(int x, int y)
        {
            return this.isVisible[x, y];
        }

        public bool IsStairs(int x, int y)
        {
            return this.contents[x, y] == DungeonContents.Stairs;
        }

        public DungeonContents Contents(int x, int y)
        {
            return this.contents[x, y];
        }

        public void Reveal(int x, int y)
        {
            // TODO: validate is valid coordinates
            this.isVisible[x, y] = true;
        }

        public void AttackMonsterAt(int x, int y, bool usingSnipe)
        {
            var monster = this.contents[x, y] as DungeonMonster;
            monster.TakeDamageFrom(this.player);
            if (usingSnipe)
            {
                // Snipe = second attack / 2x damage
                monster.TakeDamageFrom(this.player);
            }

            // Don't retaliate if sniped
            if (monster.Health > 0 && !usingSnipe)
            {
                this.player.TakeDamageFrom(monster);
            }
            else if (monster.Health <= 0)
            {
                this.contents[x, y] = null;
                // TODO: grant XP etc.
            }
        }

        public void ConsumeItemAt(int x, int y)
        {
            switch (this.contents[x, y].Sprite)
            {
                case "Heal":
                    this.ConsumeHeal();
                    break;
                case "EnergyBoost":
                    this.ConsumeEnergyBoost();
                    break;
                case "Bomb":
                    this.ConsumeBomb();
                    break;
            }
            this.contents[x, y] = null;
        }

        private void ConsumeHeal()
        {
            var healPercent = DungeonContents.HealPercent;
            var healAmount = (int)Math.Ceiling((healPercent / 100f) * this.player.MaxHealth);
            player.Heal(healAmount);

        }

        private void ConsumeEnergyBoost()
        {
            var boostAmount = DungeonContents.EnergyBoostAmount;
            player.RecoverEnergy(boostAmount);
        }

        private void ConsumeBomb()
        {
            for (var y = 0; y < TilesHigh; y++)
            {
                for (var x = 0; x < TilesWide; x++)
                {
                    if (this.isVisible[x, y] && this.contents[x, y] is DungeonMonster)
                    {
                        var monster = this.contents[x, y] as DungeonMonster;
                        monster.TakeDamage(DungeonContents.BombDamage);
                        if (monster.Health <= 0)
                        {
                            this.contents[x, y] = null;
                        }
                    }
                }
            }
        }

        private void GenerateFloor()
        {
            var playerX = random.Next(TilesWide);
            var playerY = random.Next(TilesHigh);
            this.isVisible[playerX, playerY] = true;

            var stairsX = playerX;
            var stairsY = playerY;

            while (stairsX == playerX || stairsY == playerY)
            {
                stairsX = random.Next(TilesWide);
                stairsY = random.Next(TilesHigh);
            }
            this.contents[stairsX, stairsY] = DungeonContents.Stairs;

            // Generate monsters
            for (var y = 0; y < TilesHigh; y++)
            {
                for (var x = 0; x < TilesWide; x++)
                {
                    if (this.contents[x, y] != DungeonContents.Stairs && (x != playerX && y != playerY) && !this.isVisible[x, y])
                    {
                        // Not a starting square, so we can put stuff on it
                        if (random.NextDouble() <= DiscoveryDungeon.MonsterProbability)
                        {
                            this.contents[x, y] = new DungeonMonster(100, 25);
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.HealProbability)
                        {
                            this.contents[x, y] = DungeonContents.Heal;
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.EnergyBoostProbability)
                        {
                            this.contents[x, y] = DungeonContents.EnergyBoost;
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.BombProbability)
                        {
                            this.contents[x, y] = DungeonContents.Bomb;
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.AlienProbability)
                        {
                            this.contents[x, y] = DungeonContents.Alien;
                        }
                    }
                }
            }
        }
    }
}