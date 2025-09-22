using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtMouseX_TextChanged(object sender, EventArgs e)
        {

        }

        private void picCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            txtMouseX.Text = e.X.ToString();
            txtMouseY.Text = e.Y.ToString();
        }

        private void picCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            string coordinates = $"({e.X}, {e.Y})" + Environment.NewLine;
            txtClicks.AppendText(coordinates);
        }
    }
}
