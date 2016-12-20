using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace howto_hexagonal_grid
{
    public static class TerrainController
    {
        private static Stack<RecursiveFillCommand> stack = new Stack<RecursiveFillCommand>();

        // Mutates the collection.
        public static void FillAll(HexagonCollection collection, int maxRow)
        {
            var seeds = collection.List().Where(h => h.Terrain.TerrainEnum != TerrainEnum.NoTerrain);
            if (!seeds.Any())
            {
                throw new InvalidOperationException("Needs seeds to generate.");
            }
            else
            {
                foreach (var seed in seeds)
                {
                    stack.Push(new RecursiveFillCommand() { collection = collection, hex = seed });
                }
            }

            while (stack.Count > 0)
            {
                FillAllRecursive(stack.Pop(), maxRow);
            }
        }

        private static void FillAllRecursive(RecursiveFillCommand command, int maxRow)
        {
            var blankAdjacent = command.collection.GetBlankAdjacent(command.hex).OrderBy(x => Guid.NewGuid());
            foreach (var blank in blankAdjacent)
            {
                Console.WriteLine("FOUND");
                var nonBlankAdjacent = command.collection.GetNonBlankAdjacent(blank);

                if (nonBlankAdjacent.Count == 0)
                {
                    throw new InvalidOperationException("No Non-Blank Adjacent.");
                }

                int averageAltitude = (int) nonBlankAdjacent.Select(h => h.Elevation).Average();
                var newaltitude = ElevationTable.Get() + averageAltitude;
                blank.Elevation = newaltitude;

                if (newaltitude < command.collection.MinElevation)
                {
                    command.collection.MinElevation = newaltitude;
                }
                else if (newaltitude > command.collection.MaxElevation)
                {
                    command.collection.MaxElevation = newaltitude;
                }

                var elevationAdjusted = TerrainAdjacentcy.ApplyAltitudeAdjustment(TerrainController.DefaultTerrains, blank.Elevation);
                var latitudeAdjusted = TerrainAdjacentcy.ApplyLatitudeAdjustment(elevationAdjusted, blank.Row, maxRow);
                var finalTable = new RandomTable<TerrainEnum>(latitudeAdjusted);

                blank.Terrain = Terrain.EnumMap[finalTable.Get()];
                stack.Push(new RecursiveFillCommand() { collection = command.collection, hex = blank });
            }
        }

        public static List<RandomElement<TerrainEnum>> DefaultTerrains = new List<RandomElement<TerrainEnum>>(){    
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Grassland,       Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Desert,          Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Scrubland,       Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Hills,           Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Mountain,        Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.TemperateForest, Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.ColdForest,      Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Jungle,          Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Swamp,           Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Tundra,          Prob = 1.0},
            new RandomElement<TerrainEnum>() {Element = TerrainEnum.Ice,             Prob = 1.0}
        };

        private static RandomTable<int> ElevationTable = new RandomTable<int>(new List<int>(){-250, -100, -25, 0, 0, 25, 100, 250});

        private class RecursiveFillCommand{
            public HexagonCollection collection {get; set;}
            public HexagonData hex {get; set;}
        }

        private static HexagonCollection MakeDefaultHexagons(int maxX, int maxY, bool wrapEastWest)
        {
            var hexList = new List<HexagonData>();
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    hexList.Add(new HexagonData()
                    {
                        Row = i,
                        Col = j,
                        Terrain = Terrain.EnumMap[TerrainEnum.NoTerrain],
                        Elevation = 0
                    });
                }
            }

            var collection = new HexagonCollection(hexList, wrapEastWest, maxY);

            return collection;
        }

        public static HexagonCollection GenerateWorld(WorldGenParamters world)
        {
            var collection = MakeDefaultHexagons(world.MaxRows, world.MaxCol, world.WrapEastWest);
 
            CenterSeed(collection, world);
            if (world.WaterSurrounds)
            {
                SidesOcean(collection, world);
            }


            TerrainController.FillAll(collection, world.MaxRows);

            return collection;
        }

        public static void CenterSeed(HexagonCollection collection, WorldGenParamters world){
            collection.Get(world.MaxRows/2, world.MaxCol/2).Terrain = Terrain.EnumMap[TerrainEnum.Grassland];
        }

        public static void SidesOcean(HexagonCollection collection, WorldGenParamters world)
        {
            List<int> rows = new List<int>(){ 0, 1, world.MaxRows-2, world.MaxRows-1};

            foreach (int row in rows)
            {
                for (int col = 0; col < world.MaxCol; col++)
                {
                    collection.Get(row, col).Elevation = -500;
                }
            }

            List<int> cols = new List<int>() { 0, world.MaxCol - 1 };

            foreach (int col in cols)
            {
                for (int row = 0; row < world.MaxRows; row++)
                {
                    var hex = collection.Get(row, col);
                    hex.Elevation = -500;
                    hex.Terrain.TerrainEnum = TerrainEnum.Grassland;
                }
            }
        }

    }


}
