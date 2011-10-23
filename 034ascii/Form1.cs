using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _034ascii
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The bitmap to be used as source data
        /// </summary>
        protected Bitmap m_bmpOriginal = null;
        protected const int BiggerSize = 100;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Open Image File";
            dlg.Filter = "Bitmap Files|*.bmp" +
                "|Gif Files|*.gif" +
                "|JPEG Files|*.jpg" +
                "|PNG Files|*.png" +
                "|TIFF Files|*.tif" +
                "|All image types|*.bmp;*.gif;*.jpg;*.png;*.tif";

            dlg.FilterIndex = 6;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_bmpOriginal = new Bitmap(Bitmap.FromFile(dlg.FileName));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

                pictureBox1.Image = m_bmpOriginal;
            }

            int w = m_bmpOriginal.Width;
            int h = m_bmpOriginal.Height;
            double factor = 1.0;

            if (w > h)
            {
                factor = (double)BiggerSize / (double)w;
            }
            else
            {
                factor = (double)BiggerSize / (double)h;
            }

            txtHeight.Text = Math.Floor((double)h * factor).ToString();
            txtWidth.Text = Math.Floor((double)w * factor).ToString();

        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            int w = int.Parse(txtWidth.Text);
            int h = int.Parse(txtHeight.Text);

            string text = AsciiArt.Process( m_bmpOriginal, w, h );
            Font   fnt  = AsciiArt.GetFont();

            Output dlgOut  = new Output();
            dlgOut.WndText = text;
            dlgOut.Fnt     = fnt;
            dlgOut.ShowDialog();
        }
    }
}
