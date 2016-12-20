using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace howto_hexagonal_grid
{
    public enum TerrainEnum
    {
        NoTerrain,

        Grassland,
        Desert,
        Scrubland,
        
        Hills,
        Mountain,

        TemperateForest,
        ColdForest,
        Jungle,
        Swamp,
        
        Tundra,
        Ice
    }

    public class Terrain
    {
        public Terrain()
        {
            this.TextColor = Brushes.Black;
        }

        public bool NoTerrainFlag { get; set; }

        public TerrainEnum TerrainEnum { get; set; }

        public String Glyph { get; set; }

        public Brush Color { get; set; }

        public Brush TextColor { get; set; }

        public static Dictionary<TerrainEnum, Terrain> EnumMap = BuildDict();


        private static Dictionary<TerrainEnum, Terrain> BuildDict(){
            var dict = new Dictionary<TerrainEnum, Terrain>();
            dict.Add(TerrainEnum.NoTerrain, Terrain.NoTerrain());

            dict.Add(TerrainEnum.Grassland, Terrain.Grassland());
            dict.Add(TerrainEnum.Desert, Terrain.Desert());
            dict.Add(TerrainEnum.Scrubland, Terrain.Scrubland());
        
            dict.Add(TerrainEnum.Hills, Terrain.Hills());
            dict.Add(TerrainEnum.Mountain, Terrain.Mountain());

            dict.Add(TerrainEnum.TemperateForest, Terrain.TemperateForest());
            dict.Add(TerrainEnum.ColdForest, Terrain.ColdForest());
            dict.Add(TerrainEnum.Jungle, Terrain.Jungle());
            dict.Add(TerrainEnum.Swamp, Terrain.Swamp());
        
            dict.Add(TerrainEnum.Tundra, Terrain.Tundra());
            dict.Add(TerrainEnum.Ice, Terrain.Ice());

            return dict;
        }

        private static Terrain NoTerrain()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.NoTerrain,                
                NoTerrainFlag = true,
                Color = Brushes.White,
            };
        }

        private static Terrain Grassland()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Grassland,
                Glyph =  '\u0df4'.ToString(),
                Color = Brushes.LightGreen,
            };
        }

        private static Terrain Desert()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Desert,
                Color = Brushes.Yellow,
                Glyph = "...",
            };
        }

        private static Terrain Scrubland()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Scrubland,
                Color = Brushes.Orange,
                Glyph = "+",
            };
        }

        private static Terrain Hills()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Hills,
                Color = Brushes.DarkKhaki,
                Glyph = '_'.ToString(),
            };
        }

        private static Terrain Mountain()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Mountain,
                Color = Brushes.DarkRed,
                Glyph = '\u20E4'.ToString(),
            };
        }

        private static Terrain TemperateForest()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.TemperateForest,
                Color = Brushes.GreenYellow,
                Glyph = "t",
            };
        }

        private static Terrain ColdForest()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.ColdForest,
                Glyph = '\u16AF'.ToString(),
                Color = Brushes.Green,
            };
        }

        private static Terrain Jungle()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Jungle,
                Color = Brushes.DarkGreen,
                Glyph = '\u2766'.ToString(),
            };
        }

        private static Terrain Swamp()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Swamp,
                Color = Brushes.DarkKhaki,
                Glyph = '\u2AE8'.ToString(),
                TextColor = Brushes.White,
            };
        }

        private static Terrain Tundra()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Tundra,
                Color = Brushes.DarkGray,
                Glyph = "*",
                TextColor = Brushes.White,
            };
        }

        private static Terrain Ice()
        {
            return new Terrain
            {
                TerrainEnum = TerrainEnum.Ice,
                Glyph = '\u2055'.ToString(),
                Color = Brushes.LightBlue,
            };
        }
    }
}
