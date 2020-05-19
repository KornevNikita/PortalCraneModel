// Decompiled with JetBrains decompiler
// Type: PortalCraneModel.PortalCraneModel
// Assembly: PortalCraneModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BAA13FC9-345E-43AF-A47A-80EBEB1AFDE9
// Assembly location: C:\Users\Nikita\Desktop\PortalCraneModel\x64\Release\PortalCraneModel.exe

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
        Random rand = new Random();
        public int buildCount = 0;
        const string dll = "PortalCraneCalc.dll";
        public double[] DrawPoints;
        public IntPtr ptrTAllDrawPoints;
        public PortalCraneModel.TAllDrawPoints allPoints;
        public PortalCraneModel.TStatePoint CurrentPoint;
        public IntPtr PtrNextPoint;
        public int dinPointsCount;
        public double[] roots; // zhelaemie korni
        public double M;
        public double m;
        public double l;
        public double R;
        public double g;
        public double h_fi;
        public double h_x;
        public double B;
        public double gamma;
        public double E;
        public double fi;
        public double dfi_dt;
        public double x;
        public double dx_dt;
        public double xMax;
        public double yMax;
        public double dt;
        public double t_start;
        public double t_stop;

        public double lambda1_re;
        public double lambda1_im;
        public double lambda2_re;
        public double lambda2_im;
        public double lambda3_re;
        public double lambda3_im;
        public double lambda4_re;
        public double lambda4_im;


        public int drawStCount;
        public bool inDinamic;
        private IContainer components;
        private GroupBox groupBox1;
        private TextBox textBox_h_fi;
        private TextBox textBox_B;
        private TextBox textBox_l;
        private TextBox textBox_h_x;
        private TextBox textBox_mmal;
        private TextBox textBox_M;
        private TextBox textBox_R;
        private TextBox textBox_g;
        private Label label_B;
        private Label label7;
        private Label label_h_fi;
        private Label label_g;
        private Label label_R;
        private Label label_l;
        private Label label_mmal;
        private Label label_M;
        private TextBox textBox_fi;
        private TextBox textBox_x;
        private TextBox textBox_dfi_dt;
        private TextBox textBox_dx_dt;
        private GroupBox groupBox2;
        private Label labelfi;
        private Label labeldfi_dt;
        private Label labelx;
        private Label labeldx_dt;
        private Label label_h_x;
        private TextBox textBox_E;
        private TextBox textBox_gamma;
        private Label label_gamma;
        private Label label_E;
        private Button Button_setParam;
        private Button Button_setInitVal;
        private GroupBox groupBox3;
        private Button Button_setCalcParam;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox textBox_drawStCount;
        private TextBox textBox_t_stop;
        private TextBox textBox_t_start;
        private TextBox textBox_step;
        private Button Button_runCalc;
        private Label label16;
        private GroupBox roots_gbox;
        private Label label18;
        private Label root1;
        private TextBox tbox_lambda3_im;
        private TextBox tbox_lambda1_re;
        private TextBox tbox_lambda3_re;
        private TextBox tbox_lambda1_im;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Chart chart2;
        private Chart chart3;
        private Chart chart4;
        private Button btn_clear;
        private Label label10;
        private TextBox tbox_lambda4_im;
        private TextBox tbox_lambda4_re;
        private TextBox tbox_lambda2_im;
        private TextBox tbox_lambda2_re;
        private Label label9;
        private Label label8;
        private Label label11;
        private GroupBox groupBox4;
        private Button button1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label5;
        private Label label6;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label17;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
        private TextBox textBox6;
        private TextBox textBox7;
        private TextBox textBox8;
        private TextBox textBox9;
        private TextBox textBox10;
        private Button button2;
        private CheckBox checkBox_dinDraw;
        private Timer Timer1;
        private CheckBox cBox_non_linear;
        private Label textBox_run_time;

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetModelParams(double _M, double _m, double _l, double _R, double _g,
            double _h_fi, double _h_x, double _B, double _gamma, double _E,
            double _p1_re, double _p1_im, double _p2_re, double _p2_im,
            double _p3_re, double _p3_im, double _p4_re, double _p4_im);

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
        public static extern void GetAllDrawPoints(IntPtr ptrAllDrawPoints, bool system);

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
            // считываем:
            this.SetParam(); // параметры модели
            this.SetCalcParam(); // параметры расчета
            this.SetInitVal(); // начальное состояние системы

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
                PortalCraneModel.GetAllDrawPoints(this.ptrTAllDrawPoints, cBox_non_linear.Checked);
                this.allPoints = (PortalCraneModel.TAllDrawPoints)Marshal.PtrToStructure(this.ptrTAllDrawPoints, typeof(PortalCraneModel.TAllDrawPoints));
                Marshal.Copy(this.allPoints.allDrawPoints, this.DrawPoints, 0, this.allPoints.drawCount * 5);
                PortalCraneModel.DeleteAllPointsArray(this.ptrTAllDrawPoints);
                Marshal.FreeHGlobal(this.ptrTAllDrawPoints);
                //this.PictureBox1.Refresh();
                //this.PictureBox2.Refresh();
                //this.PictureBox3.Refresh();
                //this.PictureBox4.Refresh();

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


                stopwatch.Stop();
                this.textBox_run_time.Text = stopwatch.Elapsed.TotalSeconds.ToString();
                this.textBox_run_time.BackColor = Color.Yellow;
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
            lambda1_re = double.Parse(tbox_lambda1_re.Text);
            lambda1_im = double.Parse(tbox_lambda1_im.Text);
            lambda2_re = double.Parse(tbox_lambda2_re.Text);
            lambda2_im = double.Parse(tbox_lambda2_im.Text);
            lambda3_re = double.Parse(tbox_lambda3_re.Text);
            lambda3_im = double.Parse(tbox_lambda3_im.Text);
            lambda4_re = double.Parse(tbox_lambda4_re.Text);
            lambda4_im = double.Parse(tbox_lambda4_im.Text);
            PortalCraneModel.SetModelParams(this.M, this.m, this.l, this.R, this.g, this.h_fi, this.h_x, this.B, this.gamma, this.E,
                lambda1_re, lambda1_im, lambda2_re, lambda2_im,
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

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clipRectangle1 = e.ClipRectangle;
            int right = clipRectangle1.Right;
            clipRectangle1 = e.ClipRectangle;
            int left = clipRectangle1.Left;
            double num1 = 0.5 * (double)(right + left);
            Rectangle clipRectangle2 = e.ClipRectangle;
            int bottom = clipRectangle2.Bottom;
            clipRectangle2 = e.ClipRectangle;
            int top = clipRectangle2.Top;
            double num2 = 0.5 * (double)(bottom + top);
            double height = (double)e.ClipRectangle.Height;
            double width = (double)e.ClipRectangle.Width;
            Pen pen = new Pen(Color.Black, 2f);
            int num3 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
            int red = 0;
            int green = 0;
            int blue = 0;
            for (int index = 1; index < num3; ++index)
            {
                switch (index * 5 / (int)byte.MaxValue % 3)
                {
                    case 0:
                        green = 0;
                        blue = 0;
                        red = index * 5 % (int)byte.MaxValue;
                        break;
                    case 1:
                        red = 0;
                        blue = 0;
                        green = index * 5 % (int)byte.MaxValue;
                        break;
                    case 2:
                        red = 0;
                        green = 0;
                        blue = index * 5 % (int)byte.MaxValue;
                        break;
                }
                pen.Color = Color.FromArgb(red, green, blue);
                e.Graphics.DrawLine(pen, (float)(num1 + this.DrawPoints[(index - 1) * 5] / this.xMax * width / 2.0), (float)(num2 - this.DrawPoints[(index - 1) * 5 + 1] / this.yMax * height / 2.0), (float)(num1 + this.DrawPoints[index * 5] / this.xMax * width / 2.0), (float)(num2 - this.DrawPoints[index * 5 + 1] / this.yMax * height / 2.0));
            }
        }

        private void PictureBox2_Paint(object sender, PaintEventArgs e)
        {
            double left = (double)e.ClipRectangle.Left;
            Rectangle clipRectangle = e.ClipRectangle;
            int bottom = clipRectangle.Bottom;
            clipRectangle = e.ClipRectangle;
            int top = clipRectangle.Top;
            double num1 = 0.5 * (double)(bottom + top);
            double height = (double)e.ClipRectangle.Height;
            double width = (double)e.ClipRectangle.Width;
            Pen pen = new Pen(Color.Black, 2f);
            int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
            int red = 0;
            int green = 0;
            int blue = 0;
            for (int index = 1; index < num2; ++index)
            {
                switch (index * 5 / (int)byte.MaxValue % 5)
                {
                    case 0:
                        green = 0;
                        blue = 0;
                        red = index * 5 % (int)byte.MaxValue;
                        break;
                    case 1:
                        red = 0;
                        blue = 0;
                        green = index * 5 % (int)byte.MaxValue;
                        break;
                    case 2:
                        red = 0;
                        green = 0;
                        blue = index * 5 % (int)byte.MaxValue;
                        break;
                }
                pen.Color = Color.FromArgb(red, green, blue);
                e.Graphics.DrawLine(pen, (float)(left + this.DrawPoints[(index - 1) * 5 + 2] / this.xMax * width / 15.0), (float)(num1 - this.DrawPoints[(index - 1) * 5 + 3] / this.yMax * height / 2.0), (float)(left + this.DrawPoints[index * 5 + 2] / this.xMax * width / 15.0), (float)(num1 - this.DrawPoints[index * 5 + 3] / this.yMax * height / 2.0));
            }
        }

        private void PictureBox3_Paint(object sender, PaintEventArgs e)
        {
            double left = (double)e.ClipRectangle.Left;
            Rectangle clipRectangle = e.ClipRectangle;
            int bottom = clipRectangle.Bottom;
            clipRectangle = e.ClipRectangle;
            int top = clipRectangle.Top;
            double num1 = 0.5 * (double)(bottom + top);
            double height = (double)e.ClipRectangle.Height;
            double width = (double)e.ClipRectangle.Width;
            Pen pen = new Pen(Color.Black, 2f);
            int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
            int red = 0;
            int green = 0;
            int blue = 0;
            for (int index = 1; index < num2; ++index)
            {
                switch (index * 5 / (int)byte.MaxValue % 5)
                {
                    case 0:
                        green = 0;
                        blue = 0;
                        red = index * 5 % (int)byte.MaxValue;
                        break;
                    case 1:
                        red = 0;
                        blue = 0;
                        green = index * 5 % (int)byte.MaxValue;
                        break;
                    case 2:
                        red = 0;
                        green = 0;
                        blue = index * 5 % (int)byte.MaxValue;
                        break;
                }
                pen.Color = Color.FromArgb(red, green, blue);
                e.Graphics.DrawLine(pen, (float)(left + this.DrawPoints[(index - 1) * 5 + 4] / this.xMax * width / 70.0), (float)(num1 - this.DrawPoints[(index - 1) * 5] / this.yMax * height), (float)(left + this.DrawPoints[index * 5 + 4] / this.xMax * width / 70.0), (float)(num1 - this.DrawPoints[index * 5] / this.yMax * height));
            }
        }

        private void PictureBox4_Paint(object sender, PaintEventArgs e)
        {
            double left = (double)e.ClipRectangle.Left;
            Rectangle clipRectangle = e.ClipRectangle;
            int bottom = clipRectangle.Bottom;
            clipRectangle = e.ClipRectangle;
            int top = clipRectangle.Top;
            double num1 = 0.5 * (double)(bottom + top);
            double height = (double)e.ClipRectangle.Height;
            double width = (double)e.ClipRectangle.Width;
            Pen pen = new Pen(Color.Black, 2f);
            int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
            int red = 0;
            int green = 0;
            int blue = 0;
            for (int index = 1; index < num2; ++index)
            {
                switch (index * 5 / (int)byte.MaxValue % 5)
                {
                    case 0:
                        green = 0;
                        blue = 0;
                        red = index * 5 % (int)byte.MaxValue;
                        break;
                    case 1:
                        red = 0;
                        blue = 0;
                        green = index * 5 % (int)byte.MaxValue;
                        break;
                    case 2:
                        red = 0;
                        green = 0;
                        blue = index * 5 % (int)byte.MaxValue;
                        break;
                }
                pen.Color = Color.FromArgb(red, green, blue);
                e.Graphics.DrawLine(pen, (float)(left + this.DrawPoints[(index - 1) * 5 + 4] / this.xMax * width / 50.0), (float)(num1 - this.DrawPoints[(index - 1) * 5 + 2] / this.yMax * height / 50.0), (float)(left + this.DrawPoints[index * 5 + 4] / this.xMax * width / 50.0), (float)(num1 - this.DrawPoints[index * 5 + 2] / this.yMax * height / 50.0));
            }
        }

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
            this.textBox_fi = new System.Windows.Forms.TextBox();
            this.textBox_x = new System.Windows.Forms.TextBox();
            this.textBox_dfi_dt = new System.Windows.Forms.TextBox();
            this.textBox_dx_dt = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Button_setInitVal = new System.Windows.Forms.Button();
            this.labelfi = new System.Windows.Forms.Label();
            this.labeldfi_dt = new System.Windows.Forms.Label();
            this.labelx = new System.Windows.Forms.Label();
            this.labeldx_dt = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_run_time = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.Button_setCalcParam = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_drawStCount = new System.Windows.Forms.TextBox();
            this.textBox_t_stop = new System.Windows.Forms.TextBox();
            this.textBox_t_start = new System.Windows.Forms.TextBox();
            this.textBox_step = new System.Windows.Forms.TextBox();
            this.Button_runCalc = new System.Windows.Forms.Button();
            this.roots_gbox = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbox_lambda4_im = new System.Windows.Forms.TextBox();
            this.tbox_lambda4_re = new System.Windows.Forms.TextBox();
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
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox_dinDraw = new System.Windows.Forms.CheckBox();
            this.cBox_non_linear = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.roots_gbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(9, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(135, 270);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры модели";
            this.groupBox1.UseWaitCursor = true;
            // 
            // Button_setParam
            // 
            this.Button_setParam.Location = new System.Drawing.Point(55, 245);
            this.Button_setParam.Margin = new System.Windows.Forms.Padding(2);
            this.Button_setParam.Name = "Button_setParam";
            this.Button_setParam.Size = new System.Drawing.Size(75, 19);
            this.Button_setParam.TabIndex = 27;
            this.Button_setParam.Text = "Принять";
            this.Button_setParam.UseVisualStyleBackColor = true;
            this.Button_setParam.UseWaitCursor = true;
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
            this.textBox_E.UseWaitCursor = true;
            // 
            // textBox_gamma
            // 
            this.textBox_gamma.Location = new System.Drawing.Point(55, 199);
            this.textBox_gamma.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_gamma.Name = "textBox_gamma";
            this.textBox_gamma.Size = new System.Drawing.Size(76, 20);
            this.textBox_gamma.TabIndex = 25;
            this.textBox_gamma.Text = "4,481";
            this.textBox_gamma.UseWaitCursor = true;
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
            this.label_gamma.UseWaitCursor = true;
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
            this.label_E.UseWaitCursor = true;
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
            this.label_h_x.UseWaitCursor = true;
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
            this.label_B.UseWaitCursor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 158);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 13);
            this.label7.TabIndex = 19;
            this.label7.UseWaitCursor = true;
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
            this.label_h_fi.UseWaitCursor = true;
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
            this.label_g.UseWaitCursor = true;
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
            this.label_R.UseWaitCursor = true;
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
            this.label_l.UseWaitCursor = true;
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
            this.label_mmal.UseWaitCursor = true;
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
            this.label_M.UseWaitCursor = true;
            // 
            // textBox_h_fi
            // 
            this.textBox_h_fi.Location = new System.Drawing.Point(55, 131);
            this.textBox_h_fi.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_h_fi.Name = "textBox_h_fi";
            this.textBox_h_fi.Size = new System.Drawing.Size(76, 20);
            this.textBox_h_fi.TabIndex = 7;
            this.textBox_h_fi.Text = "0,0024";
            this.textBox_h_fi.UseWaitCursor = true;
            // 
            // textBox_g
            // 
            this.textBox_g.Location = new System.Drawing.Point(55, 108);
            this.textBox_g.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_g.Name = "textBox_g";
            this.textBox_g.Size = new System.Drawing.Size(76, 20);
            this.textBox_g.TabIndex = 3;
            this.textBox_g.Text = "9,81";
            this.textBox_g.UseWaitCursor = true;
            // 
            // textBox_B
            // 
            this.textBox_B.Location = new System.Drawing.Point(55, 176);
            this.textBox_B.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_B.Name = "textBox_B";
            this.textBox_B.Size = new System.Drawing.Size(76, 20);
            this.textBox_B.TabIndex = 6;
            this.textBox_B.Text = "0,024";
            this.textBox_B.UseWaitCursor = true;
            // 
            // textBox_R
            // 
            this.textBox_R.Location = new System.Drawing.Point(55, 85);
            this.textBox_R.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_R.Name = "textBox_R";
            this.textBox_R.Size = new System.Drawing.Size(76, 20);
            this.textBox_R.TabIndex = 2;
            this.textBox_R.Text = "2,6";
            this.textBox_R.UseWaitCursor = true;
            // 
            // textBox_l
            // 
            this.textBox_l.Location = new System.Drawing.Point(55, 63);
            this.textBox_l.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_l.Name = "textBox_l";
            this.textBox_l.Size = new System.Drawing.Size(76, 20);
            this.textBox_l.TabIndex = 5;
            this.textBox_l.Text = "0,641";
            this.textBox_l.UseWaitCursor = true;
            // 
            // textBox_h_x
            // 
            this.textBox_h_x.Location = new System.Drawing.Point(55, 154);
            this.textBox_h_x.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_h_x.Name = "textBox_h_x";
            this.textBox_h_x.Size = new System.Drawing.Size(76, 20);
            this.textBox_h_x.TabIndex = 4;
            this.textBox_h_x.Text = "5,4";
            this.textBox_h_x.UseWaitCursor = true;
            // 
            // textBox_mmal
            // 
            this.textBox_mmal.Location = new System.Drawing.Point(55, 40);
            this.textBox_mmal.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_mmal.Name = "textBox_mmal";
            this.textBox_mmal.Size = new System.Drawing.Size(76, 20);
            this.textBox_mmal.TabIndex = 1;
            this.textBox_mmal.Text = "0,019";
            this.textBox_mmal.UseWaitCursor = true;
            // 
            // textBox_M
            // 
            this.textBox_M.Location = new System.Drawing.Point(55, 17);
            this.textBox_M.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_M.Name = "textBox_M";
            this.textBox_M.Size = new System.Drawing.Size(76, 20);
            this.textBox_M.TabIndex = 0;
            this.textBox_M.Text = "1,073";
            this.textBox_M.UseWaitCursor = true;
            // 
            // textBox_fi
            // 
            this.textBox_fi.Location = new System.Drawing.Point(55, 17);
            this.textBox_fi.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_fi.Name = "textBox_fi";
            this.textBox_fi.Size = new System.Drawing.Size(76, 20);
            this.textBox_fi.TabIndex = 8;
            this.textBox_fi.Text = "0";
            this.textBox_fi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_x
            // 
            this.textBox_x.Location = new System.Drawing.Point(55, 63);
            this.textBox_x.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_x.Name = "textBox_x";
            this.textBox_x.Size = new System.Drawing.Size(76, 20);
            this.textBox_x.TabIndex = 9;
            this.textBox_x.Text = "5";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Button_setInitVal);
            this.groupBox2.Controls.Add(this.labelfi);
            this.groupBox2.Controls.Add(this.labeldfi_dt);
            this.groupBox2.Controls.Add(this.labelx);
            this.groupBox2.Controls.Add(this.labeldx_dt);
            this.groupBox2.Controls.Add(this.textBox_fi);
            this.groupBox2.Controls.Add(this.textBox_dx_dt);
            this.groupBox2.Controls.Add(this.textBox_x);
            this.groupBox2.Controls.Add(this.textBox_dfi_dt);
            this.groupBox2.Location = new System.Drawing.Point(9, 284);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(135, 134);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Исходное значение";
            // 
            // Button_setInitVal
            // 
            this.Button_setInitVal.Location = new System.Drawing.Point(55, 108);
            this.Button_setInitVal.Margin = new System.Windows.Forms.Padding(2);
            this.Button_setInitVal.Name = "Button_setInitVal";
            this.Button_setInitVal.Size = new System.Drawing.Size(75, 19);
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
            // groupBox3
            // 
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
            this.groupBox3.Location = new System.Drawing.Point(148, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(191, 179);
            this.groupBox3.TabIndex = 13;
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
            this.Button_setCalcParam.Size = new System.Drawing.Size(75, 19);
            this.Button_setCalcParam.TabIndex = 9;
            this.Button_setCalcParam.Text = "Принять";
            this.Button_setCalcParam.UseVisualStyleBackColor = true;
            this.Button_setCalcParam.Click += new System.EventHandler(this.Button_setCalcParam_Click);
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
            this.textBox_t_stop.Text = "10";
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
            // roots_gbox
            // 
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
            this.roots_gbox.Location = new System.Drawing.Point(148, 194);
            this.roots_gbox.Name = "roots_gbox";
            this.roots_gbox.Size = new System.Drawing.Size(191, 237);
            this.roots_gbox.TabIndex = 32;
            this.roots_gbox.TabStop = false;
            this.roots_gbox.Text = "Желаемые корни хар. полинома";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(83, 146);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 23);
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
            this.tbox_lambda4_im.Text = "-1";
            this.tbox_lambda4_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda4_re
            // 
            this.tbox_lambda4_re.Location = new System.Drawing.Point(83, 120);
            this.tbox_lambda4_re.Name = "tbox_lambda4_re";
            this.tbox_lambda4_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda4_re.TabIndex = 41;
            this.tbox_lambda4_re.Text = "-1";
            this.tbox_lambda4_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda2_im
            // 
            this.tbox_lambda2_im.Location = new System.Drawing.Point(133, 68);
            this.tbox_lambda2_im.Name = "tbox_lambda2_im";
            this.tbox_lambda2_im.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda2_im.TabIndex = 40;
            this.tbox_lambda2_im.Text = "-1";
            this.tbox_lambda2_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda2_re
            // 
            this.tbox_lambda2_re.Location = new System.Drawing.Point(83, 68);
            this.tbox_lambda2_re.Name = "tbox_lambda2_re";
            this.tbox_lambda2_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda2_re.TabIndex = 39;
            this.tbox_lambda2_re.Text = "1";
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
            this.tbox_lambda3_im.Text = "1";
            this.tbox_lambda3_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbox_lambda3_im.TextChanged += new System.EventHandler(this.Tbox_lambda3_im_TextChanged);
            // 
            // tbox_lambda1_re
            // 
            this.tbox_lambda1_re.Location = new System.Drawing.Point(83, 42);
            this.tbox_lambda1_re.Name = "tbox_lambda1_re";
            this.tbox_lambda1_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda1_re.TabIndex = 12;
            this.tbox_lambda1_re.Text = "1";
            this.tbox_lambda1_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda3_re
            // 
            this.tbox_lambda3_re.Location = new System.Drawing.Point(83, 94);
            this.tbox_lambda3_re.Name = "tbox_lambda3_re";
            this.tbox_lambda3_re.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda3_re.TabIndex = 14;
            this.tbox_lambda3_re.Text = "-1";
            this.tbox_lambda3_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbox_lambda1_im
            // 
            this.tbox_lambda1_im.Location = new System.Drawing.Point(133, 42);
            this.tbox_lambda1_im.Name = "tbox_lambda1_im";
            this.tbox_lambda1_im.Size = new System.Drawing.Size(31, 20);
            this.tbox_lambda1_im.TabIndex = 13;
            this.tbox_lambda1_im.Text = "1";
            this.tbox_lambda1_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbox_lambda1_im.TextChanged += new System.EventHandler(this.Tbox_lambda1_im_TextChanged);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(344, 17);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(500, 350);
            this.chart1.TabIndex = 33;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(850, 17);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(500, 350);
            this.chart2.TabIndex = 34;
            this.chart2.Text = "chart2";
            // 
            // chart3
            // 
            chartArea3.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chart3.Legends.Add(legend3);
            this.chart3.Location = new System.Drawing.Point(344, 373);
            this.chart3.Name = "chart3";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart3.Series.Add(series3);
            this.chart3.Size = new System.Drawing.Size(500, 350);
            this.chart3.TabIndex = 33;
            this.chart3.Text = "chart1";
            // 
            // chart4
            // 
            chartArea4.Name = "ChartArea1";
            this.chart4.ChartAreas.Add(chartArea4);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chart4.Legends.Add(legend4);
            this.chart4.Location = new System.Drawing.Point(850, 373);
            this.chart4.Name = "chart4";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            series4.XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.chart4.Series.Add(series4);
            this.chart4.Size = new System.Drawing.Size(500, 350);
            this.chart4.TabIndex = 33;
            this.chart4.Text = "chart1";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Controls.Add(this.textBox2);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.label21);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.textBox3);
            this.groupBox4.Controls.Add(this.textBox4);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Controls.Add(this.textBox7);
            this.groupBox4.Controls.Add(this.textBox8);
            this.groupBox4.Controls.Add(this.textBox9);
            this.groupBox4.Controls.Add(this.textBox10);
            this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox4.Location = new System.Drawing.Point(9, 10);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(135, 270);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Параметры модели";
            this.groupBox4.UseWaitCursor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(55, 245);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 19);
            this.button1.TabIndex = 27;
            this.button1.Text = "Принять";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.UseWaitCursor = true;
            this.button1.Click += new System.EventHandler(this.Button_setParam_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(55, 222);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(76, 20);
            this.textBox1.TabIndex = 26;
            this.textBox1.Text = "0,00767";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.UseWaitCursor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(55, 199);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(76, 20);
            this.textBox2.TabIndex = 25;
            this.textBox2.Text = "4,481";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.UseWaitCursor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 202);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "gamma:";
            this.label5.UseWaitCursor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 226);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "E:";
            this.label6.UseWaitCursor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 158);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "h_x:";
            this.label12.UseWaitCursor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 180);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "B:";
            this.label13.UseWaitCursor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 158);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 13);
            this.label14.TabIndex = 19;
            this.label14.UseWaitCursor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 135);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(27, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "h_fi:";
            this.label15.UseWaitCursor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(4, 112);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(16, 13);
            this.label17.TabIndex = 17;
            this.label17.Text = "g:";
            this.label17.UseWaitCursor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(4, 89);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(18, 13);
            this.label19.TabIndex = 16;
            this.label19.Text = "R:";
            this.label19.UseWaitCursor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(4, 67);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(12, 13);
            this.label20.TabIndex = 15;
            this.label20.Text = "l:";
            this.label20.UseWaitCursor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(4, 44);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(18, 13);
            this.label21.TabIndex = 14;
            this.label21.Text = "m:";
            this.label21.UseWaitCursor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(4, 21);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(19, 13);
            this.label22.TabIndex = 13;
            this.label22.Text = "M:";
            this.label22.UseWaitCursor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(55, 131);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(76, 20);
            this.textBox3.TabIndex = 7;
            this.textBox3.Text = "0,0024";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox3.UseWaitCursor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(55, 108);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(76, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.Text = "9,81";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox4.UseWaitCursor = true;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(55, 176);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(76, 20);
            this.textBox5.TabIndex = 6;
            this.textBox5.Text = "0,024";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox5.UseWaitCursor = true;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(55, 85);
            this.textBox6.Margin = new System.Windows.Forms.Padding(2);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(76, 20);
            this.textBox6.TabIndex = 2;
            this.textBox6.Text = "2,6";
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox6.UseWaitCursor = true;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(55, 63);
            this.textBox7.Margin = new System.Windows.Forms.Padding(2);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(76, 20);
            this.textBox7.TabIndex = 5;
            this.textBox7.Text = "0,641";
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox7.UseWaitCursor = true;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(55, 154);
            this.textBox8.Margin = new System.Windows.Forms.Padding(2);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(76, 20);
            this.textBox8.TabIndex = 4;
            this.textBox8.Text = "5,4";
            this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox8.UseWaitCursor = true;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(55, 40);
            this.textBox9.Margin = new System.Windows.Forms.Padding(2);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(76, 20);
            this.textBox9.TabIndex = 1;
            this.textBox9.Text = "0,019";
            this.textBox9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox9.UseWaitCursor = true;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(55, 17);
            this.textBox10.Margin = new System.Windows.Forms.Padding(2);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(76, 20);
            this.textBox10.TabIndex = 0;
            this.textBox10.Text = "1,073";
            this.textBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox10.UseWaitCursor = true;
            // 
            // Timer1
            // 
            this.Timer1.Interval = 10;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
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
            // cBox_non_linear
            // 
            this.cBox_non_linear.AutoSize = true;
            this.cBox_non_linear.Location = new System.Drawing.Point(148, 437);
            this.cBox_non_linear.Name = "cBox_non_linear";
            this.cBox_non_linear.Size = new System.Drawing.Size(176, 17);
            this.cBox_non_linear.TabIndex = 35;
            this.cBox_non_linear.Text = "Нелинеаризованная система";
            this.cBox_non_linear.UseVisualStyleBackColor = true;
            // 
            // PortalCraneModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1364, 768);
            this.Controls.Add(this.cBox_non_linear);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart4);
            this.Controls.Add(this.chart3);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.roots_gbox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PortalCraneModel";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "PortalCraneModel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.roots_gbox.ResumeLayout(false);
            this.roots_gbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

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
            tbox_lambda1_re.Text = "";
            tbox_lambda1_im.Text = "";

            tbox_lambda2_re.Text = "";
            tbox_lambda2_re.ReadOnly = false;
            tbox_lambda2_im.Text = "";
            tbox_lambda2_im.ReadOnly = false;

            tbox_lambda3_re.Text = "";
            tbox_lambda3_im.Text = "";

            tbox_lambda4_re.Text = "";
            tbox_lambda4_re.ReadOnly = false;
            tbox_lambda4_im.Text = "";
            tbox_lambda4_im.ReadOnly = false;
        }
    }
}
