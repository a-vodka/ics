using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_graph
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
Excel.Application excelApp = new Excel.Application();
excelApp.Visible = true;
            
Excel.Workbook wb = excelApp.Workbooks.Add();
Excel.Worksheet sheet = wb.ActiveSheet;

//add data
sheet.Cells[1, 1] = "";
sheet.Cells[1, 2] = "Student1";
sheet.Cells[1, 3] = "Student2";
sheet.Cells[1, 4] = "Student3";
sheet.Cells[2, 1] = "Term1";
sheet.Cells[2, 2] = "80";
sheet.Cells[2, 3] = "65";
sheet.Cells[2, 4] = "45";
sheet.Cells[3, 1] = "Term2";
sheet.Cells[3, 2] = "78";
sheet.Cells[3, 3] = "72";
sheet.Cells[3, 4] = "60";
sheet.Cells[4, 1] = "Term3";
sheet.Cells[4, 2] = "82";
sheet.Cells[4, 3] = "80";
sheet.Cells[4, 4] = "65";
sheet.Cells[5, 1] = "Term4";
sheet.Cells[5, 2] = "75";
sheet.Cells[5, 3] = "82";
sheet.Cells[5, 4] = "68";

Excel.Range chartRange = sheet.get_Range("A1", "d5");
Excel.ChartObjects xlCharts = (Excel.ChartObjects)sheet.ChartObjects();
Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
Excel.Chart chartPage = myChart.Chart;
chartPage.ChartWizard(chartRange, Excel.XlChartType.xlColumnClustered, Title:"Diagram title");

chartPage.Export(Application.StartupPath + @"./excel_chart_export.png", "png");
        }
    }
}
