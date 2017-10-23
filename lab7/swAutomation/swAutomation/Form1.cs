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

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                pictureBox1.Load();
                pictureBox1.Update();
            }

            double a = Convert.ToDouble(textBox1.Text) / 1e3;
            double b = Convert.ToDouble(textBox2.Text) / 1e3;
            double c = Convert.ToDouble(textBox3.Text) / 1e3;

            SldWorks sw = new SldWorks();
            sw.Visible = true;
            int err = 0, war = 0;
            ModelDoc2 swModel = sw.OpenDoc6(Application.StartupPath+"\\cube.sldprt", 1, 0, "", ref err, ref war);

            swModel.Parameter("D1@Эскиз1").SystemValue = a; // Задание размера
            swModel.Parameter("D2@Эскиз1").SystemValue = b; // Задание размера
            swModel.Parameter("D1@Бобышка-Вытянуть1").SystemValue = c; // Задание размера

            swModel.EditRebuild3(); // перестроение
            swModel.SaveAs(Application.StartupPath + "\\image.jpg");
            swModel.Save2(false); // сохранение
           // sw.CloseDoc("cube");
            pictureBox1.Load(Application.StartupPath + "\\image.jpg");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
