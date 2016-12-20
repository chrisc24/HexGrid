using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;

namespace howto_hexagonal_grid
{
    public class HexagonData
    {
        public int Row { get; set; }

        public int Col { get; set; }

        public Terrain Terrain { get; set; }

        public int Elevation { get; set; }

        public bool Underwater()
        {
            return Elevation < 0;
        }
    }

    public class HexagonCollection
    {
        private Dictionary<Point, HexagonData> dict;
        private bool eastWestWrap;
        private int maxCol;

        public HexagonCollection(List<HexagonData> hexagons, bool eastWestWrap, int maxCol)
        {
            dict = new Dictionary<Point, HexagonData>();
            foreach(var hex in hexagons){
                dict.Add(new Point(hex.Row, hex.Col), hex);
            }
            this.eastWestWrap = eastWestWrap;
            this.maxCol = maxCol;
        }

        public int MaxElevation { get; set; }

        public int MinElevation { get; set; }

        public HexagonData Get(int x, int y) {
            int initialY = y;

            DebugWrite(initialY, "START: y:" + y);

            if (this.eastWestWrap && y >= this.maxCol)
            {
                y = y - maxCol;
            }
            else if (this.eastWestWrap && y < 0)
            {
                y = maxCol + y;
            }

            HexagonData outVar;
            DebugWrite(initialY, "QUERY: y:" + y);
            dict.TryGetValue(new Point(x, y), out outVar);
            DebugWrite(initialY, "RESULT: " + (outVar == null));
            return outVar;
        }

        public void DebugWrite(int y, String s)
        {
            //if (y == 0 || y == maxCol || y == maxCol -1 || y == -1)
            //{
            //    Console.WriteLine(s);
            //}
        }

        public IEnumerable<HexagonData> List()
        {
            return this.dict.Values;
        }

        public List<HexagonData> GetAdjacent(HexagonData hex)
        {
            int parity = hex.Col % 2;

            var returnList = new List<HexagonData>(){
                Get(hex.Row-1, hex.Col),
                Get(hex.Row+1, hex.Col), 

                Get(hex.Row -1 + parity, hex.Col + 1),
                Get(hex.Row + parity, hex.Col + 1), 

                Get(hex.Row - 1 + parity, hex.Col - 1),
                Get(hex.Row + parity, hex.Col - 1),
                };
            return returnList.Where(h => h != null).ToList();
        }

        public List<HexagonData> GetBlankAdjacent(HexagonData hex)
        {
            return GetAdjacent(hex).Where(h => h.Terrain.TerrainEnum == TerrainEnum.NoTerrain).ToList();
        }

        public List<HexagonData> GetNonBlankAdjacent(HexagonData hex)
        {
            return GetAdjacent(hex).Where(h => h.Terrain.TerrainEnum != TerrainEnum.NoTerrain).ToList();
        }

    }
}
