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


            double r1 = Convert.ToDouble(textBox1.Text) / 1e3;
            double r2 =Convert.ToDouble( textBox2.Text) / 1e3;
            double h = Convert.ToDouble(textBox3.Text) / 1e3;

            SldWorks sw = new SldWorks();
            sw.Visible = true;
            PartDoc Part = sw.NewPart();
            ModelDoc2 Model = sw.ActiveDoc;
            Model.InsertSketch2(true);

            Model.SelectByID("Спереди", "PLANE", 0, 0, 0); // Выделяем плоскость на которой будем рисовать
            
            Model.InsertSketch2(true); // Добавляем эскиз
            Model.CreateCircleByRadius2(0,0,0, r1); // рисуем окружность 1
            Model.CreateCircleByRadius2(0,0,0, r2); // рисуем окружность 2
            Model.FeatureBoss2(true, false, false,
                                0, 0, h, 0, true, false, true, false,
                                0, 0, false, false, false, false); // Бобышка - вытянуть

            /*Model.CreateEllipse2(0, 0, 0, 1, 0, 0, 0, 2, 0);
            Model.FeatureBoss2(true, false, false, 0, 0, 1, 0, true, false, true, false, 0.5, 0, false, false, false, false);*/
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
