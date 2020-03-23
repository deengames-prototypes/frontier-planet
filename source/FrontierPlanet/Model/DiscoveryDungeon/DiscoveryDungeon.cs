using System;

namespace DeenGames.FrontierPlanet.Model.DiscoveryDungeon
{
    class DiscoveryDungeon
    {
        
        private const int MaxTilesWide = 30; // 30 * 32 = 960px, minimum supported screen width
        private const int MaxTilesHigh = 16; // fits on 960x540
        
        // On a 4x4, I expect 4 monsters, 2 items, 1-2 chests
        // Given that each tile is independent: 25% monster change, 12.5% item/chest chance
        // NB: this assumes no wall tiles.
        private const float MonsterProbability = 1/2f;
        private const float ItemProbability = 1/4f;
        private const float TreasureChestProbability = 1/8f;
        private const float AlienProbability = 1/32f; // one on every other 4x4 floors

        private readonly int tilesWide;
        private readonly int tilesHigh;

        private const int StartVisibleSize = 2; // 2 = 2x2 visible on start
        private bool[,] isVisible;

        private int floorNum = 1;
        private Random random = new Random();

        public DiscoveryDungeon(int floorNum, int tilesWide, int tilesHigh)
        {
            this.floorNum = floorNum;
            this.tilesWide = Math.Min(tilesWide, MaxTilesWide);
            this.tilesHigh = Math.Min(tilesHigh, MaxTilesHigh);
            this.isVisible = new bool[this.tilesWide, this.tilesHigh];

            this.GenerateFloor();
        }

        private void GenerateFloor()
        {
            // Player starts in a random corner
            var corners = new Tuple<int, int>[] {
                new Tuple<int, int>(0, 0),
                new Tuple<int, int>(this.tilesWide - 2, 0),
                new Tuple<int, int>(0, this.tilesHigh - 2),
                new Tuple<int, int>(this.tilesWide - 2, this.tilesHigh - 2),
            };

            var playerCorner = corners[random.Next(corners.Length)];
            // Player is invisible!
            this.isVisible[playerCorner.Item1, playerCorner.Item2] = true;
            this.isVisible[playerCorner.Item1 + 1, playerCorner.Item2] = true;
            this.isVisible[playerCorner.Item1, playerCorner.Item2 + 1] = true;
            this.isVisible[playerCorner.Item1 + 1, playerCorner.Item2 + 1] = true;

            // Generate monsters
            for (var y = 0; y < this.tilesHigh; y++)
            {
                for (var x = 0; x < this.tilesWide; x++)
                {
                    if (!this.isVisible[x, y])
                    {
                        // Not a starting square, so we can put stuff on it
                        if (random.NextDouble() <= DiscoveryDungeon.MonsterProbability)
                        {
                            
                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.ItemProbability)
                        {

                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.TreasureChestProbability)
                        {

                        }
                        else if (random.NextDouble() <= DiscoveryDungeon.AlienProbability)
                        {

                        }
                    }
                }
            }
        }
    }
}