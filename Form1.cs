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

namespace PortalCraneModel
{
  public class PortalCraneModel : Form
  {
    const string dll = "PortalCraneCalc.dll";
    public double[] DrawPoints;
    public IntPtr ptrTAllDrawPoints;
    public PortalCraneModel.TAllDrawPoints allPoints;
    public PortalCraneModel.TStatePoint CurrentPoint;
    public IntPtr PtrNextPoint;
    public int dinPointsCount;
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
    private CheckBox checkBox_dinDraw;
    private Label label4;
    private Label label3;
    private Label label2;
    private Label label1;
    private TextBox textBox_drawStCount;
    private TextBox textBox_t_stop;
    private TextBox textBox_t_start;
    private TextBox textBox_step;
    private GroupBox groupBox4;
    private Button Button_setScale;
    private Label label6;
    private Label label5;
    private TextBox textBox_yMax;
    private TextBox textBox_xMax;
    private Button Button_runCalc;
    private Timer Timer1;
    private PictureBox PictureBox1;
    private PictureBox PictureBox2;
    private PictureBox PictureBox3;
    private PictureBox PictureBox4;
    private Label label8;
    private Label label9;
    private Label label10;
    private Label label11;
    private Label label12;
    private Label label13;
    private Label label14;
    private Label label15;
    private Label label16;
    private Label textBox_run_time;

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetModelParams(double _M, double _m, double _l, double _R, double _g, double _h_fi, double _h_x, double _B, double _gamma, double _E);
    
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
    public static extern void GetAllDrawPoints(IntPtr ptrAllDrawPoints);

    public PortalCraneModel()
    {
      this.InitializeComponent();
      this.allPoints = new PortalCraneModel.TAllDrawPoints();
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
        this.CurrentPoint = (PortalCraneModel.TStatePoint) Marshal.PtrToStructure(this.PtrNextPoint, typeof (PortalCraneModel.TStatePoint));
        this.DrawPoints[this.dinPointsCount * 5] = this.CurrentPoint.fi;
        this.DrawPoints[this.dinPointsCount * 5 + 1] = this.CurrentPoint.dfi_dt;
        this.DrawPoints[this.dinPointsCount * 5 + 2] = this.CurrentPoint.x;
        this.DrawPoints[this.dinPointsCount * 5 + 3] = this.CurrentPoint.dx_dt;
        this.DrawPoints[this.dinPointsCount * 5 + 4] = this.CurrentPoint.t;
        ++this.dinPointsCount;
        this.PictureBox1.Refresh();
        this.PictureBox2.Refresh();
        this.PictureBox3.Refresh();
        this.PictureBox4.Refresh();
      }
    }

    private void PortalCraneModel_Load(object sender, EventArgs e)
    {
    }

    private void Button_setParam_Click(object sender, EventArgs e)
    {
      this.SetParam();
    }

    private void Button_setInitVal_Click(object sender, EventArgs e)
    {
      this.SetInitVal();
    }

    private void Button_setScale_Click(object sender, EventArgs e)
    {
      this.SetScale();
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
        this.SetScale(); // масштаб
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
            PortalCraneModel.GetAllDrawPoints(this.ptrTAllDrawPoints);
            this.allPoints = (PortalCraneModel.TAllDrawPoints)Marshal.PtrToStructure(this.ptrTAllDrawPoints, typeof(PortalCraneModel.TAllDrawPoints));
            Marshal.Copy(this.allPoints.allDrawPoints, this.DrawPoints, 0, this.allPoints.drawCount * 5);
            PortalCraneModel.DeleteAllPointsArray(this.ptrTAllDrawPoints);
            Marshal.FreeHGlobal(this.ptrTAllDrawPoints);
            this.PictureBox1.Refresh();
            this.PictureBox2.Refresh();
            this.PictureBox3.Refresh();
            this.PictureBox4.Refresh();
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
      PortalCraneModel.SetModelParams(this.M, this.m, this.l, this.R, this.g, this.h_fi, this.h_x, this.B, this.gamma, this.E);
    }

    private void SetInitVal()
    {
      this.fi = double.Parse(this.textBox_fi.Text);
      this.dfi_dt = double.Parse(this.textBox_dfi_dt.Text);
      this.x = double.Parse(this.textBox_x.Text);
      this.dx_dt = double.Parse(this.textBox_dx_dt.Text);
      PortalCraneModel.SetInitParams(this.fi, this.dfi_dt, this.x, this.dx_dt);
    }

    private void SetScale()
    {
      this.xMax = (double) float.Parse(this.textBox_xMax.Text);
      this.yMax = (double) float.Parse(this.textBox_yMax.Text);
      PortalCraneModel.SetScaleParams(this.xMax, this.yMax);
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
      double num1 = 0.5 * (double) (right + left);
      Rectangle clipRectangle2 = e.ClipRectangle;
      int bottom = clipRectangle2.Bottom;
      clipRectangle2 = e.ClipRectangle;
      int top = clipRectangle2.Top;
      double num2 = 0.5 * (double) (bottom + top);
      double height = (double) e.ClipRectangle.Height;
      double width = (double) e.ClipRectangle.Width;
      Pen pen = new Pen(Color.Black, 2f);
      int num3 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
      int red = 0;
      int green = 0;
      int blue = 0;
      for (int index = 1; index < num3; ++index)
      {
        switch (index * 5 / (int) byte.MaxValue % 3)
        {
          case 0:
            green = 0;
            blue = 0;
            red = index * 5 % (int) byte.MaxValue;
            break;
          case 1:
            red = 0;
            blue = 0;
            green = index * 5 % (int) byte.MaxValue;
            break;
          case 2:
            red = 0;
            green = 0;
            blue = index * 5 % (int) byte.MaxValue;
            break;
        }
        pen.Color = Color.FromArgb(red, green, blue);
        e.Graphics.DrawLine(pen, (float) (num1 + this.DrawPoints[(index - 1) * 5] / this.xMax * width / 2.0), (float) (num2 - this.DrawPoints[(index - 1) * 5 + 1] / this.yMax * height / 2.0), (float) (num1 + this.DrawPoints[index * 5] / this.xMax * width / 2.0), (float) (num2 - this.DrawPoints[index * 5 + 1] / this.yMax * height / 2.0));
      }
    }

    private void PictureBox2_Paint(object sender, PaintEventArgs e)
    {
      double left = (double) e.ClipRectangle.Left;
      Rectangle clipRectangle = e.ClipRectangle;
      int bottom = clipRectangle.Bottom;
      clipRectangle = e.ClipRectangle;
      int top = clipRectangle.Top;
      double num1 = 0.5 * (double) (bottom + top);
      double height = (double) e.ClipRectangle.Height;
      double width = (double) e.ClipRectangle.Width;
      Pen pen = new Pen(Color.Black, 2f);
      int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
      int red = 0;
      int green = 0;
      int blue = 0;
      for (int index = 1; index < num2; ++index)
      {
        switch (index * 5 / (int) byte.MaxValue % 5)
        {
          case 0:
            green = 0;
            blue = 0;
            red = index * 5 % (int) byte.MaxValue;
            break;
          case 1:
            red = 0;
            blue = 0;
            green = index * 5 % (int) byte.MaxValue;
            break;
          case 2:
            red = 0;
            green = 0;
            blue = index * 5 % (int) byte.MaxValue;
            break;
        }
        pen.Color = Color.FromArgb(red, green, blue);
        e.Graphics.DrawLine(pen, (float) (left + this.DrawPoints[(index - 1) * 5 + 2] / this.xMax * width / 15.0), (float) (num1 - this.DrawPoints[(index - 1) * 5 + 3] / this.yMax * height / 2.0), (float) (left + this.DrawPoints[index * 5 + 2] / this.xMax * width / 15.0), (float) (num1 - this.DrawPoints[index * 5 + 3] / this.yMax * height / 2.0));
      }
    }

    private void PictureBox3_Paint(object sender, PaintEventArgs e)
    {
      double left = (double) e.ClipRectangle.Left;
      Rectangle clipRectangle = e.ClipRectangle;
      int bottom = clipRectangle.Bottom;
      clipRectangle = e.ClipRectangle;
      int top = clipRectangle.Top;
      double num1 = 0.5 * (double) (bottom + top);
      double height = (double) e.ClipRectangle.Height;
      double width = (double) e.ClipRectangle.Width;
      Pen pen = new Pen(Color.Black, 2f);
      int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
      int red = 0;
      int green = 0;
      int blue = 0;
      for (int index = 1; index < num2; ++index)
      {
        switch (index * 5 / (int) byte.MaxValue % 5)
        {
          case 0:
            green = 0;
            blue = 0;
            red = index * 5 % (int) byte.MaxValue;
            break;
          case 1:
            red = 0;
            blue = 0;
            green = index * 5 % (int) byte.MaxValue;
            break;
          case 2:
            red = 0;
            green = 0;
            blue = index * 5 % (int) byte.MaxValue;
            break;
        }
        pen.Color = Color.FromArgb(red, green, blue);
        e.Graphics.DrawLine(pen, (float) (left + this.DrawPoints[(index - 1) * 5 + 4] / this.xMax * width / 70.0), (float) (num1 - this.DrawPoints[(index - 1) * 5] / this.yMax * height), (float) (left + this.DrawPoints[index * 5 + 4] / this.xMax * width / 70.0), (float) (num1 - this.DrawPoints[index * 5] / this.yMax * height));
      }
    }

    private void PictureBox4_Paint(object sender, PaintEventArgs e)
    {
      double left = (double) e.ClipRectangle.Left;
      Rectangle clipRectangle = e.ClipRectangle;
      int bottom = clipRectangle.Bottom;
      clipRectangle = e.ClipRectangle;
      int top = clipRectangle.Top;
      double num1 = 0.5 * (double) (bottom + top);
      double height = (double) e.ClipRectangle.Height;
      double width = (double) e.ClipRectangle.Width;
      Pen pen = new Pen(Color.Black, 2f);
      int num2 = !this.inDinamic ? this.allPoints.drawCount : this.dinPointsCount;
      int red = 0;
      int green = 0;
      int blue = 0;
      for (int index = 1; index < num2; ++index)
      {
        switch (index * 5 / (int) byte.MaxValue % 5)
        {
          case 0:
            green = 0;
            blue = 0;
            red = index * 5 % (int) byte.MaxValue;
            break;
          case 1:
            red = 0;
            blue = 0;
            green = index * 5 % (int) byte.MaxValue;
            break;
          case 2:
            red = 0;
            green = 0;
            blue = index * 5 % (int) byte.MaxValue;
            break;
        }
        pen.Color = Color.FromArgb(red, green, blue);
        e.Graphics.DrawLine(pen, (float) (left + this.DrawPoints[(index - 1) * 5 + 4] / this.xMax * width / 50.0), (float) (num1 - this.DrawPoints[(index - 1) * 5 + 2] / this.yMax * height / 50.0), (float) (left + this.DrawPoints[index * 5 + 4] / this.xMax * width / 50.0), (float) (num1 - this.DrawPoints[index * 5 + 2] / this.yMax * height / 50.0));
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
      this.components = (IContainer) new Container();
      this.groupBox1 = new GroupBox();
      this.Button_setParam = new Button();
      this.textBox_E = new TextBox();
      this.textBox_gamma = new TextBox();
      this.label_gamma = new Label();
      this.label_E = new Label();
      this.label_h_x = new Label();
      this.label_B = new Label();
      this.label7 = new Label();
      this.label_h_fi = new Label();
      this.label_g = new Label();
      this.label_R = new Label();
      this.label_l = new Label();
      this.label_mmal = new Label();
      this.label_M = new Label();
      this.textBox_h_fi = new TextBox();
      this.textBox_g = new TextBox();
      this.textBox_B = new TextBox();
      this.textBox_R = new TextBox();
      this.textBox_l = new TextBox();
      this.textBox_h_x = new TextBox();
      this.textBox_mmal = new TextBox();
      this.textBox_M = new TextBox();
      this.textBox_fi = new TextBox();
      this.textBox_x = new TextBox();
      this.textBox_dfi_dt = new TextBox();
      this.textBox_dx_dt = new TextBox();
      this.groupBox2 = new GroupBox();
      this.Button_setInitVal = new Button();
      this.labelfi = new Label();
      this.labeldfi_dt = new Label();
      this.labelx = new Label();
      this.labeldx_dt = new Label();
      this.groupBox3 = new GroupBox();
      this.textBox_run_time = new Label();
      this.label16 = new Label();
      this.Button_setCalcParam = new Button();
      this.checkBox_dinDraw = new CheckBox();
      this.label4 = new Label();
      this.label3 = new Label();
      this.label2 = new Label();
      this.label1 = new Label();
      this.textBox_drawStCount = new TextBox();
      this.textBox_t_stop = new TextBox();
      this.textBox_t_start = new TextBox();
      this.textBox_step = new TextBox();
      this.groupBox4 = new GroupBox();
      this.Button_setScale = new Button();
      this.label6 = new Label();
      this.label5 = new Label();
      this.textBox_yMax = new TextBox();
      this.textBox_xMax = new TextBox();
      this.Button_runCalc = new Button();
      this.Timer1 = new Timer(this.components);
      this.PictureBox1 = new PictureBox();
      this.PictureBox2 = new PictureBox();
      this.PictureBox3 = new PictureBox();
      this.PictureBox4 = new PictureBox();
      this.label8 = new Label();
      this.label9 = new Label();
      this.label10 = new Label();
      this.label11 = new Label();
      this.label12 = new Label();
      this.label13 = new Label();
      this.label14 = new Label();
      this.label15 = new Label();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      ((ISupportInitialize) this.PictureBox1).BeginInit();
      ((ISupportInitialize) this.PictureBox2).BeginInit();
      ((ISupportInitialize) this.PictureBox3).BeginInit();
      ((ISupportInitialize) this.PictureBox4).BeginInit();
      this.SuspendLayout();
      this.groupBox1.BackColor = SystemColors.Control;
      this.groupBox1.Controls.Add((Control) this.Button_setParam);
      this.groupBox1.Controls.Add((Control) this.textBox_E);
      this.groupBox1.Controls.Add((Control) this.textBox_gamma);
      this.groupBox1.Controls.Add((Control) this.label_gamma);
      this.groupBox1.Controls.Add((Control) this.label_E);
      this.groupBox1.Controls.Add((Control) this.label_h_x);
      this.groupBox1.Controls.Add((Control) this.label_B);
      this.groupBox1.Controls.Add((Control) this.label7);
      this.groupBox1.Controls.Add((Control) this.label_h_fi);
      this.groupBox1.Controls.Add((Control) this.label_g);
      this.groupBox1.Controls.Add((Control) this.label_R);
      this.groupBox1.Controls.Add((Control) this.label_l);
      this.groupBox1.Controls.Add((Control) this.label_mmal);
      this.groupBox1.Controls.Add((Control) this.label_M);
      this.groupBox1.Controls.Add((Control) this.textBox_h_fi);
      this.groupBox1.Controls.Add((Control) this.textBox_g);
      this.groupBox1.Controls.Add((Control) this.textBox_B);
      this.groupBox1.Controls.Add((Control) this.textBox_R);
      this.groupBox1.Controls.Add((Control) this.textBox_l);
      this.groupBox1.Controls.Add((Control) this.textBox_h_x);
      this.groupBox1.Controls.Add((Control) this.textBox_mmal);
      this.groupBox1.Controls.Add((Control) this.textBox_M);
      this.groupBox1.FlatStyle = FlatStyle.Flat;
      this.groupBox1.Location = new Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(180, 332);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Параметры модели";
      this.groupBox1.UseWaitCursor = true;
      this.Button_setParam.Location = new Point(73, 301);
      this.Button_setParam.Name = "Button_setParam";
      this.Button_setParam.Size = new Size(100, 23);
      this.Button_setParam.TabIndex = 27;
      this.Button_setParam.Text = "Принять";
      this.Button_setParam.UseVisualStyleBackColor = true;
      this.Button_setParam.UseWaitCursor = true;
      this.Button_setParam.Click += new EventHandler(this.Button_setParam_Click);
      this.textBox_E.Location = new Point(73, 273);
      this.textBox_E.Name = "textBox_E";
      this.textBox_E.Size = new Size(100, 22);
      this.textBox_E.TabIndex = 26;
      this.textBox_E.Text = "0,00767";
      this.textBox_E.UseWaitCursor = true;
      this.textBox_gamma.Location = new Point(73, 245);
      this.textBox_gamma.Name = "textBox_gamma";
      this.textBox_gamma.Size = new Size(100, 22);
      this.textBox_gamma.TabIndex = 25;
      this.textBox_gamma.Text = "4,481";
      this.textBox_gamma.UseWaitCursor = true;
      this.label_gamma.AutoSize = true;
      this.label_gamma.Location = new Point(6, 248);
      this.label_gamma.Name = "label_gamma";
      this.label_gamma.Size = new Size(58, 17);
      this.label_gamma.TabIndex = 24;
      this.label_gamma.Text = "gamma:";
      this.label_gamma.UseWaitCursor = true;
      this.label_E.AutoSize = true;
      this.label_E.Location = new Point(6, 278);
      this.label_E.Name = "label_E";
      this.label_E.Size = new Size(21, 17);
      this.label_E.TabIndex = 23;
      this.label_E.Text = "E:";
      this.label_E.UseWaitCursor = true;
      this.label_h_x.AutoSize = true;
      this.label_h_x.Location = new Point(6, 194);
      this.label_h_x.Name = "label_h_x";
      this.label_h_x.Size = new Size(34, 17);
      this.label_h_x.TabIndex = 21;
      this.label_h_x.Text = "h_x:";
      this.label_h_x.UseWaitCursor = true;
      this.label_B.AutoSize = true;
      this.label_B.Location = new Point(6, 222);
      this.label_B.Name = "label_B";
      this.label_B.Size = new Size(21, 17);
      this.label_B.TabIndex = 20;
      this.label_B.Text = "B:";
      this.label_B.UseWaitCursor = true;
      this.label7.AutoSize = true;
      this.label7.Location = new Point(21, 194);
      this.label7.Name = "label7";
      this.label7.Size = new Size(0, 17);
      this.label7.TabIndex = 19;
      this.label7.UseWaitCursor = true;
      this.label_h_fi.AutoSize = true;
      this.label_h_fi.Location = new Point(6, 166);
      this.label_h_fi.Name = "label_h_fi";
      this.label_h_fi.Size = new Size(35, 17);
      this.label_h_fi.TabIndex = 18;
      this.label_h_fi.Text = "h_fi:";
      this.label_h_fi.UseWaitCursor = true;
      this.label_g.AutoSize = true;
      this.label_g.Location = new Point(6, 138);
      this.label_g.Name = "label_g";
      this.label_g.Size = new Size(20, 17);
      this.label_g.TabIndex = 17;
      this.label_g.Text = "g:";
      this.label_g.UseWaitCursor = true;
      this.label_R.AutoSize = true;
      this.label_R.Location = new Point(6, 110);
      this.label_R.Name = "label_R";
      this.label_R.Size = new Size(22, 17);
      this.label_R.TabIndex = 16;
      this.label_R.Text = "R:";
      this.label_R.UseWaitCursor = true;
      this.label_l.AutoSize = true;
      this.label_l.Location = new Point(6, 82);
      this.label_l.Name = "label_l";
      this.label_l.Size = new Size(15, 17);
      this.label_l.TabIndex = 15;
      this.label_l.Text = "l:";
      this.label_l.UseWaitCursor = true;
      this.label_mmal.AutoSize = true;
      this.label_mmal.Location = new Point(6, 54);
      this.label_mmal.Name = "label_mmal";
      this.label_mmal.Size = new Size(23, 17);
      this.label_mmal.TabIndex = 14;
      this.label_mmal.Text = "m:";
      this.label_mmal.UseWaitCursor = true;
      this.label_M.AutoSize = true;
      this.label_M.Location = new Point(6, 26);
      this.label_M.Name = "label_M";
      this.label_M.Size = new Size(23, 17);
      this.label_M.TabIndex = 13;
      this.label_M.Text = "M:";
      this.label_M.UseWaitCursor = true;
      this.textBox_h_fi.Location = new Point(73, 161);
      this.textBox_h_fi.Name = "textBox_h_fi";
      this.textBox_h_fi.Size = new Size(100, 22);
      this.textBox_h_fi.TabIndex = 7;
      this.textBox_h_fi.Text = "0,0024";
      this.textBox_h_fi.UseWaitCursor = true;
      this.textBox_g.Location = new Point(73, 133);
      this.textBox_g.Name = "textBox_g";
      this.textBox_g.Size = new Size(100, 22);
      this.textBox_g.TabIndex = 3;
      this.textBox_g.Text = "9,81";
      this.textBox_g.UseWaitCursor = true;
      this.textBox_B.Location = new Point(73, 217);
      this.textBox_B.Name = "textBox_B";
      this.textBox_B.Size = new Size(100, 22);
      this.textBox_B.TabIndex = 6;
      this.textBox_B.Text = "0,024";
      this.textBox_B.UseWaitCursor = true;
      this.textBox_R.Location = new Point(73, 105);
      this.textBox_R.Name = "textBox_R";
      this.textBox_R.Size = new Size(100, 22);
      this.textBox_R.TabIndex = 2;
      this.textBox_R.Text = "2,6";
      this.textBox_R.UseWaitCursor = true;
      this.textBox_l.Location = new Point(73, 77);
      this.textBox_l.Name = "textBox_l";
      this.textBox_l.Size = new Size(100, 22);
      this.textBox_l.TabIndex = 5;
      this.textBox_l.Text = "0,641";
      this.textBox_l.UseWaitCursor = true;
      this.textBox_h_x.Location = new Point(73, 189);
      this.textBox_h_x.Name = "textBox_h_x";
      this.textBox_h_x.Size = new Size(100, 22);
      this.textBox_h_x.TabIndex = 4;
      this.textBox_h_x.Text = "5,4";
      this.textBox_h_x.UseWaitCursor = true;
      this.textBox_mmal.Location = new Point(73, 49);
      this.textBox_mmal.Name = "textBox_mmal";
      this.textBox_mmal.Size = new Size(100, 22);
      this.textBox_mmal.TabIndex = 1;
      this.textBox_mmal.Text = "0,019";
      this.textBox_mmal.UseWaitCursor = true;
      this.textBox_M.Location = new Point(73, 21);
      this.textBox_M.Name = "textBox_M";
      this.textBox_M.Size = new Size(100, 22);
      this.textBox_M.TabIndex = 0;
      this.textBox_M.Text = "1,073";
      this.textBox_M.UseWaitCursor = true;
      this.textBox_fi.Location = new Point(73, 21);
      this.textBox_fi.Name = "textBox_fi";
      this.textBox_fi.Size = new Size(100, 22);
      this.textBox_fi.TabIndex = 8;
      this.textBox_fi.Text = "0";
      this.textBox_x.Location = new Point(73, 77);
      this.textBox_x.Name = "textBox_x";
      this.textBox_x.Size = new Size(100, 22);
      this.textBox_x.TabIndex = 9;
      this.textBox_x.Text = "0";
      this.textBox_dfi_dt.Location = new Point(73, 49);
      this.textBox_dfi_dt.Name = "textBox_dfi_dt";
      this.textBox_dfi_dt.Size = new Size(100, 22);
      this.textBox_dfi_dt.TabIndex = 10;
      this.textBox_dfi_dt.Text = "0";
      this.textBox_dx_dt.Location = new Point(73, 105);
      this.textBox_dx_dt.Name = "textBox_dx_dt";
      this.textBox_dx_dt.Size = new Size(100, 22);
      this.textBox_dx_dt.TabIndex = 11;
      this.textBox_dx_dt.Text = "0";
      this.groupBox2.Controls.Add((Control) this.Button_setInitVal);
      this.groupBox2.Controls.Add((Control) this.labelfi);
      this.groupBox2.Controls.Add((Control) this.labeldfi_dt);
      this.groupBox2.Controls.Add((Control) this.labelx);
      this.groupBox2.Controls.Add((Control) this.labeldx_dt);
      this.groupBox2.Controls.Add((Control) this.textBox_fi);
      this.groupBox2.Controls.Add((Control) this.textBox_dx_dt);
      this.groupBox2.Controls.Add((Control) this.textBox_x);
      this.groupBox2.Controls.Add((Control) this.textBox_dfi_dt);
      this.groupBox2.Location = new Point(12, 350);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(180, 165);
      this.groupBox2.TabIndex = 12;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Исходное значение";
      this.Button_setInitVal.Location = new Point(73, 133);
      this.Button_setInitVal.Name = "Button_setInitVal";
      this.Button_setInitVal.Size = new Size(100, 23);
      this.Button_setInitVal.TabIndex = 25;
      this.Button_setInitVal.Text = "Принять";
      this.Button_setInitVal.UseVisualStyleBackColor = true;
      this.Button_setInitVal.Click += new EventHandler(this.Button_setInitVal_Click);
      this.labelfi.AutoSize = true;
      this.labelfi.Location = new Point(6, 26);
      this.labelfi.Name = "labelfi";
      this.labelfi.Size = new Size(19, 17);
      this.labelfi.TabIndex = 21;
      this.labelfi.Text = "fi:";
      this.labeldfi_dt.AutoSize = true;
      this.labeldfi_dt.Location = new Point(6, 54);
      this.labeldfi_dt.Name = "labeldfi_dt";
      this.labeldfi_dt.Size = new Size(43, 17);
      this.labeldfi_dt.TabIndex = 22;
      this.labeldfi_dt.Text = "dfi/dt:";
      this.labelx.AutoSize = true;
      this.labelx.Location = new Point(6, 82);
      this.labelx.Name = "labelx";
      this.labelx.Size = new Size(42, 17);
      this.labelx.TabIndex = 23;
      this.labelx.Text = "x - x*:";
      this.labeldx_dt.AutoSize = true;
      this.labeldx_dt.Location = new Point(6, 108);
      this.labeldx_dt.Name = "labeldx_dt";
      this.labeldx_dt.Size = new Size(42, 17);
      this.labeldx_dt.TabIndex = 24;
      this.labeldx_dt.Text = "dx/dt:";
      this.groupBox3.Controls.Add((Control) this.textBox_run_time);
      this.groupBox3.Controls.Add((Control) this.label16);
      this.groupBox3.Controls.Add((Control) this.Button_setCalcParam);
      this.groupBox3.Controls.Add((Control) this.checkBox_dinDraw);
      this.groupBox3.Controls.Add((Control) this.label4);
      this.groupBox3.Controls.Add((Control) this.label3);
      this.groupBox3.Controls.Add((Control) this.label2);
      this.groupBox3.Controls.Add((Control) this.label1);
      this.groupBox3.Controls.Add((Control) this.textBox_drawStCount);
      this.groupBox3.Controls.Add((Control) this.textBox_t_stop);
      this.groupBox3.Controls.Add((Control) this.textBox_t_start);
      this.groupBox3.Controls.Add((Control) this.textBox_step);
      this.groupBox3.Location = new Point(198, 126);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new Size((int) byte.MaxValue, 220);
      this.groupBox3.TabIndex = 13;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Параметры расчета";
      this.textBox_run_time.AutoSize = true;
      this.textBox_run_time.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.textBox_run_time.Location = new Point(144, 187);
      this.textBox_run_time.Name = "textBox_run_time";
      this.textBox_run_time.Size = new Size(0, 25);
      this.textBox_run_time.TabIndex = 11;
      this.label16.AutoSize = true;
      this.label16.Location = new Point(7, 191);
      this.label16.Name = "label16";
      this.label16.Size = new Size(139, 17);
      this.label16.TabIndex = 10;
      this.label16.Text = "Время выполнения:";
      this.Button_setCalcParam.Location = new Point(6, 160);
      this.Button_setCalcParam.Name = "Button_setCalcParam";
      this.Button_setCalcParam.Size = new Size(100, 23);
      this.Button_setCalcParam.TabIndex = 9;
      this.Button_setCalcParam.Text = "Принять";
      this.Button_setCalcParam.UseVisualStyleBackColor = true;
      this.Button_setCalcParam.Click += new EventHandler(this.Button_setCalcParam_Click);
      this.checkBox_dinDraw.AutoSize = true;
      this.checkBox_dinDraw.Location = new Point(9, 133);
      this.checkBox_dinDraw.Name = "checkBox_dinDraw";
      this.checkBox_dinDraw.Size = new Size(170, 21);
      this.checkBox_dinDraw.TabIndex = 8;
      this.checkBox_dinDraw.Text = "Рисовать в динамике";
      this.checkBox_dinDraw.UseVisualStyleBackColor = true;
      this.label4.AutoSize = true;
      this.label4.Location = new Point(6, 108);
      this.label4.Name = "label4";
      this.label4.Size = new Size(125, 17);
      this.label4.TabIndex = 7;
      this.label4.Text = "Шагов инт. в шаге";
      this.label3.AutoSize = true;
      this.label3.Location = new Point(6, 82);
      this.label3.Name = "label3";
      this.label3.Size = new Size(51, 17);
      this.label3.TabIndex = 6;
      this.label3.Text = "t_stop:";
      this.label2.AutoSize = true;
      this.label2.Location = new Point(6, 54);
      this.label2.Name = "label2";
      this.label2.Size = new Size(52, 17);
      this.label2.TabIndex = 5;
      this.label2.Text = "t_start:";
      this.label1.AutoSize = true;
      this.label1.Location = new Point(6, 24);
      this.label1.Name = "label1";
      this.label1.Size = new Size(67, 17);
      this.label1.TabIndex = 4;
      this.label1.Text = "Шаг инт.:";
      this.textBox_drawStCount.Location = new Point(149, 105);
      this.textBox_drawStCount.Name = "textBox_drawStCount";
      this.textBox_drawStCount.Size = new Size(100, 22);
      this.textBox_drawStCount.TabIndex = 3;
      this.textBox_drawStCount.Text = "10";
      this.textBox_t_stop.Location = new Point(149, 77);
      this.textBox_t_stop.Name = "textBox_t_stop";
      this.textBox_t_stop.Size = new Size(100, 22);
      this.textBox_t_stop.TabIndex = 2;
      this.textBox_t_stop.Text = "10";
      this.textBox_t_start.Location = new Point(149, 49);
      this.textBox_t_start.Name = "textBox_t_start";
      this.textBox_t_start.Size = new Size(100, 22);
      this.textBox_t_start.TabIndex = 1;
      this.textBox_t_start.Text = "0";
      this.textBox_step.Location = new Point(149, 21);
      this.textBox_step.Name = "textBox_step";
      this.textBox_step.Size = new Size(100, 22);
      this.textBox_step.TabIndex = 0;
      this.textBox_step.Text = "0,005";
      this.groupBox4.Controls.Add((Control) this.Button_setScale);
      this.groupBox4.Controls.Add((Control) this.label6);
      this.groupBox4.Controls.Add((Control) this.label5);
      this.groupBox4.Controls.Add((Control) this.textBox_yMax);
      this.groupBox4.Controls.Add((Control) this.textBox_xMax);
      this.groupBox4.Location = new Point(198, 12);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new Size((int) byte.MaxValue, 108);
      this.groupBox4.TabIndex = 14;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Масштаб";
      this.Button_setScale.Location = new Point(30, 76);
      this.Button_setScale.Name = "Button_setScale";
      this.Button_setScale.Size = new Size(100, 23);
      this.Button_setScale.TabIndex = 4;
      this.Button_setScale.Text = "Принять";
      this.Button_setScale.UseVisualStyleBackColor = true;
      this.Button_setScale.Click += new EventHandler(this.Button_setScale_Click);
      this.label6.AutoSize = true;
      this.label6.Location = new Point(7, 52);
      this.label6.Name = "label6";
      this.label6.Size = new Size(19, 17);
      this.label6.TabIndex = 3;
      this.label6.Text = "y:";
      this.label5.AutoSize = true;
      this.label5.Location = new Point(7, 24);
      this.label5.Name = "label5";
      this.label5.Size = new Size(18, 17);
      this.label5.TabIndex = 2;
      this.label5.Text = "x:";
      this.textBox_yMax.Location = new Point(30, 49);
      this.textBox_yMax.Name = "textBox_yMax";
      this.textBox_yMax.Size = new Size(100, 22);
      this.textBox_yMax.TabIndex = 1;
      this.textBox_yMax.Text = "0,4";
      this.textBox_xMax.Location = new Point(30, 21);
      this.textBox_xMax.Name = "textBox_xMax";
      this.textBox_xMax.Size = new Size(100, 22);
      this.textBox_xMax.TabIndex = 0;
      this.textBox_xMax.Text = "0,15";
      this.Button_runCalc.Location = new Point(198, 352);
      this.Button_runCalc.Name = "Button_runCalc";
      this.Button_runCalc.Size = new Size((int) byte.MaxValue, 23);
      this.Button_runCalc.TabIndex = 19;
      this.Button_runCalc.Text = "Рассчитать";
      this.Button_runCalc.UseVisualStyleBackColor = true;
      this.Button_runCalc.Click += new EventHandler(this.Button_runCalc_Click);
      this.Timer1.Interval = 10;
      this.Timer1.Tick += new EventHandler(this.Timer1_Tick);
      this.PictureBox1.BackColor = SystemColors.Window;
      this.PictureBox1.BorderStyle = BorderStyle.FixedSingle;
      this.PictureBox1.Location = new Point(516, 20);
      this.PictureBox1.Name = "PictureBox1";
      this.PictureBox1.Size = new Size(377, 257);
      this.PictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.PictureBox1.TabIndex = 20;
      this.PictureBox1.TabStop = false;
      this.PictureBox1.Paint += new PaintEventHandler(this.PictureBox1_Paint);
      this.PictureBox2.BackColor = SystemColors.Window;
      this.PictureBox2.BorderStyle = BorderStyle.FixedSingle;
      this.PictureBox2.Location = new Point(968, 20);
      this.PictureBox2.Name = "PictureBox2";
      this.PictureBox2.Size = new Size(377, 257);
      this.PictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
      this.PictureBox2.TabIndex = 21;
      this.PictureBox2.TabStop = false;
      this.PictureBox2.Paint += new PaintEventHandler(this.PictureBox2_Paint);
      this.PictureBox3.BackColor = SystemColors.Window;
      this.PictureBox3.BorderStyle = BorderStyle.FixedSingle;
      this.PictureBox3.Location = new Point(516, 336);
      this.PictureBox3.Name = "PictureBox3";
      this.PictureBox3.Size = new Size(377, 257);
      this.PictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
      this.PictureBox3.TabIndex = 22;
      this.PictureBox3.TabStop = false;
      this.PictureBox3.Paint += new PaintEventHandler(this.PictureBox3_Paint);
      this.PictureBox4.BackColor = SystemColors.Window;
      this.PictureBox4.BorderStyle = BorderStyle.FixedSingle;
      this.PictureBox4.Location = new Point(968, 336);
      this.PictureBox4.Name = "PictureBox4";
      this.PictureBox4.Size = new Size(377, 257);
      this.PictureBox4.TabIndex = 23;
      this.PictureBox4.TabStop = false;
      this.PictureBox4.Paint += new PaintEventHandler(this.PictureBox4_Paint);
      this.label8.AutoSize = true;
      this.label8.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label8.Location = new Point(872, 280);
      this.label8.Name = "label8";
      this.label8.Size = new Size(21, 25);
      this.label8.TabIndex = 24;
      this.label8.Text = "fi";
      this.label9.AutoSize = true;
      this.label9.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label9.Location = new Point(456, 20);
      this.label9.Name = "label9";
      this.label9.Size = new Size(54, 25);
      this.label9.TabIndex = 25;
      this.label9.Text = "dfi/dt";
      this.label10.AutoSize = true;
      this.label10.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label10.Location = new Point(902, 20);
      this.label10.Name = "label10";
      this.label10.Size = new Size(60, 25);
      this.label10.TabIndex = 26;
      this.label10.Text = "dx_dt";
      this.label11.AutoSize = true;
      this.label11.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label11.Location = new Point(1323, 280);
      this.label11.Name = "label11";
      this.label11.Size = new Size(22, 25);
      this.label11.TabIndex = 27;
      this.label11.Text = "x";
      this.label12.AutoSize = true;
      this.label12.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label12.Location = new Point(489, 336);
      this.label12.Name = "label12";
      this.label12.Size = new Size(21, 25);
      this.label12.TabIndex = 28;
      this.label12.Text = "fi";
      this.label13.AutoSize = true;
      this.label13.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label13.Location = new Point(876, 596);
      this.label13.Name = "label13";
      this.label13.Size = new Size(17, 25);
      this.label13.TabIndex = 29;
      this.label13.Text = "t";
      this.label14.AutoSize = true;
      this.label14.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label14.Location = new Point(940, 336);
      this.label14.Name = "label14";
      this.label14.Size = new Size(22, 25);
      this.label14.TabIndex = 30;
      this.label14.Text = "x";
      this.label15.AutoSize = true;
      this.label15.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.label15.Location = new Point(1328, 596);
      this.label15.Name = "label15";
      this.label15.Size = new Size(17, 25);
      this.label15.TabIndex = 31;
      this.label15.Text = "t";
      this.AutoScaleDimensions = new SizeF(8f, 16f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.ClientSize = new Size(1406, 644);
      this.Controls.Add((Control) this.label15);
      this.Controls.Add((Control) this.label14);
      this.Controls.Add((Control) this.label13);
      this.Controls.Add((Control) this.label12);
      this.Controls.Add((Control) this.label11);
      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.label9);
      this.Controls.Add((Control) this.label8);
      this.Controls.Add((Control) this.PictureBox4);
      this.Controls.Add((Control) this.PictureBox3);
      this.Controls.Add((Control) this.PictureBox2);
      this.Controls.Add((Control) this.PictureBox1);
      this.Controls.Add((Control) this.Button_runCalc);
      this.Controls.Add((Control) this.groupBox4);
      this.Controls.Add((Control) this.groupBox3);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Name = "PortalCraneModel";
      this.SizeGripStyle = SizeGripStyle.Show;
      this.Text = "PortalCraneModel";
      this.Load += new EventHandler(this.PortalCraneModel_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((ISupportInitialize) this.PictureBox1).EndInit();
      ((ISupportInitialize) this.PictureBox2).EndInit();
      ((ISupportInitialize) this.PictureBox3).EndInit();
      ((ISupportInitialize) this.PictureBox4).EndInit();
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
  }
}
