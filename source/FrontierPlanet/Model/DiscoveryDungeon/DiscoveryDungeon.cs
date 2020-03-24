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
        private const float ItemProbability = 1/8f;
        private const float TreasureChestProbability = 1/8f;
        private const float AlienProbability = 1/32f; // one on every other 4x4 floors

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

        public void AttackMonsterAt(int x, int y)
        {
            var monster = this.contents[x, y] as DungeonMonster;
            monster.TakeDamageFrom(this.player);
            if (monster.Health > 0)
            {
                this.player.TakeDamageFrom(monster);
            }
            else
            {
                this.contents[x, y] = null;
                // TODO: grant XP etc.
            }
        }

        private void GenerateFloor()
        {
            var playerX = random.Next(TilesWide);
            var playerY = random.Next(TilesHigh);
            this.isVisible[playerX, playerY] = true;

            var stairsX = playerX;
            var stairsY = playerY;

            while (isVisible[stairsX, stairsY])
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
                            this.contents[x, y] = DungeonContents.CreateMonster();
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.ItemProbability)
                        {
                            this.contents[x, y] = DungeonContents.CreateItem();
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.TreasureChestProbability)
                        {
                            this.contents[x, y] = DungeonContents.CreateTreasure();
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