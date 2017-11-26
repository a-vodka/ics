 public bool procces(bool start)
        {
            if (start)
            {

                if (Directory.Exists(dir)) Directory.Delete(dir, true);
                Directory.CreateDirectory(dir);
                createAnsData(dir);//поготовка макросов
                string AnsysDbName = "myjob";
                string ANSYS_DIR = Environment.GetEnvironmentVariable("ANSYS110_DIR");
                if (ANSYS_DIR == null) ANSYS_DIR = Environment.GetEnvironmentVariable("ANSYS120_DIR");
                if (ANSYS_DIR == null) { MessageBox.Show("Can't find ansys"); return false; }
                string np = Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
                string sysdir = Environment.GetEnvironmentVariable("ANSYS_SYSDIR");
                string AnsysRunParameters = "-b -j \"" + AnsysDbName + "\" -p ANE3FL -dir \"" + dir + "\" -np " + np + " -l en-us  -d win32 -i go.txt -o extracting_modes.log";
                Process myProcess = Process.Start(ANSYS_DIR + "\\bin\\"+sysdir+"\\ansys.exe", AnsysRunParameters);
                while (!myProcess.HasExited)
                {
                    Application.DoEvents();
                }
                myProcess.WaitForExit();
                myProcess.Close();
               
            }
            
            StreamReader sr;
            
            try
            {
                sr = new StreamReader(dir + "\\mizes.dat", System.Text.Encoding.Default);
            }
            catch
            {
                MessageBox.Show("Неизвестная ошибка");
                return false;
            }

            String line = "";

            int i=0;
            while ((line = sr.ReadLine()) != null)
            {
                if (i < 5)
                {
                    vonMizesStress[i] = Convert.ToDouble(line.Replace('.', ',').Trim());
                }
                else
                {
                    smax = Convert.ToDouble(line.Replace('.', ',').Trim());
                    line = sr.ReadLine();
                    smin = Convert.ToDouble(line.Replace('.', ',').Trim());
                    line = sr.ReadLine();
                    umax = Convert.ToDouble(line.Replace('.', ',').Trim());
                    line = sr.ReadLine();
                    umin = Convert.ToDouble(line.Replace('.', ',').Trim());
                }
                i++;
            }
            sr.Close();
            System.Threading.Thread.Sleep(200);//задерка 200 мс
            //Directory.Delete(dir, true);

            for (i = 1; i < 5; i++)
                concFactor[i - 1] = vonMizesStress[i] / vonMizesStress[0];

            return true;
        }





  List<System.Windows.Forms.Label> lables = new List<System.Windows.Forms.Label>();
        List<Panel> panels = new List<Panel>();
        uint[] colors = { 0xff0000FF, 0xff0fb3ff, 0xff00ffff, 0xff00ffb3, 0xff00ff00, 0xffb3ff00, 0xffffff00, 0xffffb300, 0xffff0000 };



 private double[] getLegend(double smin, double smax)
        {
            const int N=11;
            double min = 1 / 9 * smax;
            double delta = (smax - min) / 9;
            double []res = new double[N];
            res[0] = smin;
            for (int i=1;i<N;i++)
            {
                res[i] = (i * delta + min);
            }
            return res;
        }

        private void createLegendBox(System.Windows.Forms.Control obj,double[] arr)
        {
            lables.Clear();
            panels.Clear();
            System.Windows.Forms.Label l = new System.Windows.Forms.Label();
            l.Parent = obj;
            l.Text = arr[0].ToString("0.###e-00");
            l.Top = 50;
            l.Left = 100;
            lables.Add(l);

            for (int i = 1; i <= 9; i++)
            {
                l = new System.Windows.Forms.Label();
                l.Parent = obj;
                l.Text = arr[i].ToString("0.###e-00");
                l.Left = 100;
                l.Top = 50 + i * 25;
                lables.Add(l);
            }

            for (int i = 0; i < 9; i++)
            {
                Panel p = new Panel();

                p.Width = 70;
                p.Height = 22;
                p.Top = 60 + i * 25;
                p.Left = 20;
                p.BackColor = Color.FromArgb((int)colors[i]);
                p.Parent = obj;
                panels.Add(p);
            }
        }

        private void расчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (boltModel.procces(false))
            {
                string path = boltModel.dir + "\\";
                webBrowser1.Navigate(path + "myjob00.wrl");
                webBrowser2.Navigate(path + "myjob01.wrl");
                double[] stressLegend = getLegend(boltModel.smin, boltModel.smax);
                double[] usumLegend = getLegend(boltModel.umin, boltModel.umax);
                createLegendBox(this.groupBox1, stressLegend);
                createLegendBox(this.groupBox2, usumLegend);

                cl2.Text = boltModel.concFactor[0].ToString("#,0.000");
                cl3.Text = boltModel.concFactor[1].ToString("#,0.000");
                cl4.Text = boltModel.concFactor[2].ToString("#,0.000");
                cl5.Text = boltModel.concFactor[3].ToString("#,0.000");

                sl1.Text = boltModel.vonMizesStress[0].ToString("0.###e-00");
                sl2.Text = boltModel.vonMizesStress[1].ToString("0.###e-00");
                sl3.Text = boltModel.vonMizesStress[2].ToString("0.###e-00");
                sl4.Text = boltModel.vonMizesStress[3].ToString("0.###e-00");
                sl5.Text = boltModel.vonMizesStress[4].ToString("0.###e-00");


                GraphPane pane = zedGraphControl1.GraphPane;

                // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
                pane.CurveList.Clear();

                // Создадим список точек
                PointPairList list = new PointPairList();
                PointPairList list2 = new PointPairList();

                // Заполняем список точек
                double psi = 0,psi2=0, t = 0; 
                while (psi<1 || psi2<1)
                {
                    if (psi<1)
                        list.Add(t, psi);
                    list2.Add(t, psi2);
                    t += 10;
                    psi = boltModel.psi(t);
                    psi2 = boltModel.psi2(t);
                }

                // Создадим кривую с названием "Sinc", 
                // которая будет рисоваться голубым цветом (Color.Blue),
                // Опорные точки выделяться не будут (SymbolType.None)
                LineItem myCurve = pane.AddCurve("exp", list, Color.Blue, SymbolType.None);
                LineItem myCurve2 = pane.AddCurve("linear", list2, Color.Red, SymbolType.None);

                // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
                // В противном случае на рисунке будет показана только часть графика, 
                // которая умещается в интервалы по осям, установленные по умолчанию
                zedGraphControl1.AxisChange();

                // Обновляем график
                zedGraphControl1.Invalidate();

                GraphPane pane2 = zedGraphControl2.GraphPane;
                pane2.CurveList.Clear();
                PointPairList list3 = new PointPairList();
                list3.Add(0, 0);
                double fRaskrSt = boltModel.vonMizesStress[0]*boltModel.getValueByName("db4")*boltModel.getValueByName("db4")*Math.PI;
                FraskrSt.Text = fRaskrSt.ToString("0.###e-00") + " Н";
                list3.Add(boltModel.getValueByName("tempz"), fRaskrSt);
                LineItem myCurve3 = pane2.AddCurve("", list3, Color.Blue, SymbolType.None);
                zedGraphControl2.AxisChange();

                // Обновляем график
                zedGraphControl2.Invalidate();

            }
        }

    }
