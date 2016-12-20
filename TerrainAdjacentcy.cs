using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace howto_hexagonal_grid
{
    public class TerrainAdjacentcy
    {
        private const int SeaLevel = 0;
        private const int MidLevel = 1000;
        private const int HighLevel = 3000;

        public class TerrainAltitudeAdjustment
        {
            public TerrainAltitudeAdjustment() {
                SeaLevel = 1.0;
                MidLevel = 1.0;
                HighLevel = 1.0;
            }

            public double SeaLevel { get; set; }

            public double MidLevel { get; set; }

            public double HighLevel { get; set; }

            public static TerrainAltitudeAdjustment Normal = new TerrainAltitudeAdjustment();
            public static TerrainAltitudeAdjustment SeaLevelBias = new TerrainAltitudeAdjustment() { SeaLevel = 1.2, MidLevel = .8, HighLevel = .1 };
            public static TerrainAltitudeAdjustment MidLevelBias = new TerrainAltitudeAdjustment() { SeaLevel = 0.7, MidLevel = 1.2, HighLevel = .7 };
            public static TerrainAltitudeAdjustment HighLevelBias = new TerrainAltitudeAdjustment() { SeaLevel = 0.0, MidLevel = .3, HighLevel = 1.0 };
        }
        
        public static Dictionary<TerrainEnum, TerrainAltitudeAdjustment> AltitudeAdjustments = BuildAdjustments();

        public static List<RandomElement<TerrainEnum>> ApplyAltitudeAdjustment(List<RandomElement<TerrainEnum>> initial, int altitude)
        {
            var toReturn = new List<RandomElement<TerrainEnum>>();

            foreach (var element in initial)
            {
                double multiplier;
                if (Sealevel(altitude)){
                    multiplier = AltitudeAdjustments[element.Element].SeaLevel;
                } else if (Midlevel(altitude)){
                    multiplier = AltitudeAdjustments[element.Element].MidLevel;
                } else {
                    multiplier = AltitudeAdjustments[element.Element].HighLevel;
                }

                toReturn.Add(new RandomElement<TerrainEnum>() { Element = element.Element, Prob = element.Prob * multiplier });
            }

            return toReturn;
        }

        private static bool Sealevel(int altitude)
        {
            return altitude < MidLevel;
        }

        private static bool Midlevel(int altitude)
        {
            return altitude < HighLevel;
        }

        private static Dictionary<TerrainEnum, TerrainAltitudeAdjustment> BuildAdjustments()
        {
            var toReturn = new Dictionary<TerrainEnum, TerrainAltitudeAdjustment>();

            toReturn.Add(TerrainEnum.NoTerrain,      TerrainAltitudeAdjustment.Normal);
            toReturn.Add(TerrainEnum.Grassland,      TerrainAltitudeAdjustment.Normal);
            toReturn.Add(TerrainEnum.Desert,         TerrainAltitudeAdjustment.Normal);
            toReturn.Add(TerrainEnum.Scrubland,      TerrainAltitudeAdjustment.MidLevelBias);
            toReturn.Add(TerrainEnum.Hills,          TerrainAltitudeAdjustment.MidLevelBias);
            toReturn.Add(TerrainEnum.Mountain,       TerrainAltitudeAdjustment.HighLevelBias);
            toReturn.Add(TerrainEnum.TemperateForest,TerrainAltitudeAdjustment.Normal);
            toReturn.Add(TerrainEnum.ColdForest,     TerrainAltitudeAdjustment.MidLevelBias);
            toReturn.Add(TerrainEnum.Jungle,         TerrainAltitudeAdjustment.SeaLevelBias);  
            toReturn.Add(TerrainEnum.Swamp,          TerrainAltitudeAdjustment.SeaLevelBias);  
            toReturn.Add(TerrainEnum.Tundra,         TerrainAltitudeAdjustment.Normal);
            toReturn.Add(TerrainEnum.Ice,            TerrainAltitudeAdjustment.Normal);

            return toReturn;
        }

        // Temperateness 
        // 10 <==> 50 <==> 90
        // Midpoints at 30 and 70
        // if(y < 45){
        //   return 
        //
        // Polarness
        // 0 <==> 20 80 <==> 100
        // Peaks at 0 and 100
        //
        // Tropicalness
        // 40 <==> 60
        // Peak at 50


        public class TerrainLatitudeAdjustment{
            public TerrainLatitudeAdjustment(){
                Polar = 1.0;
                Temperate = 1.0;
                Tropical = 1.0;
            }

            public double Polar { get; set; }

            public double Temperate { get; set; }

            public double Tropical { get; set; }

            public static TerrainLatitudeAdjustment NoBias = new TerrainLatitudeAdjustment();
            public static TerrainLatitudeAdjustment ExtremePolarBias = new TerrainLatitudeAdjustment() { Polar = 1.2, Temperate = .1, Tropical = 0.0 };
            public static TerrainLatitudeAdjustment PolarBias = new TerrainLatitudeAdjustment() { Polar = 1.2, Temperate = .6, Tropical = .1 };
            public static TerrainLatitudeAdjustment TemperateBias = new TerrainLatitudeAdjustment() { Polar = .6, Temperate = 1.2, Tropical = .8 };
            public static TerrainLatitudeAdjustment TropicalBias = new TerrainLatitudeAdjustment() { Polar = .1, Temperate = .7, Tropical = 1.2 };

        }

        public static Dictionary<TerrainEnum, TerrainLatitudeAdjustment> LatitudeAdjustments = L_BuildAdjustments();

        public static List<RandomElement<TerrainEnum>> ApplyLatitudeAdjustment(List<RandomElement<TerrainEnum>> initial, int thisRow, int maxRow)
        {
            var toReturn = new List<RandomElement<TerrainEnum>>();

            foreach (var element in initial)
            {
                double multiplier = 0.0;
                multiplier += Polarness(thisRow+1, maxRow) * LatitudeAdjustments[element.Element].Polar;
                multiplier += Tropicalness(thisRow+1, maxRow) * LatitudeAdjustments[element.Element].Tropical;
                multiplier += Temperateness(thisRow+1, maxRow) *LatitudeAdjustments[element.Element].Temperate;

                toReturn.Add(new RandomElement<TerrainEnum>() { Element = element.Element, Prob = element.Prob * multiplier });
            }

            return toReturn;
        }

        public static double Temperateness(int thisRow, int maxRow)
        {
            double toReturn;
            var lattitude = (double)thisRow * 100.0 / (double)maxRow;
            if (lattitude <= 30)
            {
                toReturn = 5 * lattitude -50 ;
            }
            else if (lattitude > 30 && lattitude <= 50)
            {
                toReturn = -5 * lattitude + 250;
            } else if (lattitude > 50 && lattitude <= 70){
                toReturn = 5 * lattitude - 250;
            }
            else
            {
                toReturn = -5 * lattitude + 450;
            }

            return Math.Max(0.0, toReturn/100);
        }

        public static double Polarness(int thisRow, int maxRow)
        {
            double toReturn;
            var lattitude = (double)thisRow * 100.0 / (double)maxRow;
            if (lattitude <= 50)
            {
                toReturn = -5 * lattitude + 100;
            }
            else
            {
                toReturn = 5 * lattitude - 400;
            }

            return Math.Max(0.0, toReturn/100.0);
        }

        public static double Tropicalness(int thisRow, int maxRow)
        {
            double toReturn;
            var lattitude = (double)thisRow * 100.0 / (double)maxRow;
            if (lattitude <= 50)
            {
                toReturn = 10 * lattitude - 400;
            }
            else
            {
                toReturn = -10 * lattitude + 600;
            }

            return Math.Max(0.0, toReturn / 100.0);
        }


        private static Dictionary<TerrainEnum, TerrainLatitudeAdjustment> L_BuildAdjustments(){
            var toReturn = new Dictionary<TerrainEnum, TerrainLatitudeAdjustment>();

            toReturn.Add(TerrainEnum.NoTerrain,      TerrainLatitudeAdjustment.NoBias);
            toReturn.Add(TerrainEnum.Grassland,      TerrainLatitudeAdjustment.TemperateBias);
            toReturn.Add(TerrainEnum.Desert,         TerrainLatitudeAdjustment.NoBias);
            toReturn.Add(TerrainEnum.Scrubland,      TerrainLatitudeAdjustment.NoBias);
            toReturn.Add(TerrainEnum.Hills,          TerrainLatitudeAdjustment.NoBias);
            toReturn.Add(TerrainEnum.Mountain,       TerrainLatitudeAdjustment.NoBias);
            toReturn.Add(TerrainEnum.TemperateForest,TerrainLatitudeAdjustment.TemperateBias);
            toReturn.Add(TerrainEnum.ColdForest,     TerrainLatitudeAdjustment.PolarBias);
            toReturn.Add(TerrainEnum.Jungle,         TerrainLatitudeAdjustment.TropicalBias);
            toReturn.Add(TerrainEnum.Swamp,          TerrainLatitudeAdjustment.TemperateBias);
            toReturn.Add(TerrainEnum.Tundra,         TerrainLatitudeAdjustment.ExtremePolarBias);
            toReturn.Add(TerrainEnum.Ice,            TerrainLatitudeAdjustment.ExtremePolarBias);

            return toReturn;
        }
    }
}
