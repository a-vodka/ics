﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SW_Rendering
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                pictureBox1.Load();
                pictureBox1.Update();
            }

            SldWorks sw = new SldWorks();
            sw.Visible = true;
            int err = 0, war = 0;
            ModelDoc2 swModel = sw.OpenDoc6(Application.StartupPath + "\\cylinder.sldprt", 1, 0, "", ref err, ref war);

            double[] props = swModel.GetMassProperties();

            textBox1.Text = props[5].ToString();

            IRayTraceRenderer swRayTraceRenderer = sw.GetRayTraceRenderer(1);
            if (swRayTraceRenderer == null)
            {
                MessageBox.Show(sw.GetExecutablePath() + ".\\sldraytracerenderu.dll");
                //int fileerror = sw.LoadAddIn(sw.GetExecutablePath()+"C:\Program Files\SolidWorks Corp\SolidWorks (2)sldraytracerenderu.dll");
                int fileerror = sw.LoadAddIn(@"C:\Program Files\SolidWorks Corp\SolidWorks (2)\sldraytracerenderu.dll");
                swRayTraceRenderer = sw.IGetRayTraceRenderer(1);
            }

            RayTraceRendererOptions swRayTraceRenderOptions = swRayTraceRenderer.RayTraceRendererOptions;

          textBox2.Clear();
          textBox2.Text += "\n Current rendering values ";
          textBox2.Text += "\n ImageWidth            = " + (swRayTraceRenderOptions.ImageWidth);
          textBox2.Text += "\n ImageFormat           = " + (swRayTraceRenderOptions.ImageFormat);
          textBox2.Text += "\n ImageHeight           = " + (swRayTraceRenderOptions.ImageHeight);
          textBox2.Text += "\n PreviewRenderQuality  = " + (swRayTraceRenderOptions.PreviewRenderQuality);
          textBox2.Text += "\n FinalRenderQuality    = " + (swRayTraceRenderOptions.FinalRenderQuality);
          textBox2.Text += "\n BloomEnabled          = " + (swRayTraceRenderOptions.BloomEnabled);
          textBox2.Text += "\n BloomThreshold        = " + (swRayTraceRenderOptions.BloomThreshold);
          textBox2.Text += "\n BloomRadius           = " + (swRayTraceRenderOptions.BloomRadius);
          textBox2.Text += "\n ContourEnabled        = " + (swRayTraceRenderOptions.ContourEnabled);
          textBox2.Text += "\n ShadedContour         = " + (swRayTraceRenderOptions.ShadedContour);
          textBox2.Text += "\n ContourLineThickness  = " + (swRayTraceRenderOptions.ContourLineThickness);
          textBox2.Text += "\n ContourLineColor      = " + (swRayTraceRenderOptions.ContourLineColor);

          bool status = swRayTraceRenderer.RenderToFile(Application.StartupPath+"./lter_1.jpg", 0, 0);
          status = swRayTraceRenderer.CloseRayTraceRender();

          pictureBox1.Load(Application.StartupPath+"./lter_1.jpg");
          
        }
    }
}

