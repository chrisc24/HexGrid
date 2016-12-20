using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace howto_hexagonal_grid
{
    public class WorldGenParamters
    {
        public int MaxRows { get; set; } // 30;
        public int MaxCol { get; set;} // 60;
        public int FontSize {get; set; } // 10;
        // The height of a hexagon.
        public float HexHeight {get; set;} // 20;

        public bool GlyphsEnabled {get; set;} //true

        public bool WrapEastWest { get; set; }

        public static WorldGenParamters Default() {
            return new WorldGenParamters() {
            MaxRows = 30,
            MaxCol = 60,
            HexHeight = 20,
            FontSize = 10,
            GlyphsEnabled = true,
            WrapEastWest = false,
            };
        }

       public static WorldGenParamters Tiny() {
            return new WorldGenParamters() {
                MaxRows = 115,
                MaxCol = 240,
                HexHeight = 8,
                FontSize = 10,
                GlyphsEnabled = false,
                WrapEastWest = true
            };
        }

    }
}
