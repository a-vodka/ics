using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelAutomation
{
    using Excel = Microsoft.Office.Interop.Excel;
    using Word = Microsoft.Office.Interop.Word;

    public partial class Form1 : Form
    {
        private Excel.Application excelApp;
        private Word.Application wordApp;
        
        public Form1()
        {
            InitializeComponent();
            excelApp = new Excel.Application();
            wordApp = new Word.Application();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            excelApp.Quit();
            wordApp.Quit();
        }

        private void ExcelButton_Click(object sender, EventArgs e)
        {
            excelApp.Visible = true;
            Excel.Workbook wb = excelApp.Workbooks.Add();
            Excel.Worksheet sheet = wb.ActiveSheet;

            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    sheet.Cells[i, j].Value = i * j;
                    if (i == j)
                        sheet.Cells[i, j].Font.Bold = true;
                }
            }

            sheet.Range["A1:J1"].Font.Bold = true;
            sheet.Range["A1:A10"].Font.Bold = true;
            sheet.Range["A1:J1"].Interior.Color = Color.LightGray;
            sheet.Range["A1:A10"].Interior.Color = Color.LightGray;

            float left = (float)sheet.Range["A15"].Left;
            float top = (float)sheet.Range["A15"].Top;

            sheet.Shapes.AddPicture(Application.StartupPath + @"/../../logo.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, left, top, 100, 100);
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            wordApp.Visible = true;
            Word.Document wd = wordApp.Documents.Add();
            wd.Range().Text = "Hello word!";
            wd.Range().Font.Size = 64;
        }
    }
}


