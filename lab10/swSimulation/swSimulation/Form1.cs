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
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.cosworks;
namespace swSimulation
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
            int err = 0, war = 0;
            ModelDoc2 swModel = sw.OpenDoc6(Application.StartupPath + "\\beam.sldprt", 1, 0, "", ref err, ref war);

            double a = Convert.ToDouble(textBox1.Text);
            double b = Convert.ToDouble(textBox2.Text);
            double l = Convert.ToDouble(textBox3.Text);
            double E = Convert.ToDouble(textBox4.Text);
            double nu = Convert.ToDouble(textBox5.Text);
            double q = Convert.ToDouble(textBox6.Text);

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
//                pictureBox1.Load();
                pictureBox1.Update();
                pictureBox1.Invalidate();
            }

            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
                //                pictureBox1.Load();
                pictureBox2.Update();
                pictureBox2.Invalidate();
            }



            double sigma = 3 * q * l * l  / b / b / 1e6 ;
            label8.Text = sigma.ToString();

            swModel.Parameter("D1@Эскиз1").Value = a*1e3;
            swModel.Parameter("D2@Эскиз1").Value = b*1e3;
            swModel.Parameter("D1@Бобышка-Вытянуть1").Value = l*1e3;

            swModel.EditRebuild3();
            swModel.ViewZoomtofit2();

            CwAddincallback COSMOSObject = (CwAddincallback)sw.GetAddInObject("SldWorks.Simulation");
            ICosmosWorks cw = (ICosmosWorks)COSMOSObject.CosmosWorks;
            
            CWModelDoc ActDoc = cw.ActiveDoc;
            CWStudyManager StudyMngr = ActDoc.StudyManager;
            CWStudy Study = StudyMngr.GetStudy(0); // 0 - номер уже созданного расчета

            CWSolidManager SolidMgr = Study.SolidManager;
            int CompCount = SolidMgr.ComponentCount; // кол-во деталей в расчете
            int errorCode = 0;
            for (int i = 0; i < CompCount; i++) // Присваеваем свойства материала
            {

                CWSolidComponent SolidComponent = SolidMgr.GetComponentAt(i, out errorCode);
                CWSolidBody SolidBody = SolidComponent.GetSolidBodyAt(0, out errorCode);
                CWMaterial CWMat = SolidBody.GetSolidBodyMaterial();
                CWMat.MaterialUnits = 0;
                CWMat.MaterialName = "Alloy Steel";
                CWMat.SetPropertyByName("EX", E, 0);
                CWMat.SetPropertyByName("NUXY", nu, 0);
                SolidBody.SetSolidBodyMaterial(CWMat);
                SolidBody = null;
            }

            CWLoadsAndRestraintsManager LBCMgr = Study.LoadsAndRestraintsManager;
            for (int i = 0; i < LBCMgr.Count; i++)
            {
                CWLoadsAndRestraints CWP = LBCMgr.GetLoadsAndRestraints(i, out errorCode);
                if (CWP.Type == 1) // давление
                {
                    CWPressure CWPressure = (CWPressure)CWP;
                    CWPressure.PressureBeginEdit();
                    CWPressure.Unit = 3;
                    CWPressure.Value = q;
                    CWPressure.PressureEndEdit();
                }
            }

            double esize = 0, dtol = 0;
            CWMesh CwMesh = Study.Mesh; // Remesh geometry
            CwMesh.Quality = 1;
            CwMesh.GetDefaultElementSizeAndTolerance(0, out esize, out dtol);
            Study.CreateMesh(0, esize, dtol);

            errorCode = Study.RunAnalysis(); // Run analysis

            ModelDocExtension swModelDocExt = swModel.Extension;

            CWResults CWResult = Study.Results; // Get results
            
            ActDoc.DeleteAllDefaultStaticStudyPlots();

            //Create displacement plot
            CWPlot CWCFf = CWResult.CreatePlot((int)swsPlotResultTypes_e.swsResultDisplacementOrAmplitude, (int)swsStaticResultDisplacementComponentTypes_e.swsStaticDisplacement_URES, (int)swsUnit_e.swsUnitSI, false, out err);
            
            //Activate plot
            err = CWCFf.ActivatePlot();
            
            //Get min/max resultant displacements from plot
            object[] Disp = (object[])CWCFf.GetMinMaxResultValues(out err);
            
            double MinDisp = Convert.ToDouble(Disp[1]);
            double MaxDisp = Convert.ToDouble(Disp[3]);

            swModelDocExt.SaveAs(Application.StartupPath + "\\Displacement.analysis.jpg", 0, 0, null, ref err, ref war);

            //Create stress plot
            CWCFf = CWResult.CreatePlot((int)swsPlotResultTypes_e.swsResultStress, (int)swsStaticResultNodalStressComponentTypes_e.swsStaticNodalStress_VON, (int)swsUnit_e.swsUnitSI, false, out err);
            
            //Get min/max von Mises stresses from plot
            object[] Stress = (object[])CWCFf.GetMinMaxResultValues(out err);

            double MinStress = Convert.ToDouble(Stress[1]);
            double MaxStress = Convert.ToDouble(Stress[3]);            

            swModelDocExt.SaveAs(Application.StartupPath + "\\vmStress.analysis.jpg", 0, 0, null, ref err, ref war);
          

            pictureBox1.Image = System.Drawing.Image.FromFile(Application.StartupPath + "\\vmStress.analysis.jpg");
            pictureBox2.Image = System.Drawing.Image.FromFile(Application.StartupPath + "\\Displacement.analysis.jpg");

            label10.Text = (MaxStress / 1e6).ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
