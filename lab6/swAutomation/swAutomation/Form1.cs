using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;

namespace swAutomation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SldWorks sw = new SldWorks();
            sw.Visible = true;
            PartDoc Part = sw.NewPart();
            ModelDoc2 Model = sw.ActiveDoc;
            Model.InsertSketch2(true);
            Model.CreateEllipse2(0, 0, 0, 1, 0, 0, 0, 2, 0);
            Model.FeatureBoss2(true, false, false, 0, 0, 1, 0, true, false, true, false, 0.5, 0, false, false, false, false);
            
        }
    }
}
