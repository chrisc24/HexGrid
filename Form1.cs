//#define FIG1
#define FIG34

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace howto_hexagonal_grid
{
    public partial class Form1 : Form
    {
        private readonly WorldGenParamters parameters = WorldGenParamters.Tiny();
        private readonly HexagonCollection collection;
        private readonly ColorGradient seaGradient = new ColorGradient(Color.LightBlue, Color.DarkBlue);
        private readonly ColorGradient landGradient = new ColorGradient(Color.SaddleBrown, Color.White);

        public Form1()
        {
            this.collection = TerrainController.GenerateWorld(parameters);

            InitializeComponent();
        }

        // Redraw the grid.
        private void picGrid_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the selected hexagons.
            foreach (HexagonData hex in collection.List())
            {
                //PaintTerrain(hex, e);
                PaintElevation(collection.MaxElevation, collection.MinElevation, hex, e);
            }

            // Draw the grid.
            DrawHexGrid(e.Graphics, Pens.Black,
                0, picGrid.ClientSize.Width,
                0, picGrid.ClientSize.Height);
        }

        private void PaintTerrain(HexagonData hex, PaintEventArgs e)
        {
            var color = hex.Terrain.Color;
            if (hex.Underwater())
            {
                color = Brushes.DarkBlue;
            }

            e.Graphics.FillPolygon(color, HexToPoints(hex.Row, hex.Col));

            var fontFamily = new FontFamily("Times New Roman");
            var font = new Font(fontFamily, parameters.FontSize, FontStyle.Regular, GraphicsUnit.Pixel);

            string coordinate = string.Format("[{0},{1}]", hex.Col, hex.Row);
            string glyph = hex.Terrain.Glyph;
            if (parameters.GlyphsEnabled)
            {
                e.Graphics.DrawString(hex.Terrain.Glyph, font, hex.Terrain.TextColor, MiddleText(hex.Row, hex.Col));
            }
        }

        private void PaintElevation(int max, int min, HexagonData hex, PaintEventArgs e)
        {
            Color elevationColor;
            if (hex.Elevation < 0)
            {
                double seaIndex = (double)hex.Elevation / (double)min;
                elevationColor = seaGradient.Get(seaIndex);
            }
            else
            {
                double landIndex = (double)hex.Elevation / (double)max;
                elevationColor = landGradient.Get(landIndex);
            }

            e.Graphics.FillPolygon(new SolidBrush(elevationColor), HexToPoints(hex.Row, hex.Col));
        }


        // Draw a hexagonal grid for the indicated area.
        // (You might be able to draw the hexagons without
        // drawing any duplicate edges, but this is a lot easier.)
        private void DrawHexGrid(Graphics gr, Pen pen,
            float xmin, float xmax, float ymin, float ymax)
        {
            // Loop until a hexagon won't fit.
            for (int row = 0; row < parameters.MaxRows; row++)
            {
                // Get the points for the row's first hexagon.
                PointF[] points = HexToPoints(row, 0);

                // Draw the row.
                for (int col = 0; col < parameters.MaxCol; col++)
                {
                    // Get the points for the row's next hexagon.
                    points = HexToPoints(row, col);

                    // If it fits vertically, draw it.
                    if (points[4].Y <= ymax)
                    {
                        gr.DrawPolygon(pen, points);
                    }
                }
            }
        }

        private void picGrid_Resize(object sender, EventArgs e)
        {
            picGrid.Refresh();
        }


        private void picGrid_MouseOver(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int row, col;
            PointToHex(e.X, e.Y, parameters.HexHeight, out row, out col);
            var hex = collection.Get(row, col);
            if (hex != null)
            {
                var coordinate = String.Format("[{0},{1}]:", col, row).PadRight(10);
                var terrainEnum = hex.Terrain.TerrainEnum.ToString().PadRight(15);
                var polar_temp_tropical = String.Format("{0},{1},{2}", TerrainAdjacentcy.Polarness(row, parameters.MaxRows), TerrainAdjacentcy.Temperateness(row, parameters.MaxRows), TerrainAdjacentcy.Tropicalness(row, parameters.MaxRows));

                LowerLeftIndicator.Text = string.Format("{0} {1} - {2} *** {3}", coordinate, terrainEnum, hex.Elevation, polar_temp_tropical);
            }
        }

        // Add the clicked hexagon to the Hexagons list.
        private void picGrid_MouseClick(object sender, MouseEventArgs e)
        {
                 int row, col;
                 PointToHex(e.X, e.Y, parameters.HexHeight, out row, out col);
                 var hex = collection.Get(row, col);
                 if (hex != null)
                 {
                     hex.Terrain = Terrain.EnumMap[TerrainEnum.Mountain];
                     foreach(var adjacentHex in collection.GetAdjacent(hex)){
                         adjacentHex.Terrain = Terrain.EnumMap[TerrainEnum.Tundra];
                     }
                 }
                 picGrid.Refresh();
        }


        // Return the width of a hexagon.
        private float HexWidth()
        {
            return (float)(4 * (parameters.HexHeight / 2 / Math.Sqrt(3)));
        }

        // Return the row and column of the hexagon at this point.
        private void PointToHex(float x, float y, float height,
            out int row, out int col)
        {
            // Find the test rectangle containing the point.
            float width = HexWidth();
            col = (int)(x / (width * 0.75f));

            if (col % 2 == 0)
                row = (int)(y / height);
            else
                row = (int)((y - height / 2) / height);

            // Find the test area.
            float testx = col * width * 0.75f;
            float testy = row * height;
            if (col % 2 == 1) testy += height / 2;

            // See if the point is above or
            // below the test hexagon on the left.
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    // The point is on the left edge of the test rectangle.
                    if (dy < 0) is_above = true;
                    if (dy > 0) is_below = true;
                }
                else if (dy < 0)
                {
                    // See if the point is above the test hexagon.
                    if (-dy / dx > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    // See if the point is below the test hexagon.
                    if (dy / dx > Math.Sqrt(3)) is_below = true;
                }
            }

            // Adjust the row and column if necessary.
            if (is_above)
            {
                if (col % 2 == 0) row--;
                col--;
            }
            else if (is_below)
            {
                if (col % 2 == 1) row++;
                col--;
            }
        }

        // Return the points that define the indicated hexagon.
        private PointF[] HexToPoints(int row, int col)
        {
            // Start with the leftmost corner of the upper left hexagon.
            float width = HexWidth();
            float y = parameters.HexHeight / 2;
            float x = 0;

            // Move down the required number of rows.
            y += row * parameters.HexHeight;

            // If the column is odd, move down half a hex more.
            if (col % 2 == 1) y += parameters.HexHeight / 2;

            // Move over for the column number.
            x += col * (width * 0.75f);

            // Generate the points.
            return new PointF[]
                {
                    new PointF(x, y),
                    new PointF(x + width * 0.25f, y - parameters.HexHeight / 2),
                    new PointF(x + width * 0.75f, y - parameters.HexHeight / 2),
                    new PointF(x + width, y),
                    new PointF(x + width * 0.75f, y + parameters.HexHeight / 2),
                    new PointF(x + width * 0.25f, y + parameters.HexHeight / 2),
                };
        }

        private PointF MiddleText(float row, float col)
        {
            // Start with the leftmost corner of the upper left hexagon.
            float width = HexWidth();
            float y = parameters.HexHeight / 2;
            float x = 0;

            // Move down the required number of rows.
            y += row * parameters.HexHeight;

            // If the column is odd, move down half a hex more.
            if (col % 2 == 1) y += parameters.HexHeight / 2;

            // Move over for the column number.
            x += col * (width * 0.75f);

            return new PointF(x + width * 0.20f, y - parameters.HexHeight / 6);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void heightToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
