using Contour_line;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PortalCraneModel
{
    public class PortalCraneModel : Form
    {
        public struct TAllDrawPoints
        {
            public IntPtr allDrawPoints; // массив точек
            public int drawCount; // число точек
        }

        public struct TStatePoint
        {
            public double fi;
            public double dfi_dt;
            public double x;
            public double dx_dt;
            public double t;
        }

        Random rand = new Random();

        public int buildCount = 0, dinPointsCount, drawStCount;

        const string dll = "PortalCraneCalc.dll";

        public double[] DrawPoints, DrawCriteria;


        public IntPtr ptrTAllDrawPoints, PtrNextPoint,
            ptrCriteria;

        public PortalCraneModel.TAllDrawPoints allPoints, criteria;

        public PortalCraneModel.TStatePoint CurrentPoint;

        public double M, m, l, R, g,
            h_fi, h_x, B, gamma, E, // parametri modeli

            fi, dfi_dt, x, dx_dt, // ishodnoye sostoyanie sistemi

            dt, t_start, t_stop, // parametri integrirovaniya

            lambda1_re, lambda1_im, lambda2_re, lambda2_im,
            lambda3_re, lambda3_im, lambda4_re, lambda4_im; // zhelaemiye korni har. polinoma

        public bool inDinamic;

        private TabControl tabControl1;
        private TabPage tabPage1;
        private GroupBox groupBox3;
        private Label textBox_run_time;
        private Label label16;
        private Button Button_setCalcParam;
        private CheckBox checkBox_dinDraw;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox textBox_drawStCount;
        private TextBox textBox_t_stop;
        private TextBox textBox_t_start;
        private TextBox textBox_step;
        private GroupBox groupBox1;
        private Button Button_setParam;
        private TextBox textBox_E;
        private TextBox textBox_gamma;
        private Label label_gamma;
        private Label label_E;
        private Label label_h_x;
        private Label label_B;
        private Label label7;
        private Label label_h_fi;
        private Label label_g;
        private Label label_R;
        private Label label_l;
        private Label label_mmal;
        private Label label_M;
        private TextBox textBox_h_fi;
        private TextBox textBox_g;
        private TextBox textBox_B;
        private TextBox textBox_R;
        private TextBox textBox_l;
        private TextBox textBox_h_x;
        private TextBox textBox_mmal;
        private TextBox textBox_M;
        private GroupBox roots_gbox;
        private Button button2;
        private Button btn_clear;
        private Label label11;
        private Label label10;
        private TextBox tbox_lambda4_im;
        private TextBox tbox_lambda4_re;
        private Button Button_runCalc;
        private TextBox tbox_lambda2_im;
        private TextBox tbox_lambda2_re;
        private Label label9;
        private Label label8;
        private Label label18;
        private Label root1;
        private TextBox tbox_lambda3_im;
        private TextBox tbox_lambda1_re;
        private TextBox tbox_lambda3_re;
        private TextBox tbox_lambda1_im;
        private Chart chart2;
        private Chart chart4;
        private Chart chart3;
        private Chart chart1;
        private CheckBox cBox_non_linear;
        private CheckBox cBox_Reg_on;
        private DataGridView dataGridView1;
        private GroupBox groupBox2;
        private Button Button_setInitVal;
        private Label labelfi;
        private Label labeldfi_dt;
        private Label labelx;
        private Label labeldx_dt;
        private TextBox textBox_fi;
        private TextBox textBox_dx_dt;
        private TextBox textBox_x;
        private TextBox textBox_dfi_dt;
        private TabPage tabPage2;
        private Timer Timer1;
        private PictureBox pBox_T_criterion;
        public TextBox xmin_t;
        public TextBox ymin_t;
        public TextBox xmax_t;
        public TextBox ymax_t;
        public TextBox DL_M3;
        public TextBox DL_N;
        public TextBox DL_M1;
        public TextBox DL_M2;
        private TextBox func_num_text;
        private Button button1;
        private Button button3;
        private PictureBox pBox_h2_criterion;
        private PictureBox pBox_h1_criterion;
        private PictureBox pBox_H_criterion;
        private PictureBox pBox_Vmax_criterion;
        private Label label5;
        private GroupBox groupBox4;
        private Label label_mu1;
        private Label label_M3;
        private Label label_M2;
        private Label label_mu2;
        private Label label_M1;
        private Label label_N;
        private Label label_sigma;
        private IContainer components;

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetModelParams(double _M, double _m, double _l, double _R, double _g,
            double _h_fi, double _h_x, double _B, double _gamma, double _E);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetModelLambdas(double _p1_re, double _p1_im, double _p2_re, double _p2_im,
            double _p3_re, double _p3_im, double _p4_re, double _p4_im);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Calc_regulator();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCalcParams(double _dt, double _t_start, double _t_stop, int _drawStCount, bool _inDinamic);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetScaleParams(double _xMax, double _yMax);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAllDrawPointsCount();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitAllPointsArray(IntPtr allDrawData);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteAllPointsArray(IntPtr allDrawData);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetAllDrawPoints(IntPtr ptrAllDrawPoints, bool system, bool reg);

        public PortalCraneModel()
        {
            this.InitializeComponent();
            this.allPoints = new PortalCraneModel.TAllDrawPoints();
            chart1.ChartAreas[0].AxisX.RoundAxisValues();
            chart1.ChartAreas[0].AxisY.RoundAxisValues();
            //chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;
            chart1.ChartAreas[0].AxisX.Title = "fi";
            chart1.ChartAreas[0].AxisY.Title = "dfi_dt";

            chart2.ChartAreas[0].AxisX.RoundAxisValues();
            chart2.ChartAreas[0].AxisY.RoundAxisValues();
            //chart2.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart2.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;
            chart2.ChartAreas[0].AxisX.Title = "x";
            chart2.ChartAreas[0].AxisY.Title = "dx_dt";

            chart3.ChartAreas[0].AxisX.RoundAxisValues();
            chart3.ChartAreas[0].AxisY.RoundAxisValues();
            chart3.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //chart3.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart3.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;
            chart3.ChartAreas[0].AxisX.Title = "t";
            chart3.ChartAreas[0].AxisY.Title = "fi";

            chart4.ChartAreas[0].AxisX.RoundAxisValues();
            chart4.ChartAreas[0].AxisY.RoundAxisValues();
            chart4.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //chart4.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart4.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Horizontal;
            chart4.ChartAreas[0].AxisX.Title = "t";
            chart4.ChartAreas[0].AxisY.Title = "x - x*";
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.dinPointsCount == this.allPoints.drawCount)
            {
                this.Timer1.Enabled = false;
                Marshal.FreeHGlobal(this.PtrNextPoint);
            }
            else
            {
                Marshal.StructureToPtr(this.CurrentPoint, this.PtrNextPoint, false);
                // PortalCraneModel.GetNextDrawPoint(this.PtrNextPoint);
                this.CurrentPoint = (PortalCraneModel.TStatePoint)Marshal.PtrToStructure(this.PtrNextPoint, typeof(PortalCraneModel.TStatePoint));
                this.DrawPoints[this.dinPointsCount * 5] = this.CurrentPoint.fi;
                this.DrawPoints[this.dinPointsCount * 5 + 1] = this.CurrentPoint.dfi_dt;
                this.DrawPoints[this.dinPointsCount * 5 + 2] = this.CurrentPoint.x;
                this.DrawPoints[this.dinPointsCount * 5 + 3] = this.CurrentPoint.dx_dt;
                this.DrawPoints[this.dinPointsCount * 5 + 4] = this.CurrentPoint.t;
                ++this.dinPointsCount;
                //this.PictureBox1.Refresh();
                //this.PictureBox2.Refresh();
                //this.PictureBox3.Refresh();
                //this.PictureBox4.Refresh();
            }
        }

        private void Button_setParam_Click(object sender, EventArgs e)
        {
            this.SetParam();
        }

        private void Button_setInitVal_Click(object sender, EventArgs e)
        {
            this.SetInitVal();
        }

        private void Button_setCalcParam_Click(object sender, EventArgs e)
        {
            this.SetCalcParam();
        }

        private void Button_runCalc_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            dataGridView1.Rows.Clear();

            // считываем:
            this.SetParam(); // параметры модели
            SetLambdas(); // корни желаемого хар. полинома
            this.SetCalcParam(); // параметры расчета
            this.SetInitVal(); // начальное состояние системы

            Calc_regulator(); // rasschitivaem parametri regulatora

            // определяем количество точек, которые будут отрисованы
            this.allPoints.drawCount = GetAllDrawPointsCount();
            // создаем управляемое хранилище
            this.DrawPoints = new double[allPoints.drawCount * 5];

            if (this.inDinamic)
            {
                this.CurrentPoint.fi = this.fi;
                this.CurrentPoint.dfi_dt = this.dfi_dt;
                this.CurrentPoint.x = this.x;
                this.CurrentPoint.dx_dt = this.dx_dt;
                this.CurrentPoint.t = this.t_start;
                this.DrawPoints[0] = this.fi;
                this.DrawPoints[1] = this.dfi_dt;
                this.DrawPoints[2] = this.x;
                this.DrawPoints[3] = this.dx_dt;
                this.DrawPoints[4] = this.t_start;
                this.dinPointsCount = 0;
                this.PtrNextPoint = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PortalCraneModel.TStatePoint)));
                this.Timer1.Enabled = true;
            }

            else
            {
                int sizeStruct = Marshal.SizeOf(typeof(PortalCraneModel.TAllDrawPoints)); // определяем размер управляемой структуры
                this.ptrTAllDrawPoints = Marshal.AllocHGlobal(sizeStruct); // выделяем память под неуправляемую структуру
                Marshal.StructureToPtr(this.allPoints, this.ptrTAllDrawPoints, false); // копируем данные из неуправляемой в управляемую
                PortalCraneModel.InitAllPointsArray(this.ptrTAllDrawPoints); // выделяем память под внутренний неуправляемый массив в неупр структуре
                PortalCraneModel.GetAllDrawPoints(this.ptrTAllDrawPoints, cBox_non_linear.Checked, cBox_Reg_on.Checked);
                this.allPoints = (PortalCraneModel.TAllDrawPoints)Marshal.PtrToStructure(this.ptrTAllDrawPoints, typeof(PortalCraneModel.TAllDrawPoints));
                Marshal.Copy(this.allPoints.allDrawPoints, this.DrawPoints, 0, this.allPoints.drawCount * 5);
                PortalCraneModel.DeleteAllPointsArray(this.ptrTAllDrawPoints);
                Marshal.FreeHGlobal(this.ptrTAllDrawPoints);

                chart1.Series.Add(buildCount.ToString());
                chart2.Series.Add(buildCount.ToString());
                chart3.Series.Add(buildCount.ToString());
                chart4.Series.Add(buildCount.ToString());


                chart1.Series[buildCount.ToString()].ChartType = SeriesChartType.Spline;
                chart2.Series[buildCount.ToString()].ChartType = SeriesChartType.Spline;
                chart3.Series[buildCount.ToString()].ChartType = SeriesChartType.Spline;
                chart4.Series[buildCount.ToString()].ChartType = SeriesChartType.Spline;

                chart1.Series[buildCount.ToString()].BorderWidth = 2;
                chart2.Series[buildCount.ToString()].BorderWidth = 2;
                chart3.Series[buildCount.ToString()].BorderWidth = 2;
                chart4.Series[buildCount.ToString()].BorderWidth = 2;

                //chart2.ChartAreas[0].AxisX.Minimum = DrawPoints[DrawPoints.Length - 3];
                chart3.ChartAreas[0].AxisX.Minimum = t_start;
                chart4.ChartAreas[0].AxisX.Minimum = t_start;
                //chart4.ChartAreas[0].AxisY.Minimum = DrawPoints[DrawPoints.Length - 3];

                chart1.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);
                chart2.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);
                chart3.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);
                chart4.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);

                for (int i = 0; i < DrawPoints.Length; i += 5)
                    chart1.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i], DrawPoints[i + 1]);

                for (int i = 2; i < DrawPoints.Length; i += 5)
                    chart2.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i], DrawPoints[i + 1]);

                for (int i = 0; i < DrawPoints.Length; i += 5)
                    chart3.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i + 4], DrawPoints[i]);

                for (int i = 2; i < DrawPoints.Length - 4; i += 5)
                    chart4.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i + 2], DrawPoints[i]);
                buildCount++;

                dataGridView1.ColumnCount = 6;
                dataGridView1.Columns[0].HeaderText = "n";
                dataGridView1.Columns[1].HeaderText = "t";
                dataGridView1.Columns[2].HeaderText = "fi";
                dataGridView1.Columns[3].HeaderText = "fi_dt";
                dataGridView1.Columns[4].HeaderText = "x";
                dataGridView1.Columns[5].HeaderText = "x_dt";

                int count = 0;
                for (int i = 0; i < DrawPoints.Length; i += 5)
                {
                    dataGridView1.Rows.Add(count, Math.Round(DrawPoints[i + 4], 12), Math.Round(DrawPoints[i], 12),
                        Math.Round(DrawPoints[i + 1], 12), Math.Round(DrawPoints[i + 2], 12), Math.Round(DrawPoints[i + 3], 12));
                    count++;
                }

                stopwatch.Stop();
                this.textBox_run_time.Text = stopwatch.Elapsed.TotalSeconds.ToString();
                this.textBox_run_time.BackColor = Color.LightGreen;

                //button1.Enabled = true;
            }
        }

        private void SetParam()
        {
            this.M = double.Parse(this.textBox_M.Text);
            this.m = double.Parse(this.textBox_mmal.Text);
            this.l = double.Parse(this.textBox_l.Text);
            this.R = double.Parse(this.textBox_R.Text);
            this.g = double.Parse(this.textBox_g.Text);
            this.h_fi = double.Parse(this.textBox_h_fi.Text);
            this.h_x = double.Parse(this.textBox_h_x.Text);
            this.B = double.Parse(this.textBox_B.Text);
            this.gamma = double.Parse(this.textBox_gamma.Text);
            this.E = double.Parse(this.textBox_E.Text);



            PortalCraneModel.SetModelParams(this.M, this.m, this.l, this.R, this.g,
                this.h_fi, this.h_x, this.B, this.gamma, this.E);
        }

        private void SetLambdas()
        {
            lambda1_re = double.Parse(tbox_lambda1_re.Text);
            lambda1_im = double.Parse(tbox_lambda1_im.Text);
            lambda2_re = double.Parse(tbox_lambda2_re.Text);
            lambda2_im = double.Parse(tbox_lambda2_im.Text);
            lambda3_re = double.Parse(tbox_lambda3_re.Text);
            lambda3_im = double.Parse(tbox_lambda3_im.Text);
            lambda4_re = double.Parse(tbox_lambda4_re.Text);
            lambda4_im = double.Parse(tbox_lambda4_im.Text);

            SetModelLambdas(lambda1_re, lambda1_im, lambda2_re, lambda2_im,
                lambda3_re, lambda3_im, lambda4_re, lambda4_im);
        }

        private void SetInitVal()
        {
            this.fi = double.Parse(this.textBox_fi.Text);
            this.dfi_dt = double.Parse(this.textBox_dfi_dt.Text);
            this.x = double.Parse(this.textBox_x.Text);
            this.dx_dt = double.Parse(this.textBox_dx_dt.Text);
            PortalCraneModel.SetInitParams(this.fi, this.dfi_dt, this.x, this.dx_dt);
        }

        private void SetCalcParam()
        {
            this.dt = double.Parse(this.textBox_step.Text);
            this.t_start = double.Parse(this.textBox_t_start.Text);
            this.t_stop = double.Parse(this.textBox_t_stop.Text);
            this.drawStCount = int.Parse(this.textBox_drawStCount.Text);
            this.inDinamic = this.checkBox_dinDraw.Checked;
            PortalCraneModel.SetCalcParams(this.dt, this.t_start, this.t_stop, this.drawStCount, this.inDinamic);
        }

        //private void PictureBox1_Paint(object sender, PaintEventArgs e)
        //{
        //    Rectangle clipRectangle1 = e.ClipRectangle;
        //    int right = clipRectangle1.Right;
        //    clipRectangle1 = e.ClipRectangle;
        //    int left = clipRectangle1.Left;
        //    double num1 = 0.5 * (double)(right + left);
        //    Rectangle clipRectangle2 = e.ClipRectangle;
        //    int bottom = clipRectangle2.Bottom;
        //    clipRectangle2 = e.ClipRectangle;
        //    int top = clipRectangle2.Top;
        //    double num2 = 0.5 * (double)(bottom + top);
        //    double height = (double)e.ClipRectangle.Height;
        //    double width = (double)e.ClipRectangle.Width;
        //    Pen pen = new Pen(Color.Black, 2f);
        //    int num3 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;
        //    for (int index = 1; index < num3; ++index)
        //    {
        //        switch (index * 5 / (int)byte.MaxValue % 3)
        //        {
        //            case 0:
        //                green = 0;
        //                blue = 0;
        //                red = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 1:
        //                red = 0;
        //                blue = 0;
        //                green = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 2:
        //                red = 0;
        //                green = 0;
        //                blue = index * 5 % (int)byte.MaxValue;
        //                break;
        //        }
        //        pen.Color = Color.FromArgb(red, green, blue);
        //        e.Graphics.DrawLine(pen, (float)(num1 + this.DrawPoints[(index - 1) * 5] / this.xMax * width / 2.0), (float)(num2 - this.DrawPoints[(index - 1) * 5 + 1] / this.yMax * height / 2.0), (float)(num1 + this.DrawPoints[index * 5] / this.xMax * width / 2.0), (float)(num2 - this.DrawPoints[index * 5 + 1] / this.yMax * height / 2.0));
        //    }
        //}

        //private void PictureBox2_Paint(object sender, PaintEventArgs e)
        //{
        //    double left = (double)e.ClipRectangle.Left;
        //    Rectangle clipRectangle = e.ClipRectangle;
        //    int bottom = clipRectangle.Bottom;
        //    clipRectangle = e.ClipRectangle;
        //    int top = clipRectangle.Top;
        //    double num1 = 0.5 * (double)(bottom + top);
        //    double height = (double)e.ClipRectangle.Height;
        //    double width = (double)e.ClipRectangle.Width;
        //    Pen pen = new Pen(Color.Black, 2f);
        //    int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;
        //    for (int index = 1; index < num2; ++index)
        //    {
        //        switch (index * 5 / (int)byte.MaxValue % 5)
        //        {
        //            case 0:
        //                green = 0;
        //                blue = 0;
        //                red = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 1:
        //                red = 0;
        //                blue = 0;
        //                green = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 2:
        //                red = 0;
        //                green = 0;
        //                blue = index * 5 % (int)byte.MaxValue;
        //                break;
        //        }
        //        pen.Color = Color.FromArgb(red, green, blue);
        //        e.Graphics.DrawLine(pen, (float)(left + this.DrawPoints[(index - 1) * 5 + 2] / this.xMax * width / 15.0), (float)(num1 - this.DrawPoints[(index - 1) * 5 + 3] / this.yMax * height / 2.0), (float)(left + this.DrawPoints[index * 5 + 2] / this.xMax * width / 15.0), (float)(num1 - this.DrawPoints[index * 5 + 3] / this.yMax * height / 2.0));
        //    }
        //}

        //private void PictureBox3_Paint(object sender, PaintEventArgs e)
        //{
        //    double left = (double)e.ClipRectangle.Left;
        //    Rectangle clipRectangle = e.ClipRectangle;
        //    int bottom = clipRectangle.Bottom;
        //    clipRectangle = e.ClipRectangle;
        //    int top = clipRectangle.Top;
        //    double num1 = 0.5 * (double)(bottom + top);
        //    double height = (double)e.ClipRectangle.Height;
        //    double width = (double)e.ClipRectangle.Width;
        //    Pen pen = new Pen(Color.Black, 2f);
        //    int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;
        //    for (int index = 1; index < num2; ++index)
        //    {
        //        switch (index * 5 / (int)byte.MaxValue % 5)
        //        {
        //            case 0:
        //                green = 0;
        //                blue = 0;
        //                red = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 1:
        //                red = 0;
        //                blue = 0;
        //                green = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 2:
        //                red = 0;
        //                green = 0;
        //                blue = index * 5 % (int)byte.MaxValue;
        //                break;
        //        }
        //        pen.Color = Color.FromArgb(red, green, blue);
        //        e.Graphics.DrawLine(pen, (float)(left + this.DrawPoints[(index - 1) * 5 + 4] / this.xMax * width / 70.0), (float)(num1 - this.DrawPoints[(index - 1) * 5] / this.yMax * height), (float)(left + this.DrawPoints[index * 5 + 4] / this.xMax * width / 70.0), (float)(num1 - this.DrawPoints[index * 5] / this.yMax * height));
        //    }
        //}

        //private void PictureBox4_Paint(object sender, PaintEventArgs e)
        //{
        //    double left = (double)e.ClipRectangle.Left;
        //    Rectangle clipRectangle = e.ClipRectangle;
        //    int bottom = clipRectangle.Bottom;
        //    clipRectangle = e.ClipRectangle;
        //    int top = clipRectangle.Top;
        //    double num1 = 0.5 * (double)(bottom + top);
        //    double height = (double)e.ClipRectangle.Height;
        //    double width = (double)e.ClipRectangle.Width;
        //    Pen pen = new Pen(Color.Black, 2f);
        //    int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;
        //    for (int index = 1; index < num2; ++index)
        //    {
        //        switch (index * 5 / (int)byte.MaxValue % 5)
        //        {
        //            case 0:
        //                green = 0;
        //                blue = 0;
        //                red = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 1:
        //                red = 0;
        //                blue = 0;
        //                green = index * 5 % (int)byte.MaxValue;
        //                break;
        //            case 2:
        //                red = 0;
        //                green = 0;
        //                blue = index * 5 % (int)byte.MaxValue;
        //                break;
        //        }
        //        pen.Color = Color.FromArgb(red, green, blue);
        //        e.Graphics.DrawLine(pen, (float)(left + this.DrawPoints[(index - 1) * 5 + 4] / this.xMax * width / 50.0), (float)(num1 - this.DrawPoints[(index - 1) * 5 + 2] / this.yMax * height / 50.0), (float)(left + this.DrawPoints[index * 5 + 4] / this.xMax * width / 50.0), (float)(num1 - this.DrawPoints[index * 5 + 2] / this.yMax * height / 50.0));
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_run_time = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.Button_setCalcParam = new System.Windows.Forms.Button();
            this.checkBox_dinDraw = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_drawStCount = new System.Windows.Forms.TextBox();
            this.textBox_t_stop = new System.Windows.Forms.TextBox();
            this.textBox_t_start = new System.Windows.Forms.TextBox();
            this.textBox_step = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Button_setParam = new System.Windows.Forms.Button();
            this.textBox_E = new System.Windows.Forms.TextBox();
            this.textBox_gamma = new System.Windows.Forms.TextBox();
            this.label_gamma = new System.Windows.Forms.Label();
            this.label_E = new System.Windows.Forms.Label();
            this.label_h_x = new System.Windows.Forms.Label();
            this.label_B = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_h_fi = new System.Windows.Forms.Label();
            this.label_g = new System.Windows.Forms.Label();
            this.label_R = new System.Windows.Forms.Label();
            this.label_l = new System.Windows.Forms.Label();
            this.label_mmal = new System.Windows.Forms.Label();
            this.label_M = new System.Windows.Forms.Label();
            this.textBox_h_fi = new System.Windows.Forms.TextBox();
            this.textBox_g = new System.Windows.Forms.TextBox();
            this.textBox_B = new System.Windows.Forms.TextBox();
            this.textBox_R = new System.Windows.Forms.TextBox();
            this.textBox_l = new System.Windows.Forms.TextBox();
            this.textBox_h_x = new System.Windows.Forms.TextBox();
            this.textBox_mmal = new System.Windows.Forms.TextBox();
            this.textBox_M = new System.Windows.Forms.TextBox();
            this.roots_gbox = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbox_lambda4_im = new System.Windows.Forms.TextBox();
            this.tbox_lambda4_re = new System.Windows.Forms.TextBox();
            this.Button_runCalc = new System.Windows.Forms.Button();
            this.tbox_lambda2_im = new System.Windows.Forms.TextBox();
            this.tbox_lambda2_re = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.root1 = new System.Windows.Forms.Label();
            this.tbox_lambda3_im = new System.Windows.Forms.TextBox();
            this.tbox_lambda1_re = new System.Windows.Forms.TextBox();
            this.tbox_lambda3_re = new System.Windows.Forms.TextBox();
            this.tbox_lambda1_im = new System.Windows.Forms.TextBox();
            this.cBox_non_linear = new System.Windows.Forms.CheckBox();
            this.cBox_Reg_on = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Button_setInitVal = new System.Windows.Forms.Button();
            this.labelfi = new System.Windows.Forms.Label();
            this.labeldfi_dt = new System.Windows.Forms.Label();
            this.labelx = new System.Windows.Forms.Label();
            this.labeldx_dt = new System.Windows.Forms.Label();
            this.textBox_fi = new System.Windows.Forms.TextBox();
            this.textBox_dx_dt = new System.Windows.Forms.TextBox();
            this.textBox_x = new System.Windows.Forms.TextBox();
            this.textBox_dfi_dt = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label_mu1 = new System.Windows.Forms.Label();
            this.xmin_t = new System.Windows.Forms.TextBox();
            this.label_M3 = new System.Windows.Forms.Label();
            this.xmax_t = new System.Windows.Forms.TextBox();
            this.label_M2 = new System.Windows.Forms.Label();
            this.label_mu2 = new System.Windows.Forms.Label();
            this.label_M1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ymin_t = new System.Windows.Forms.TextBox();
            this.label_N = new System.Windows.Forms.Label();
            this.label_sigma = new System.Windows.Forms.Label();
            this.DL_N = new System.Windows.Forms.TextBox();
            this.DL_M2 = new System.Windows.Forms.TextBox();
            this.DL_M1 = new System.Windows.Forms.TextBox();
            this.DL_M3 = new System.Windows.Forms.TextBox();
            this.pBox_Vmax_criterion = new System.Windows.Forms.PictureBox();
            this.pBox_h2_criterion = new System.Windows.Forms.PictureBox();
            this.pBox_h1_criterion = new System.Windows.Forms.PictureBox();
            this.pBox_H_criterion = new System.Windows.Forms.PictureBox();
            this.pBox_T_criterion = new System.Windows.Forms.PictureBox();
            this.func_num_text = new System.Windows.Forms.TextBox();
            this.ymax_t = new System.Windows.Forms.TextBox();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.roots_gbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_Vmax_criterion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_h2_criterion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_h1_criterion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_H_criterion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_T_criterion)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1364, 767);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chart2);
            this.tabPage1.Controls.Add(this.chart4);
            this.tabPage1.Controls.Add(this.chart3);
            this.tabPage1.Controls.Add(this.chart1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.roots_gbox);
            this.tabPage1.Controls.Add(this.cBox_non_linear);
            this.tabPage1.Controls.Add(this.cBox_Reg_on);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1356, 741);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Модель";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chart2
            // 
            this.chart2.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart2.Legends.Add(legend1);
            this.chart2.Location = new System.Drawing.Point(849, 8);
            this.chart2.Name = "chart2";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart2.Series.Add(series1);
            this.chart2.Size = new System.Drawing.Size(500, 350);
            this.chart2.TabIndex = 55;
            this.chart2.Text = "chart2";
            // 
            // chart4
            // 
            this.chart4.BackColor = System.Drawing.SystemColors.Control;
            chartArea2.Name = "ChartArea1";
            this.chart4.ChartAreas.Add(chartArea2);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chart4.Legends.Add(legend2);
            this.chart4.Location = new System.Drawing.Point(849, 364);
            this.chart4.Name = "chart4";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            series2.XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.chart4.Series.Add(series2);
            this.chart4.Size = new System.Drawing.Size(500, 350);
            this.chart4.TabIndex = 52;
            this.chart4.Text = "chart1";
            // 
            // chart3
            // 
            this.chart3.BackColor = System.Drawing.SystemColors.Control;
            chartArea3.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chart3.Legends.Add(legend3);
            this.chart3.Location = new System.Drawing.Point(343, 364);
            this.chart3.Name = "chart3";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart3.Series.Add(series3);
            this.chart3.Size = new System.Drawing.Size(500, 350);
            this.chart3.TabIndex = 53;
            this.chart3.Text = "chart1";
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.SystemColors.Control;
            chartArea4.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea4);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chart1.Legends.Add(legend4);
            this.chart1.Location = new System.Drawing.Point(343, 8);
            this.chart1.Name = "chart1";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chart1.Series.Add(series4);
            this.chart1.Size = new System.Drawing.Size(500, 350);
            this.chart1.TabIndex = 54;
            this.chart1.Text = "chart1";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.textBox_run_time);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.Button_setCalcParam);
            this.groupBox3.Controls.Add(this.checkBox_dinDraw);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textBox_drawStCount);
            this.groupBox3.Controls.Add(this.textBox_t_stop);
            this.groupBox3.Controls.Add(this.textBox_t_start);
            this.groupBox3.Controls.Add(this.textBox_step);
            this.groupBox3.Location = new System.Drawing.Point(147, 8);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(191, 179);
            this.groupBox3.TabIndex = 50;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Параметры расчета";
            // 
            // textBox_run_time
            // 
            this.textBox_run_time.AutoSize = true;
            this.textBox_run_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_run_time.Location = new System.Drawing.Point(108, 152);
            this.textBox_run_time.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.textBox_run_time.Name = "textBox_run_time";
            this.textBox_run_time.Size = new System.Drawing.Size(0, 20);
            this.textBox_run_time.TabIndex = 11;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 155);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(108, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Время выполнения:";
            // 
            // Button_setCalcParam
            // 
            this.Button_setCalcParam.Location = new System.Drawing.Point(4, 130);
            this.Button_setCalcParam.Margin = new System.Windows.Forms.Padding(2);
            this.Button_setCalcParam.Name = "Button_setCalcParam";
            this.Button_setCalcParam.Size = new System.Drawing.Size(75, 25);
            this.Button_setCalcParam.TabIndex = 9;
            this.Button_setCalcParam.Text = "Принять";
            this.Button_setCalcParam.UseVisualStyleBackColor = true;
            this.Button_setCalcParam.Click += new System.EventHandler(this.Button_setCalcParam_Click);
            // 
            // checkBox_dinDraw
            // 
            this.checkBox_dinDraw.AutoSize = true;
            this.checkBox_dinDraw.Location = new System.Drawing.Point(7, 108);
            this.checkBox_dinDraw.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_dinDraw.Name = "checkBox_dinDraw";
            this.checkBox_dinDraw.Size = new System.Drawing.Size(136, 17);
            this.checkBox_dinDraw.TabIndex = 8;
            this.checkBox_dinDraw.Text = "Рисовать в динамике";
            this.checkBox_dinDraw.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 88);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Шагов инт. в шаге";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "t_stop:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "t_start:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Шаг инт.:";
            // 
            // textBox_drawStCount
            // 
            this.textBox_drawStCount.Location = new System.Drawing.Point(112, 85);
            this.textBox_drawStCount.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_drawStCount.Name = "textBox_drawStCount";
            this.textBox_drawStCount.Size = new System.Drawing.Size(76, 20);
            this.textBox_drawStCount.TabIndex = 3;
            this.textBox_drawStCount.Text = "10";
            this.textBox_drawStCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_t_stop
            // 
            this.textBox_t_stop.Location = new System.Drawing.Point(112, 63);
            this.textBox_t_stop.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_t_stop.Name = "textBox_t_stop";
            this.textBox_t_stop.Size = new System.Drawing.Size(76, 20);
            this.textBox_t_stop.TabIndex = 2;
            this.textBox_t_stop.Text = "100";
            this.textBox_t_stop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_t_start
            // 
            this.textBox_t_start.Location = new System.Drawing.Point(112, 40);
            this.textBox_t_start.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_t_start.Name = "textBox_t_start";
            this.textBox_t_start.Size = new System.Drawing.Size(76, 20);
            this.textBox_t_start.TabIndex = 1;
            this.textBox_t_start.Text = "0";
            this.textBox_t_start.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_step
            // 
            this.textBox_step.Location = new System.Drawing.Point(112, 17);
            this.textBox_step.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_step.Name = "textBox_step";
            this.textBox_step.Size = new System.Drawing.Size(76, 20);
            this.textBox_step.TabIndex = 0;
            this.textBox_step.Text = "0,005";
            this.textBox_step.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.Button_setParam);
            this.groupBox1.Controls.Add(this.textBox_E);
            this.groupBox1.Controls.Add(this.textBox_gamma);
            this.groupBox1.Controls.Add(this.label_gamma);
            this.groupBox1.Controls.Add(this.label_E);
            this.groupBox1.Controls.Add(this.label_h_x);
            this.groupBox1.Controls.Add(this.label_B);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label_h_fi);
            this.groupBox1.Controls.Add(this.label_g);
            this.groupBox1.Controls.Add(this.label_R);
            this.groupBox1.Controls.Add(this.label_l);
            this.groupBox1.Controls.Add(this.label_mmal);
            this.groupBox1.Controls.Add(this.label_M);
            this.groupBox1.Controls.Add(this.textBox_h_fi);
            this.groupBox1.Controls.Add(this.textBox_g);
            this.groupBox1.Controls.Add(this.textBox_B);
            this.groupBox1.Controls.Add(this.textBox_R);
            this.groupBox1.Controls.Add(this.textBox_l);
            this.groupBox1.Controls.Add(this.textBox_h_x);
            this.groupBox1.Controls.Add(this.textBox_mmal);
            this.groupBox1.Controls.Add(this.textBox_M);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(135, 270);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры модели";
            // 
            // Button_setParam
            // 
            this.Button_setParam.Location = new System.Drawing.Point(55, 245);
            this.Button_setParam.Margin = new System.Windows.Forms.Padding(2);
            this.Button_setParam.Name = "Button_setParam";
            this.Button_setParam.Size = new System.Drawing.Size(75, 25);
            this.Button_setParam.TabIndex = 27;
            this.Button_setParam.Text = "Принять";
            this.Button_setParam.UseVisualStyleBackColor = true;
            this.Button_setParam.Click += new System.EventHandler(this.Button_setParam_Click);
            // 
            // textBox_E
            // 
            this.textBox_E.Location = new System.Drawing.Point(55, 222);
            this.textBox_E.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_E.Name = "textBox_E";
            this.textBox_E.Size = new System.Drawing.Size(76, 20);
            this.textBox_E.TabIndex = 26;
            this.textBox_E.Text = "0,00767";
            // 
            // textBox_gamma
            // 
            this.textBox_gamma.Location = new System.Drawing.Point(55, 199);
            this.textBox_gamma.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_gamma.Name = "textBox_gamma";
            this.textBox_gamma.Size = new System.Drawing.Size(76, 20);
            this.textBox_gamma.TabIndex = 25;
            this.textBox_gamma.Text = "4,481";
            // 
            // label_gamma
            // 
            this.label_gamma.AutoSize = true;
            this.label_gamma.Location = new System.Drawing.Point(4, 202);
            this.label_gamma.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_gamma.Name = "label_gamma";
            this.label_gamma.Size = new System.Drawing.Size(44, 13);
            this.label_gamma.TabIndex = 24;
            this.label_gamma.Text = "gamma:";
            // 
            // label_E
            // 
            this.label_E.AutoSize = true;
            this.label_E.Location = new System.Drawing.Point(4, 226);
            this.label_E.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_E.Name = "label_E";
            this.label_E.Size = new System.Drawing.Size(17, 13);
            this.label_E.TabIndex = 23;
            this.label_E.Text = "E:";
            // 
            // label_h_x
            // 
            this.label_h_x.AutoSize = true;
            this.label_h_x.Location = new System.Drawing.Point(4, 158);
            this.label_h_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_h_x.Name = "label_h_x";
            this.label_h_x.Size = new System.Drawing.Size(27, 13);
            this.label_h_x.TabIndex = 21;
            this.label_h_x.Text = "h_x:";
            // 
            // label_B
            // 
            this.label_B.AutoSize = true;
            this.label_B.Location = new System.Drawing.Point(4, 180);
            this.label_B.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_B.Name = "label_B";
            this.label_B.Size = new System.Drawing.Size(17, 13);
            this.label_B.TabIndex = 20;
            this.label_B.Text = "B:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 158);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 13);
            this.label7.TabIndex = 19;
            // 
            // label_h_fi
            // 
            this.label_h_fi.AutoSize = true;
            this.label_h_fi.Location = new System.Drawing.Point(4, 135);
            this.label_h_fi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_h_fi.Name = "label_h_fi";
            this.label_h_fi.Size = new System.Drawing.Size(27, 13);
            this.label_h_fi.TabIndex = 18;
            this.label_h_fi.Text = "h_fi:";
            // 
            // label_g
            // 
            this.label_g.AutoSize = true;
            this.label_g.Location = new System.Drawing.Point(4, 112);
            this.label_g.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_g.Name = "label_g";
            this.label_g.Size = new System.Drawing.Size(16, 13);
            this.label_g.TabIndex = 17;
            this.label_g.Text = "g:";
            // 
            // label_R
            // 
            this.label_R.AutoSize = true;
            this.label_R.Location = new System.Drawing.Point(4, 89);
            this.label_R.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_R.Name = "label_R";
            this.label_R.Size = new System.Drawing.Size(18, 13);
            this.label_R.TabIndex = 16;
            this.label_R.Text = "R:";
            // 
            // label_l
            // 
            this.label_l.AutoSize = true;
            this.label_l.Location = new System.Drawing.Point(4, 67);
            this.label_l.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_l.Name = "label_l";
            this.label_l.Size = new System.Drawing.Size(12, 13);
            this.label_l.TabIndex = 15;
            this.label_l.Text = "l:";
            // 
            // label_mmal
            // 
            this.label_mmal.AutoSize = true;
            this.label_mmal.Location = new System.Drawing.Point(4, 44);
            this.label_mmal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_mmal.Name = "label_mmal";
            this.label_mmal.Size = new System.Drawing.Size(18, 13);
            this.label_mmal.TabIndex = 14;
            this.label_mmal.Text = "m:";
            // 
            // label_M
            // 
            this.label_M.AutoSize = true;
            this.label_M.Location = new System.Drawing.Point(4, 21);
            this.label_M.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_M.Name = "label_M";
            this.label_M.Size = new System.Drawing.Size(19, 13);
            this.label_M.TabIndex = 13;
            this.label_M.Text = "M:";
            // 
            // textBox_h_fi
            // 
            this.textBox_h_fi.Location = new System.Drawing.Point(55, 131);
            this.textBox_h_fi.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_h_fi.Name = "textBox_h_fi";
            this.textBox_h_fi.Size = new System.Drawing.Size(76, 20);
            this.textBox_h_fi.TabIndex = 7;
            this.textBox_h_fi.Text = "0,0024";
            // 
            // textBox_g
            // 
            this.textBox_g.Location = new System.Drawing.Point(55, 108);
            this.textBox_g.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_g.Name = "textBox_g";
            this.textBox_g.Size = new System.Drawing.Size(76, 20);
            this.textBox_g.TabIndex = 3;
            this.textBox_g.Text = "9,81";
            // 
            // textBox_B
            // 
            this.textBox_B.Location = new System.Drawing.Point(55, 176);
            this.textBox_B.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_B.Name = "textBox_B";
            this.textBox_B.Size = new System.Drawing.Size(76, 20);
            this.textBox_B.TabIndex = 6;
            this.textBox_B.Text = "0,024";
            // 
            // textBox_R
            // 
            this.textBox_R.Location = new System.Drawing.Point(55, 85);
            this.textBox_R.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_R.Name = "textBox_R";
            this.textBox_R.Size = new System.Drawing.Size(76, 20);
            this.textBox_R.TabIndex = 2;
            this.textBox_R.Text = "2,6";
            // 
            // textBox_l
            // 
            this.textBox_l.Location = new System.Drawing.Point(55, 63);
            this.textBox_l.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_l.Name = "textBox_l";
            this.textBox_l.Size = new System.Drawing.Size(76, 20);
            this.textBox_l.TabIndex = 5;
            this.textBox_l.Text = "0,641";
            // 
            // textBox_h_x
            // 
            this.textBox_h_x.Location = new System.Drawing.Point(55, 154);
            this.textBox_h_x.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_h_x.Name = "textBox_h_x";
            this.textBox_h_x.Size = new System.Drawing.Size(76, 20);
            this.textBox_h_x.TabIndex = 4;
            this.textBox_h_x.Text = "5,4";
            // 
            // textBox_mmal
            // 
            this.textBox_mmal.Location = new System.Drawing.Point(55, 40);
            this.textBox_mmal.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_mmal.Name = "textBox_mmal";
            this.textBox_mmal.Size = new System.Drawing.Size(76, 20);
            this.textBox_mmal.TabIndex = 1;
            this.textBox_mmal.Text = "0,019";
            // 
            // textBox_M
            // 
            this.textBox_M.Location = new System.Drawing.Point(55, 17);
            this.textBox_M.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_M.Name = "textBox_M";
            this.textBox_M.Size = new System.Drawing.Size(76, 20);
            this.textBox_M.TabIndex = 0;
            this.textBox_M.Text = "1,073";
            // 
            // roots_gbox
            // 
            this.roots_gbox.BackColor = System.Drawing.SystemColors.Control;
            this.roots_gbox.Controls.Add(this.button3);
            this.roots_gbox.Controls.Add(this.button2);
            this.roots_gbox.Controls.Add(this.btn_clear);
            this.roots_gbox.Controls.Add(this.label11);
            this.roots_gbox.Controls.Add(this.label10);
            this.roots_gbox.Controls.Add(this.tbox_lambda4_im);
            this.roots_gbox.Controls.Add(this.tbox_lambda4_re);
            this.roots_gbox.Controls.Add(this.Button_runCalc);
            this.roots_gbox.Controls.Add(this.tbox_lambda2_im);
            this.roots_gbox.Controls.Add(this.tbox_lambda2_re);
            this.roots_gbox.Controls.Add(this.label9);
            this.roots_gbox.Controls.Add(this.label8);
            this.roots_gbox.Controls.Add(this.label18);
            this.roots_gbox.Controls.Add(this.root1);
            this.roots_gbox.Controls.Add(this.tbox_lambda3_im);
            this.roots_gbox.Controls.Add(this.tbox_lambda1_re);
            this.roots_gbox.Controls.Add(this.tbox_lambda3_re);
            this.roots_gbox.Controls.Add(this.tbox_lambda1_im);
            this.roots_gbox.Location = new System.Drawing.Point(147, 192);
            this.roots_gbox.Name = "roots_gbox";
            this.roots_gbox.Size = new System.Drawing.Size(191, 237);
            this.roots_gbox.TabIndex = 51;
            this.roots_gbox.TabStop = false;
            this.roots_gbox.Text = "Желаемые корни хар. полинома";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(17, 146);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 25);
            this.button3.TabIndex = 46;
            this.button3.Text = "Сл. корни";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(104, 146);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 25);
            this.button2.TabIndex = 45;
            this.button2.Text = "Сбросить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(0, 204);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(188, 25);
            this.btn_clear.TabIndex = 35;
            this.btn_clear.Text = "Очистить";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.Btn_clear_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 123);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 44;
            this.label11.Text = "lambda_4:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 13);
            this.label10.TabIndex = 43;
            this.label10.Text = "lambda_2:";
            // 
            // tbox_lambda4_im
            // 
            this.tbox_lambda4_im.Location = new System.Drawing.Point(133, 120);
            this.tbox_lambda4_im.Name = "tbox_lambda4_im";
            this.tbox_lambda4_im.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda4_im.TabIndex = 42;
            this.tbox_lambda4_im.Text = "0";
            this.tbox_lambda4_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda4_re
            // 
            this.tbox_lambda4_re.Location = new System.Drawing.Point(83, 120);
            this.tbox_lambda4_re.Name = "tbox_lambda4_re";
            this.tbox_lambda4_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda4_re.TabIndex = 41;
            this.tbox_lambda4_re.Text = "0";
            this.tbox_lambda4_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Button_runCalc
            // 
            this.Button_runCalc.Location = new System.Drawing.Point(0, 174);
            this.Button_runCalc.Margin = new System.Windows.Forms.Padding(2);
            this.Button_runCalc.Name = "Button_runCalc";
            this.Button_runCalc.Size = new System.Drawing.Size(188, 25);
            this.Button_runCalc.TabIndex = 19;
            this.Button_runCalc.Text = "Рассчитать";
            this.Button_runCalc.UseVisualStyleBackColor = true;
            this.Button_runCalc.Click += new System.EventHandler(this.Button_runCalc_Click);
            // 
            // tbox_lambda2_im
            // 
            this.tbox_lambda2_im.Location = new System.Drawing.Point(133, 68);
            this.tbox_lambda2_im.Name = "tbox_lambda2_im";
            this.tbox_lambda2_im.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda2_im.TabIndex = 40;
            this.tbox_lambda2_im.Text = "0";
            this.tbox_lambda2_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda2_re
            // 
            this.tbox_lambda2_re.Location = new System.Drawing.Point(83, 68);
            this.tbox_lambda2_re.Name = "tbox_lambda2_re";
            this.tbox_lambda2_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda2_re.TabIndex = 39;
            this.tbox_lambda2_re.Text = "0";
            this.tbox_lambda2_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(130, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 38;
            this.label9.Text = "Im:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(80, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Re:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 97);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(56, 13);
            this.label18.TabIndex = 35;
            this.label18.Text = "lambda_3:";
            // 
            // root1
            // 
            this.root1.AutoSize = true;
            this.root1.Location = new System.Drawing.Point(6, 45);
            this.root1.Name = "root1";
            this.root1.Size = new System.Drawing.Size(56, 13);
            this.root1.TabIndex = 33;
            this.root1.Text = "lambda_1:";
            // 
            // tbox_lambda3_im
            // 
            this.tbox_lambda3_im.Location = new System.Drawing.Point(133, 94);
            this.tbox_lambda3_im.Name = "tbox_lambda3_im";
            this.tbox_lambda3_im.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda3_im.TabIndex = 15;
            this.tbox_lambda3_im.Text = "0";
            this.tbox_lambda3_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbox_lambda3_im.TextChanged += new System.EventHandler(this.Tbox_lambda3_im_TextChanged);
            // 
            // tbox_lambda1_re
            // 
            this.tbox_lambda1_re.Location = new System.Drawing.Point(83, 42);
            this.tbox_lambda1_re.Name = "tbox_lambda1_re";
            this.tbox_lambda1_re.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.tbox_lambda1_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda1_re.TabIndex = 12;
            this.tbox_lambda1_re.Text = "0";
            this.tbox_lambda1_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda3_re
            // 
            this.tbox_lambda3_re.Location = new System.Drawing.Point(83, 94);
            this.tbox_lambda3_re.Name = "tbox_lambda3_re";
            this.tbox_lambda3_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda3_re.TabIndex = 14;
            this.tbox_lambda3_re.Text = "0";
            this.tbox_lambda3_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda1_im
            // 
            this.tbox_lambda1_im.Location = new System.Drawing.Point(133, 42);
            this.tbox_lambda1_im.Name = "tbox_lambda1_im";
            this.tbox_lambda1_im.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda1_im.TabIndex = 13;
            this.tbox_lambda1_im.Text = "0";
            this.tbox_lambda1_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbox_lambda1_im.TextChanged += new System.EventHandler(this.Tbox_lambda1_im_TextChanged);
            // 
            // cBox_non_linear
            // 
            this.cBox_non_linear.AutoSize = true;
            this.cBox_non_linear.Checked = true;
            this.cBox_non_linear.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBox_non_linear.Location = new System.Drawing.Point(147, 435);
            this.cBox_non_linear.Name = "cBox_non_linear";
            this.cBox_non_linear.Size = new System.Drawing.Size(122, 17);
            this.cBox_non_linear.TabIndex = 56;
            this.cBox_non_linear.Text = "Линейная система";
            this.cBox_non_linear.UseVisualStyleBackColor = true;
            // 
            // cBox_Reg_on
            // 
            this.cBox_Reg_on.AutoSize = true;
            this.cBox_Reg_on.Checked = true;
            this.cBox_Reg_on.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBox_Reg_on.Location = new System.Drawing.Point(8, 435);
            this.cBox_Reg_on.Name = "cBox_Reg_on";
            this.cBox_Reg_on.Size = new System.Drawing.Size(129, 17);
            this.cBox_Reg_on.TabIndex = 58;
            this.cBox_Reg_on.Text = "Включить регулятор";
            this.cBox_Reg_on.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 458);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(329, 263);
            this.dataGridView1.TabIndex = 57;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.Button_setInitVal);
            this.groupBox2.Controls.Add(this.labelfi);
            this.groupBox2.Controls.Add(this.labeldfi_dt);
            this.groupBox2.Controls.Add(this.labelx);
            this.groupBox2.Controls.Add(this.labeldx_dt);
            this.groupBox2.Controls.Add(this.textBox_fi);
            this.groupBox2.Controls.Add(this.textBox_dx_dt);
            this.groupBox2.Controls.Add(this.textBox_x);
            this.groupBox2.Controls.Add(this.textBox_dfi_dt);
            this.groupBox2.Location = new System.Drawing.Point(8, 282);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(135, 134);
            this.groupBox2.TabIndex = 49;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Исходное значение";
            // 
            // Button_setInitVal
            // 
            this.Button_setInitVal.Location = new System.Drawing.Point(55, 108);
            this.Button_setInitVal.Margin = new System.Windows.Forms.Padding(2);
            this.Button_setInitVal.Name = "Button_setInitVal";
            this.Button_setInitVal.Size = new System.Drawing.Size(75, 25);
            this.Button_setInitVal.TabIndex = 25;
            this.Button_setInitVal.Text = "Принять";
            this.Button_setInitVal.UseVisualStyleBackColor = true;
            this.Button_setInitVal.Click += new System.EventHandler(this.Button_setInitVal_Click);
            // 
            // labelfi
            // 
            this.labelfi.AutoSize = true;
            this.labelfi.Location = new System.Drawing.Point(4, 21);
            this.labelfi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelfi.Name = "labelfi";
            this.labelfi.Size = new System.Drawing.Size(15, 13);
            this.labelfi.TabIndex = 21;
            this.labelfi.Text = "fi:";
            // 
            // labeldfi_dt
            // 
            this.labeldfi_dt.AutoSize = true;
            this.labeldfi_dt.Location = new System.Drawing.Point(4, 44);
            this.labeldfi_dt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labeldfi_dt.Name = "labeldfi_dt";
            this.labeldfi_dt.Size = new System.Drawing.Size(35, 13);
            this.labeldfi_dt.TabIndex = 22;
            this.labeldfi_dt.Text = "dfi/dt:";
            // 
            // labelx
            // 
            this.labelx.AutoSize = true;
            this.labelx.Location = new System.Drawing.Point(4, 67);
            this.labelx.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelx.Name = "labelx";
            this.labelx.Size = new System.Drawing.Size(33, 13);
            this.labelx.TabIndex = 23;
            this.labelx.Text = "x - x*:";
            // 
            // labeldx_dt
            // 
            this.labeldx_dt.AutoSize = true;
            this.labeldx_dt.Location = new System.Drawing.Point(4, 88);
            this.labeldx_dt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labeldx_dt.Name = "labeldx_dt";
            this.labeldx_dt.Size = new System.Drawing.Size(35, 13);
            this.labeldx_dt.TabIndex = 24;
            this.labeldx_dt.Text = "dx/dt:";
            // 
            // textBox_fi
            // 
            this.textBox_fi.Location = new System.Drawing.Point(55, 17);
            this.textBox_fi.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_fi.Name = "textBox_fi";
            this.textBox_fi.Size = new System.Drawing.Size(76, 20);
            this.textBox_fi.TabIndex = 8;
            this.textBox_fi.Text = "0,523";
            this.textBox_fi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_dx_dt
            // 
            this.textBox_dx_dt.Location = new System.Drawing.Point(55, 85);
            this.textBox_dx_dt.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_dx_dt.Name = "textBox_dx_dt";
            this.textBox_dx_dt.Size = new System.Drawing.Size(76, 20);
            this.textBox_dx_dt.TabIndex = 11;
            this.textBox_dx_dt.Text = "0";
            this.textBox_dx_dt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_x
            // 
            this.textBox_x.Location = new System.Drawing.Point(55, 63);
            this.textBox_x.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_x.Name = "textBox_x";
            this.textBox_x.Size = new System.Drawing.Size(76, 20);
            this.textBox_x.TabIndex = 9;
            this.textBox_x.Text = "0,05";
            this.textBox_x.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_dfi_dt
            // 
            this.textBox_dfi_dt.Location = new System.Drawing.Point(55, 40);
            this.textBox_dfi_dt.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_dfi_dt.Name = "textBox_dfi_dt";
            this.textBox_dfi_dt.Size = new System.Drawing.Size(76, 20);
            this.textBox_dfi_dt.TabIndex = 10;
            this.textBox_dfi_dt.Text = "0";
            this.textBox_dfi_dt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.pBox_Vmax_criterion);
            this.tabPage2.Controls.Add(this.pBox_h2_criterion);
            this.tabPage2.Controls.Add(this.pBox_h1_criterion);
            this.tabPage2.Controls.Add(this.pBox_H_criterion);
            this.tabPage2.Controls.Add(this.pBox_T_criterion);
            this.tabPage2.Controls.Add(this.func_num_text);
            this.tabPage2.Controls.Add(this.ymax_t);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1356, 741);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Линии равного уровня";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.PeachPuff;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(280, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(290, 62);
            this.label5.TabIndex = 24;
            this.label5.Text = "-mu2 < Re(lambda_i) < -mu1;\r\n|Im(lambda_i)| < -sigma * Re(lambda_i),\r\ni = (1, ..." +
    ", 4);";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.label_mu1);
            this.groupBox4.Controls.Add(this.xmin_t);
            this.groupBox4.Controls.Add(this.label_M3);
            this.groupBox4.Controls.Add(this.xmax_t);
            this.groupBox4.Controls.Add(this.label_M2);
            this.groupBox4.Controls.Add(this.label_mu2);
            this.groupBox4.Controls.Add(this.label_M1);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.ymin_t);
            this.groupBox4.Controls.Add(this.label_N);
            this.groupBox4.Controls.Add(this.label_sigma);
            this.groupBox4.Controls.Add(this.DL_N);
            this.groupBox4.Controls.Add(this.DL_M2);
            this.groupBox4.Controls.Add(this.DL_M1);
            this.groupBox4.Controls.Add(this.DL_M3);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(268, 153);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Параметры отрисовки:";
            // 
            // label_mu1
            // 
            this.label_mu1.AutoSize = true;
            this.label_mu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_mu1.Location = new System.Drawing.Point(6, 16);
            this.label_mu1.Name = "label_mu1";
            this.label_mu1.Size = new System.Drawing.Size(44, 20);
            this.label_mu1.TabIndex = 16;
            this.label_mu1.Text = "mu1:";
            // 
            // xmin_t
            // 
            this.xmin_t.Location = new System.Drawing.Point(67, 18);
            this.xmin_t.Name = "xmin_t";
            this.xmin_t.Size = new System.Drawing.Size(75, 20);
            this.xmin_t.TabIndex = 8;
            this.xmin_t.Text = "6";
            this.xmin_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_M3
            // 
            this.label_M3.AutoSize = true;
            this.label_M3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_M3.Location = new System.Drawing.Point(148, 94);
            this.label_M3.Name = "label_M3";
            this.label_M3.Size = new System.Drawing.Size(35, 20);
            this.label_M3.TabIndex = 22;
            this.label_M3.Text = "M3:";
            // 
            // xmax_t
            // 
            this.xmax_t.Location = new System.Drawing.Point(67, 44);
            this.xmax_t.Name = "xmax_t";
            this.xmax_t.Size = new System.Drawing.Size(75, 20);
            this.xmax_t.TabIndex = 6;
            this.xmax_t.Text = "0,1";
            this.xmax_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_M2
            // 
            this.label_M2.AutoSize = true;
            this.label_M2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_M2.Location = new System.Drawing.Point(148, 68);
            this.label_M2.Name = "label_M2";
            this.label_M2.Size = new System.Drawing.Size(35, 20);
            this.label_M2.TabIndex = 21;
            this.label_M2.Text = "M2:";
            // 
            // label_mu2
            // 
            this.label_mu2.AutoSize = true;
            this.label_mu2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_mu2.Location = new System.Drawing.Point(6, 42);
            this.label_mu2.Name = "label_mu2";
            this.label_mu2.Size = new System.Drawing.Size(44, 20);
            this.label_mu2.TabIndex = 17;
            this.label_mu2.Text = "mu2:";
            // 
            // label_M1
            // 
            this.label_M1.AutoSize = true;
            this.label_M1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_M1.Location = new System.Drawing.Point(148, 42);
            this.label_M1.Name = "label_M1";
            this.label_M1.Size = new System.Drawing.Size(35, 20);
            this.label_M1.TabIndex = 20;
            this.label_M1.Text = "M1:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(2, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(262, 25);
            this.button1.TabIndex = 10;
            this.button1.Text = "Рассчитать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ymin_t
            // 
            this.ymin_t.Location = new System.Drawing.Point(67, 70);
            this.ymin_t.Name = "ymin_t";
            this.ymin_t.Size = new System.Drawing.Size(75, 20);
            this.ymin_t.TabIndex = 7;
            this.ymin_t.Text = "1";
            this.ymin_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_N
            // 
            this.label_N.AutoSize = true;
            this.label_N.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_N.Location = new System.Drawing.Point(159, 16);
            this.label_N.Name = "label_N";
            this.label_N.Size = new System.Drawing.Size(24, 20);
            this.label_N.TabIndex = 19;
            this.label_N.Text = "N:";
            // 
            // label_sigma
            // 
            this.label_sigma.AutoSize = true;
            this.label_sigma.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_sigma.Location = new System.Drawing.Point(6, 68);
            this.label_sigma.Name = "label_sigma";
            this.label_sigma.Size = new System.Drawing.Size(55, 20);
            this.label_sigma.TabIndex = 18;
            this.label_sigma.Text = "sigma:";
            // 
            // DL_N
            // 
            this.DL_N.Location = new System.Drawing.Point(189, 18);
            this.DL_N.Name = "DL_N";
            this.DL_N.Size = new System.Drawing.Size(75, 20);
            this.DL_N.TabIndex = 3;
            this.DL_N.Text = "50";
            this.DL_N.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DL_M2
            // 
            this.DL_M2.Location = new System.Drawing.Point(189, 70);
            this.DL_M2.Name = "DL_M2";
            this.DL_M2.Size = new System.Drawing.Size(75, 20);
            this.DL_M2.TabIndex = 1;
            this.DL_M2.Text = "5";
            this.DL_M2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DL_M1
            // 
            this.DL_M1.Location = new System.Drawing.Point(189, 44);
            this.DL_M1.Name = "DL_M1";
            this.DL_M1.Size = new System.Drawing.Size(75, 20);
            this.DL_M1.TabIndex = 2;
            this.DL_M1.Text = "10";
            this.DL_M1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DL_M3
            // 
            this.DL_M3.Location = new System.Drawing.Point(189, 96);
            this.DL_M3.Name = "DL_M3";
            this.DL_M3.Size = new System.Drawing.Size(75, 20);
            this.DL_M3.TabIndex = 4;
            this.DL_M3.Text = "3";
            this.DL_M3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pBox_Vmax_criterion
            // 
            this.pBox_Vmax_criterion.BackColor = System.Drawing.SystemColors.Control;
            this.pBox_Vmax_criterion.Location = new System.Drawing.Point(1003, 362);
            this.pBox_Vmax_criterion.Name = "pBox_Vmax_criterion";
            this.pBox_Vmax_criterion.Size = new System.Drawing.Size(350, 350);
            this.pBox_Vmax_criterion.TabIndex = 15;
            this.pBox_Vmax_criterion.TabStop = false;
            // 
            // pBox_h2_criterion
            // 
            this.pBox_h2_criterion.BackColor = System.Drawing.SystemColors.Control;
            this.pBox_h2_criterion.Location = new System.Drawing.Point(647, 362);
            this.pBox_h2_criterion.Name = "pBox_h2_criterion";
            this.pBox_h2_criterion.Size = new System.Drawing.Size(350, 350);
            this.pBox_h2_criterion.TabIndex = 14;
            this.pBox_h2_criterion.TabStop = false;
            // 
            // pBox_h1_criterion
            // 
            this.pBox_h1_criterion.BackColor = System.Drawing.SystemColors.Control;
            this.pBox_h1_criterion.Location = new System.Drawing.Point(291, 362);
            this.pBox_h1_criterion.Name = "pBox_h1_criterion";
            this.pBox_h1_criterion.Size = new System.Drawing.Size(350, 350);
            this.pBox_h1_criterion.TabIndex = 13;
            this.pBox_h1_criterion.TabStop = false;
            // 
            // pBox_H_criterion
            // 
            this.pBox_H_criterion.BackColor = System.Drawing.SystemColors.Control;
            this.pBox_H_criterion.Location = new System.Drawing.Point(1003, 6);
            this.pBox_H_criterion.Name = "pBox_H_criterion";
            this.pBox_H_criterion.Size = new System.Drawing.Size(350, 350);
            this.pBox_H_criterion.TabIndex = 12;
            this.pBox_H_criterion.TabStop = false;
            // 
            // pBox_T_criterion
            // 
            this.pBox_T_criterion.BackColor = System.Drawing.SystemColors.Control;
            this.pBox_T_criterion.Location = new System.Drawing.Point(647, 6);
            this.pBox_T_criterion.Name = "pBox_T_criterion";
            this.pBox_T_criterion.Size = new System.Drawing.Size(350, 350);
            this.pBox_T_criterion.TabIndex = 0;
            this.pBox_T_criterion.TabStop = false;
            this.pBox_T_criterion.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Paint);
            // 
            // func_num_text
            // 
            this.func_num_text.Location = new System.Drawing.Point(392, 205);
            this.func_num_text.Name = "func_num_text";
            this.func_num_text.Size = new System.Drawing.Size(100, 20);
            this.func_num_text.TabIndex = 9;
            this.func_num_text.Text = "2";
            this.func_num_text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ymax_t
            // 
            this.ymax_t.Location = new System.Drawing.Point(498, 205);
            this.ymax_t.Name = "ymax_t";
            this.ymax_t.Size = new System.Drawing.Size(75, 20);
            this.ymax_t.TabIndex = 5;
            this.ymax_t.Text = "10";
            this.ymax_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Timer1
            // 
            this.Timer1.Interval = 10;
            // 
            // PortalCraneModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1364, 755);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PortalCraneModel";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "PortalCraneModel";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.roots_gbox.ResumeLayout(false);
            this.roots_gbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_Vmax_criterion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_h2_criterion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_h1_criterion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_H_criterion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_T_criterion)).EndInit();
            this.ResumeLayout(false);

        }

        private void Btn_clear_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            chart1.Legends.Clear();
            chart2.Series.Clear();
            chart2.Legends.Clear();
            chart3.Series.Clear();
            chart3.Legends.Clear();
            chart4.Series.Clear();
            chart4.Legends.Clear();

            dataGridView1.Rows.Clear();

            buildCount = 0;
        }

        private void Tbox_lambda1_im_TextChanged(object sender, EventArgs e)
        {
            if (tbox_lambda1_im.Text != "" && tbox_lambda1_im.Text != "0" && tbox_lambda1_im.Text != "-")
            {
                tbox_lambda2_re.Text = tbox_lambda1_re.Text;
                tbox_lambda2_re.ReadOnly = true;

                tbox_lambda2_im.Text = (double.Parse(tbox_lambda1_im.Text) * -1).ToString();
                tbox_lambda2_im.ReadOnly = true;
            }
            else if (tbox_lambda1_im.Text == "0")
            {
                tbox_lambda2_im.Text = "0";
                tbox_lambda2_im.ReadOnly = true;
            }
        }

        private void Tbox_lambda3_im_TextChanged(object sender, EventArgs e)
        {
            if (tbox_lambda3_im.Text != "" && tbox_lambda3_im.Text != "0" && tbox_lambda3_im.Text != "-")
            {
                tbox_lambda4_re.Text = tbox_lambda3_re.Text;
                tbox_lambda4_re.ReadOnly = true;

                tbox_lambda4_im.Text = (double.Parse(tbox_lambda3_im.Text) * -1).ToString();
                tbox_lambda4_im.ReadOnly = true;
            }
            else if (tbox_lambda3_im.Text == "0")
            {
                tbox_lambda4_im.Text = "0";
                tbox_lambda4_im.ReadOnly = true;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            tbox_lambda1_re.Text = "0";
            tbox_lambda1_im.Text = "0";

            tbox_lambda2_re.Text = "0";
            tbox_lambda2_re.ReadOnly = false;
            tbox_lambda2_im.Text = "0";
            tbox_lambda2_im.ReadOnly = false;

            tbox_lambda3_re.Text = "0";
            tbox_lambda3_im.Text = "0";

            tbox_lambda4_re.Text = "0";
            tbox_lambda4_re.ReadOnly = false;
            tbox_lambda4_im.Text = "0";
            tbox_lambda4_im.ReadOnly = false;
        }

        //===============================================================================================================================================================

        static public double[] pt;
        static double XMin, XMax, YMin, YMax;

        struct Node
        {
            public double x;
            public double y;
            public double Q;
        }

        //private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        //{
        //    tabPage2.VerticalScroll.Value = vScrollBar1.Value;
        //}


        class eque_lines
        {
            Node[] pDat;
            double[] pQ;
            int N; // число разбиений сетки
            int M; // общее число уровней
            int M1; // число основных узлов
            int M2; // число подуузлов
            int M3; // число "подподузлов"

            double m1;
            double m2;
            double c1;
            double c2;
            double[] w1 = new double[2];
            double[] w2 = new double[2];

            public eque_lines()
            {
                pDat = null;
                pQ = null;
            }

            public void CreateDat(int _N, int _M1, int _M2, int _M3)
            {
                N = _N;
                M1 = _M1;
                M2 = _M2;
                M3 = _M3;
                M = M1 + M2 + M3 - 1;

                //	DelDat();
                if ((pDat = new Node[(N + 1) * (N + 1)]) == null)
                    return;
                else
                    if ((pQ = new double[M + 1]) == null)
                    return;
            }
            public void CreateDat1(double _m1, double _m2, double _c1, double _c2, double[] _w1, double[] _w2)
            {
                m1 = _m1;
                m2 = _m2;
                c1 = _c1;
                c2 = _c2;
                w1 = _w1;
                w2 = _w2;
            }
            public void SetDat(double _a0, double _b0, double _a1, double _b1, bool UseGKLS, int F_Num)
            {
                CFunction F;
                F = new CFunction();
                if (pDat == null || pQ == null)
                    return;
                else
                {
                    double Qmin, Qmax, QQ;
                    int i, j; // номер узла
                    double hx = (_b0 - _a0) / N; // вычисление шага по x
                    double hy = (_b1 - _a1) / N; // вычисление шага по y
                    double[] x_p = new double[2];
                    // обход сетки
                    pt = new double[2];
                    Qmin = 1.7976931348623158e+308;
                    Qmax = 2.2250738585072014e-308;
                    if (F_Num == 7)
                        F.set_func(m1, m2, c1, c2, w1, w2);
                    F.set(F_Num);
                    for (i = 0; i <= N; i++)
                        for (j = 0; j <= N; j++)
                        {
                            // заполнение структуры сетки
                            // координаты узла сетки
                            x_p[0] = pDat[(N + 1) * i + j].x = _a0 + hx * i;
                            x_p[1] = pDat[(N + 1) * i + j].y = _a1 + hy * j;
                            // значение функции в узле
                            pt[0] = x_p[0];
                            pt[1] = x_p[1];
                            QQ = pDat[(N + 1) * i + j].Q = F.GetValue(pt);

                            //		printf("QQ = %f\n",QQ);
                            // поиск минимального и максимального значения на сетке
                            if ((i == 0) && (j == 0) || (Qmin > QQ))
                                Qmin = QQ;
                            if ((i == 0) && (j == 0) || (Qmax < QQ))
                                Qmax = QQ;
                        }
                    double hQ1 = (Qmax - Qmin) / M1; // шаг функции по уровням
                    int ku = 0; // позиция в сетке уровней   
                    for (i = 0; i < M1; i++) // вычисление значений функции на основных уровнях 
                        pQ[ku++] = Qmax - hQ1 * i;

                    double hQ2 = hQ1 / (M2 + 1); // шаг функции по подуровням
                    for (i = 1; i <= M2; i++) // вычисление значений функции на подуровнях
                        pQ[ku++] = pQ[M1 - 1] - hQ2 * i;

                    for (i = 1; i <= M3; i++) // вычисление значений функции на "под-подуровнях"
                        pQ[ku++] = pQ[M1 + M2 - 1] - (hQ2 / (M3 + 1)) * i;
                }
            }

            public void SendLines(Graphics g, PictureBox pic)
            {
                int i, j, u, s;
                for (i = 0; i < N; i++)
                    for (j = 0; j < N; j++)
                    {
                        for (u = 0; u <= M; u++)
                        {
                            double Qu = pQ[u];// Уровень
                            double[] x = new double[5];
                            double[] y = new double[5];//Соединяемые точки
                            int kt = 0;// Количество соединяемых точек
                            double x0, x1, y0, y1, Q0, Q1;

                            //Нижняя сторона
                            x0 = pDat[(N + 1) * i + j].x;
                            x1 = pDat[(N + 1) * (i + 1) + j].x;
                            y0 = pDat[(N + 1) * i + j].y;
                            Q0 = pDat[(N + 1) * i + j].Q;
                            Q1 = pDat[(N + 1) * (i + 1) + j].Q;
                            if ((Q0 - Qu) * (Qu - Q1) >= 0 && (Q1 != Q0))
                            {
                                y[kt] = y0;
                                x[kt++] = x0 + (x1 - x0) * (Qu - Q0) / (Q1 - Q0);
                            }

                            //Левая сторона
                            x0 = pDat[(N + 1) * i + j].x;
                            y0 = pDat[(N + 1) * i + j].y;
                            y1 = pDat[(N + 1) * i + j + 1].y;
                            Q0 = pDat[(N + 1) * i + j].Q;
                            Q1 = pDat[(N + 1) * i + j + 1].Q;
                            if ((Q0 - Qu) * (Qu - Q1) >= 0 && (Q1 != Q0))
                            {
                                x[kt] = x0;
                                y[kt++] = y0 + (y1 - y0) * (Qu - Q0) / (Q1 - Q0);
                            }

                            //Верхняя сторона
                            x0 = pDat[(N + 1) * i + j + 1].x;
                            x1 = pDat[(N + 1) * (i + 1) + j + 1].x;
                            y0 = pDat[(N + 1) * i + j + 1].y;
                            Q0 = pDat[(N + 1) * i + j + 1].Q;
                            Q1 = pDat[(N + 1) * (i + 1) + j + 1].Q;
                            if ((Q0 - Qu) * (Qu - Q1) >= 0 && (Q1 != Q0))
                            {
                                y[kt] = y0;
                                x[kt++] = x0 + (x1 - x0) * (Qu - Q0) / (Q1 - Q0);
                            }

                            //Правая сторона
                            x0 = pDat[(N + 1) * (i + 1) + j].x;
                            y0 = pDat[(N + 1) * (i + 1) + j].y;
                            y1 = pDat[(N + 1) * (i + 1) + j + 1].y;
                            Q0 = pDat[(N + 1) * (i + 1) + j].Q;
                            Q1 = pDat[(N + 1) * (i + 1) + j + 1].Q;
                            if ((Q0 - Qu) * (Qu - Q1) >= 0 && (Q1 != Q0))
                            {
                                x[kt] = x0;
                                y[kt++] = y0 + (y1 - y0) * (Qu - Q0) / (Q1 - Q0);
                            }

                            if (kt > 0) //Прорисовка линии
                            {
                                if (u < M1)
                                {
                                    for (s = 0; s < kt - 1; s++)
                                    {
                                        Pen p = new Pen(Color.DimGray, 2);
                                        g.DrawLine(p, (float)((x[s] - XMin) / (XMax - XMin) * (pic.Width - 1)), (float)((YMax - y[s]) / (YMax - YMin) * (pic.Height - 1)), (float)((x[s + 1] - XMin) / (XMax - XMin) * (pic.Width - 1)), (float)((YMax - y[s + 1]) / (YMax - YMin) * (pic.Height - 1)));
                                        //	printf("Drawed Line\n");

                                    }
                                }
                                else if (u < M1 + M2)
                                {
                                    for (s = 0; s < kt - 1; s++)
                                    {
                                        Pen p = new Pen(Color.DimGray, 2);
                                        g.DrawLine(p, (float)((x[s] - XMin) / (XMax - XMin) * (pic.Width - 1)), (float)((YMax - y[s]) / (YMax - YMin) * (pic.Height - 1)), (float)((x[s + 1] - XMin) / (XMax - XMin) * (pic.Width - 1)), (float)((YMax - y[s + 1]) / (YMax - YMin) * (pic.Height - 1)));
                                        //	printf("Drawed Line\n");
                                    }
                                }
                                else
                                {
                                    for (s = 0; s < kt - 1; s++)
                                    {
                                        Pen p = new Pen(Color.DimGray, 2);
                                        g.DrawLine(p, (float)((x[s] - XMin) / (XMax - XMin) * (pic.Width - 1)), (float)((YMax - y[s]) / (YMax - YMin) * (pic.Height - 1)), (float)((x[s + 1] - XMin) / (XMax - XMin) * (pic.Width - 1)), (float)((YMax - y[s + 1]) / (YMax - YMin) * (pic.Height - 1)));
                                        //	printf("Drawed Line\n");
                                    }
                                }
                            }
                            //Конец прорисовки линии уровня Qu
                        }//Конец перебора всех Qu
                    }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            tbox_lambda1_re.Text = (-1).ToString();
            tbox_lambda1_im.Text = (-1).ToString();

            tbox_lambda3_re.Text = (-1).ToString();
            tbox_lambda3_im.Text = (-1).ToString();
        }

        eque_lines Draw_Line = new eque_lines();
        private void button1_Click(object sender, EventArgs e)
        {
            int _N = System.Convert.ToInt32(DL_N.Text);
            int _M1 = System.Convert.ToInt32(DL_M1.Text);
            int _M2 = System.Convert.ToInt32(DL_M2.Text);
            int _M3 = System.Convert.ToInt32(DL_M3.Text);
            Draw_Line.CreateDat(_N, _M1, _M2, _M3);
            XMin = -1 * System.Convert.ToDouble(xmin_t.Text);
            XMax = -1 * System.Convert.ToDouble(xmax_t.Text);
            YMin = System.Convert.ToDouble(ymin_t.Text);
            YMax = System.Convert.ToDouble(ymax_t.Text);

            // определяем количество точек, которые будут отрисованы
            criteria.drawCount = _N * _N;

            // создаем управляемое хранилище
            this.DrawCriteria = new double[criteria.drawCount * 5];

            // определяем размер управляемой структуры
            int sizeStruct = Marshal.SizeOf(typeof(PortalCraneModel.TAllDrawPoints));

            // выделяем память под неуправляемую структуру
            this.ptrCriteria = Marshal.AllocHGlobal(sizeStruct);

            // копируем данные из неуправляемой в управляемую
            Marshal.StructureToPtr(this.criteria, this.ptrCriteria, false);

            // выделяем память под внутренний неуправляемый массив в неупр структуре
            PortalCraneModel.InitAllPointsArray(this.ptrCriteria); 

            Draw_Line.SetDat(XMin, XMax, YMin, YMax, false, System.Convert.ToInt32(func_num_text.Text));

            pBox_T_criterion.Invalidate();
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            Draw_Line.SendLines(e.Graphics, pBox_T_criterion);
        }
    }
}


