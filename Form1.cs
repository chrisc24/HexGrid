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
        private readonly WorldController wc;

        public Form1()
        {
            wc = new WorldController();
            InitializeComponent();
        }

        // Redraw the grid.
        private void picGrid_Paint(object sender, PaintEventArgs e)
        {
            wc.Paint(e.Graphics);
        }

        private void picGrid_Resize(object sender, EventArgs e)
        {
            picGrid.Refresh();
        }


        private void picGrid_MouseOver(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            LowerLeftIndicator.Text = wc.MouseOver(e.X, e.Y);
        }

        // Add the clicked hexagon to the Hexagons list.
        private void picGrid_MouseClick(object sender, MouseEventArgs e)
        {
            wc.Click(e.X, e.Y);
            picGrid.Refresh();
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
