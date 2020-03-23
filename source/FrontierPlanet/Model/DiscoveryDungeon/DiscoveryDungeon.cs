using System;

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
        private string[,] contents;

        private int floorNum = 1;
        private Random random = new Random();

        public DiscoveryDungeon(int floorNum)
        {
            this.floorNum = floorNum;
            this.isVisible = new bool[TilesWide, TilesHigh];
            this.contents = new string[TilesWide, TilesHigh];

            this.GenerateFloor();
        }

        public bool IsVisible(int x, int y)
        {
            return this.isVisible[x, y];
        }

        public string Contents(int x, int y)
        {
            return this.contents[x, y];
        }

        private void GenerateFloor()
        {
            // Player starts in a random corner
            var corners = new Tuple<int, int>[] {
                new Tuple<int, int>(0, 0),
                new Tuple<int, int>(TilesWide - 2, 0),
                new Tuple<int, int>(0, TilesHigh - 2),
                new Tuple<int, int>(TilesWide - 2, TilesHigh - 2),
            };

            var playerCorner = corners[random.Next(corners.Length)];
            // Player is invisible!
            this.isVisible[playerCorner.Item1, playerCorner.Item2] = true;
            this.isVisible[playerCorner.Item1 + 1, playerCorner.Item2] = true;
            this.isVisible[playerCorner.Item1, playerCorner.Item2 + 1] = true;
            this.isVisible[playerCorner.Item1 + 1, playerCorner.Item2 + 1] = true;

            // Generate monsters
            for (var y = 0; y < TilesHigh; y++)
            {
                for (var x = 0; x < TilesWide; x++)
                {
                    if (!this.isVisible[x, y])
                    {
                        // Not a starting square, so we can put stuff on it
                        if (random.NextDouble() <= DiscoveryDungeon.MonsterProbability)
                        {
                            this.contents[x, y] = "Monster";
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.ItemProbability)
                        {
                            this.contents[x, y] = "Item";
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.TreasureChestProbability)
                        {
                            this.contents[x, y] = "Treasure";
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.AlienProbability)
                        {
                            this.contents[x, y] = "Alien";
                        }
                    }
                }
            }
        }
    }
}