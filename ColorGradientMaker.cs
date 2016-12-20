using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace howto_hexagonal_grid
{
    public class ColorGradient
    {
        private readonly Color low;
        private readonly Color high;

        public ColorGradient(Color low, Color high)
        {
            this.low = low;
            this.high = high;
        }

        public Color Get(double index)
        {
            int AScale = high.A - low.A;
            int RScale = high.R - low.R;
            int GScale = high.G - low.G;
            int BScale = high.B - low.B;

            int AValue = low.A + (int) (index * AScale);
            int RValue = low.R + (int) (index * RScale);
            int GValue = low.G + (int) (index * GScale);
            int BValue = low.B + (int) (index * BScale);

            return Color.FromArgb(AValue, RValue, GValue, BValue);
        }

    }
}
