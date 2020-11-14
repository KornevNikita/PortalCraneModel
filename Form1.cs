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

    public int buildCount = 0, dinPointsCount;
    public static int drawStCount;

    const string dll = "PortalCraneCalc.dll";

    public static double[] DrawPoints, DrawCriteria;


    public static IntPtr ptrTAllDrawPoints, PtrNextPoint, ptrCriteria;

    public static PortalCraneModel.TAllDrawPoints allPoints, criteria;

    public PortalCraneModel.TStatePoint CurrentPoint;

    public static double M, m, l, R, g,
      h_fi, h_x, B, gamma, E, // parametri modeli

      fi, dfi_dt, x, dx_dt, // ishodnoye sostoyanie sistemi

      dt, t_start, t_stop, // parametri integrirovaniya

      lambda1_re, lambda1_im, lambda2_re, lambda2_im,
      lambda3_re, lambda3_im, lambda4_re, lambda4_im; // zhelaemiye korni har. polinoma

    public static bool is_calc_criteria = false;

    private TabControl tabControl1;
    private TabPage tabPage1;
    private GroupBox groupBox3;
    private Label textBox_run_time;
    private Label label16;
    private Button Button_setCalcParam;
    private Label label4;
    private Label label3;
    private Label label2;
    private Label label1;

    private static TextBox textBox_drawStCount, textBox_t_stop,
      textBox_t_start, textBox_step;

    private GroupBox groupBox1;
    private Button Button_setParam;
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

    private static TextBox textBox_h_fi, textBox_g, textBox_B, textBox_R, textBox_gamma,
    textBox_l, textBox_h_x, textBox_mmal ,textBox_M, textBox_E;

    private GroupBox roots_gbox;
    private Button button2;
    private Button btn_clear;
    private Label label11;
    private Label label10;

    private static TextBox tbox_lambda1_im, tbox_lambda1_re,
      tbox_lambda3_im, tbox_lambda3_re,
      tbox_lambda2_im, tbox_lambda2_re,
      tbox_lambda4_im, tbox_lambda4_re;

    private Button Button_runCalc;

    private Label label9;
    private Label label8;
    private Label label18;
    private Label root1;

    private Chart chart2;
    private Chart chart4;
    private Chart chart3;
    private Chart chart1;
    public static CheckBox cBox_non_linear, cBox_Reg_on;
    private DataGridView dataGridView1;
    private GroupBox groupBox2;
    private Button Button_setInitVal;
    private Label labelfi;
    private Label labeldfi_dt;
    private Label labelx;
    private Label labeldx_dt;
    private static TextBox textBox_fi, textBox_dx_dt,
      textBox_x, textBox_dfi_dt;
    private TabPage tabPage2;
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
    private CheckBox drawing_on;
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
    public static extern void SetCalcParams(double _dt, double _t_start, double _t_stop, int _drawStCount);

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

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Calc_criteria();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Calc_criteria_eque_lines(bool system);

    public PortalCraneModel()
    {
      InitializeComponent();
      allPoints = new PortalCraneModel.TAllDrawPoints();
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

    private void Button_setParam_Click(object sender, EventArgs e)
    {
      SetParam();
    }

    private void Button_setInitVal_Click(object sender, EventArgs e)
    {
      SetInitVal();
    }

    private void Button_setCalcParam_Click(object sender, EventArgs e)
    {
      SetCalcParam();
    }

    private void Button_runCalc_Click(object sender, EventArgs e)
    {
      dataGridView1.Rows.Clear();

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      SetLambdas(); // zhelaemie korni har. polinoma
      SetParam(); // параметры модели
      SetCalcParam(); // параметры расчета
      SetInitVal(); // начальное состояние системы

      Calculate();

      stopwatch.Stop();

      if (drawing_on.Checked)
        DrawTrajectories();

      PrintGrid();

      textBox_run_time.Text = stopwatch.Elapsed.TotalSeconds.ToString();
      textBox_run_time.BackColor = Color.LightGreen;
    }

    public static void Calculate()
    {
      Calc_regulator(); // rasschitivaem parametri regulatora

      // определяем количество точек, которые будут отрисованы
      allPoints.drawCount = GetAllDrawPointsCount();
      // создаем управляемое хранилище
      DrawPoints = new double[allPoints.drawCount * 5];

      int sizeStruct = Marshal.SizeOf(typeof(PortalCraneModel.TAllDrawPoints)); // определяем размер управляемой структуры
      ptrTAllDrawPoints = Marshal.AllocHGlobal(sizeStruct); // выделяем память под неуправляемую структуру
      Marshal.StructureToPtr(allPoints, ptrTAllDrawPoints, false); // копируем данные из неуправляемой в управляемую
      PortalCraneModel.InitAllPointsArray(ptrTAllDrawPoints); // выделяем память под внутренний неуправляемый массив в неупр структуре
      PortalCraneModel.GetAllDrawPoints(ptrTAllDrawPoints, cBox_non_linear.Checked, cBox_Reg_on.Checked);
      allPoints = (PortalCraneModel.TAllDrawPoints)Marshal.PtrToStructure(ptrTAllDrawPoints, typeof(PortalCraneModel.TAllDrawPoints));
      Marshal.Copy(allPoints.allDrawPoints, DrawPoints, 0, allPoints.drawCount * 5);
      PortalCraneModel.DeleteAllPointsArray(ptrTAllDrawPoints);
      Marshal.FreeHGlobal(ptrTAllDrawPoints);
    }

    private void DrawTrajectories()
    {
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

      chart3.ChartAreas[0].AxisX.Minimum = t_start;
      chart4.ChartAreas[0].AxisX.Minimum = t_start;

      chart1.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);
      chart2.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);
      chart3.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);
      chart4.Series[buildCount.ToString()].Color = Color.FromArgb(rand.Next() % 256, rand.Next() % 256, rand.Next() % 256);

      for (int i = 0; i < DrawPoints.Length; i += 5)
      {
        chart1.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i], DrawPoints[i + 1]);
        chart2.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i + 2], DrawPoints[i + 3]);
        chart3.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i + 4], DrawPoints[i]);
        chart4.Series[buildCount.ToString()].Points.AddXY(DrawPoints[i + 4], DrawPoints[i + 2]);
      }
      buildCount++;
    }

    private void PrintGrid()
    {
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
    }

    public static void SetParam()
    {
      M = double.Parse(textBox_M.Text);
      m = double.Parse(textBox_mmal.Text);
      l = double.Parse(textBox_l.Text);
      R = double.Parse(textBox_R.Text);
      g = double.Parse(textBox_g.Text);
      h_fi = double.Parse(textBox_h_fi.Text);
      h_x = double.Parse(textBox_h_x.Text);
      B = double.Parse(textBox_B.Text);
      gamma = double.Parse(textBox_gamma.Text);
      E = double.Parse(textBox_E.Text);

      PortalCraneModel.SetModelParams(M, m, l, R, g,
          h_fi, h_x, B, gamma, E);
    }

    public static void SetLambdas()
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

    public static void SetInitVal()
    {
      fi = double.Parse(textBox_fi.Text);
      dfi_dt = double.Parse(textBox_dfi_dt.Text);
      x = double.Parse(textBox_x.Text);
      dx_dt = double.Parse(textBox_dx_dt.Text);
      SetInitParams(fi, dfi_dt, x, dx_dt);
    }

    public static void SetCalcParam()
    {
      dt = double.Parse(textBox_step.Text);
      t_start = double.Parse(textBox_t_start.Text);
      t_stop = double.Parse(textBox_t_stop.Text);
      drawStCount = int.Parse(textBox_drawStCount.Text);
     SetCalcParams(dt, t_start, t_stop, drawStCount);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && components != null)
        components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
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
      tabControl1 = new System.Windows.Forms.TabControl();
      tabPage1 = new System.Windows.Forms.TabPage();
      chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      groupBox3 = new System.Windows.Forms.GroupBox();
      textBox_run_time = new System.Windows.Forms.Label();
      label16 = new System.Windows.Forms.Label();
      Button_setCalcParam = new System.Windows.Forms.Button();
      label4 = new System.Windows.Forms.Label();
      label3 = new System.Windows.Forms.Label();
      label2 = new System.Windows.Forms.Label();
      label1 = new System.Windows.Forms.Label();
      textBox_drawStCount = new System.Windows.Forms.TextBox();
      textBox_t_stop = new System.Windows.Forms.TextBox();
      textBox_t_start = new System.Windows.Forms.TextBox();
      textBox_step = new System.Windows.Forms.TextBox();
      groupBox1 = new System.Windows.Forms.GroupBox();
      Button_setParam = new System.Windows.Forms.Button();
      textBox_E = new System.Windows.Forms.TextBox();
      textBox_gamma = new System.Windows.Forms.TextBox();
      label_gamma = new System.Windows.Forms.Label();
      label_E = new System.Windows.Forms.Label();
      label_h_x = new System.Windows.Forms.Label();
      label_B = new System.Windows.Forms.Label();
      label7 = new System.Windows.Forms.Label();
      label_h_fi = new System.Windows.Forms.Label();
      label_g = new System.Windows.Forms.Label();
      label_R = new System.Windows.Forms.Label();
      label_l = new System.Windows.Forms.Label();
      label_mmal = new System.Windows.Forms.Label();
      label_M = new System.Windows.Forms.Label();
      textBox_h_fi = new System.Windows.Forms.TextBox();
      textBox_g = new System.Windows.Forms.TextBox();
      textBox_B = new System.Windows.Forms.TextBox();
      textBox_R = new System.Windows.Forms.TextBox();
      textBox_l = new System.Windows.Forms.TextBox();
      textBox_h_x = new System.Windows.Forms.TextBox();
      textBox_mmal = new System.Windows.Forms.TextBox();
      textBox_M = new System.Windows.Forms.TextBox();
      roots_gbox = new System.Windows.Forms.GroupBox();
      button3 = new System.Windows.Forms.Button();
      button2 = new System.Windows.Forms.Button();
      btn_clear = new System.Windows.Forms.Button();
      label11 = new System.Windows.Forms.Label();
      label10 = new System.Windows.Forms.Label();
      tbox_lambda4_im = new System.Windows.Forms.TextBox();
      tbox_lambda4_re = new System.Windows.Forms.TextBox();
      Button_runCalc = new System.Windows.Forms.Button();
      tbox_lambda2_im = new System.Windows.Forms.TextBox();
      tbox_lambda2_re = new System.Windows.Forms.TextBox();
      label9 = new System.Windows.Forms.Label();
      label8 = new System.Windows.Forms.Label();
      label18 = new System.Windows.Forms.Label();
      root1 = new System.Windows.Forms.Label();
      tbox_lambda3_im = new System.Windows.Forms.TextBox();
      tbox_lambda1_re = new System.Windows.Forms.TextBox();
      tbox_lambda3_re = new System.Windows.Forms.TextBox();
      tbox_lambda1_im = new System.Windows.Forms.TextBox();
      cBox_non_linear = new System.Windows.Forms.CheckBox();
      cBox_Reg_on = new System.Windows.Forms.CheckBox();
      dataGridView1 = new System.Windows.Forms.DataGridView();
      groupBox2 = new System.Windows.Forms.GroupBox();
      Button_setInitVal = new System.Windows.Forms.Button();
      labelfi = new System.Windows.Forms.Label();
      labeldfi_dt = new System.Windows.Forms.Label();
      labelx = new System.Windows.Forms.Label();
      labeldx_dt = new System.Windows.Forms.Label();
      textBox_fi = new System.Windows.Forms.TextBox();
      textBox_dx_dt = new System.Windows.Forms.TextBox();
      textBox_x = new System.Windows.Forms.TextBox();
      textBox_dfi_dt = new System.Windows.Forms.TextBox();
      tabPage2 = new System.Windows.Forms.TabPage();
      label5 = new System.Windows.Forms.Label();
      groupBox4 = new System.Windows.Forms.GroupBox();
      label_mu1 = new System.Windows.Forms.Label();
      xmin_t = new System.Windows.Forms.TextBox();
      label_M3 = new System.Windows.Forms.Label();
      xmax_t = new System.Windows.Forms.TextBox();
      label_M2 = new System.Windows.Forms.Label();
      label_mu2 = new System.Windows.Forms.Label();
      label_M1 = new System.Windows.Forms.Label();
      button1 = new System.Windows.Forms.Button();
      ymin_t = new System.Windows.Forms.TextBox();
      label_N = new System.Windows.Forms.Label();
      label_sigma = new System.Windows.Forms.Label();
      DL_N = new System.Windows.Forms.TextBox();
      DL_M2 = new System.Windows.Forms.TextBox();
      DL_M1 = new System.Windows.Forms.TextBox();
      DL_M3 = new System.Windows.Forms.TextBox();
      pBox_Vmax_criterion = new System.Windows.Forms.PictureBox();
      pBox_h2_criterion = new System.Windows.Forms.PictureBox();
      pBox_h1_criterion = new System.Windows.Forms.PictureBox();
      pBox_H_criterion = new System.Windows.Forms.PictureBox();
      pBox_T_criterion = new System.Windows.Forms.PictureBox();
      func_num_text = new System.Windows.Forms.TextBox();
      ymax_t = new System.Windows.Forms.TextBox();
      drawing_on = new System.Windows.Forms.CheckBox();
      tabControl1.SuspendLayout();
      tabPage1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(chart2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(chart4)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(chart3)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(chart1)).BeginInit();
      groupBox3.SuspendLayout();
      groupBox1.SuspendLayout();
      roots_gbox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(dataGridView1)).BeginInit();
      groupBox2.SuspendLayout();
      tabPage2.SuspendLayout();
      groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(pBox_Vmax_criterion)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_h2_criterion)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_h1_criterion)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_H_criterion)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_T_criterion)).BeginInit();
      SuspendLayout();
      // 
      // tabControl1
      // 
      tabControl1.Controls.Add(tabPage1);
      tabControl1.Controls.Add(tabPage2);
      tabControl1.Location = new System.Drawing.Point(0, 0);
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new System.Drawing.Size(1364, 767);
      tabControl1.TabIndex = 0;
      // 
      // tabPage1
      // 
      tabPage1.Controls.Add(chart2);
      tabPage1.Controls.Add(chart4);
      tabPage1.Controls.Add(chart3);
      tabPage1.Controls.Add(chart1);
      tabPage1.Controls.Add(groupBox3);
      tabPage1.Controls.Add(groupBox1);
      tabPage1.Controls.Add(roots_gbox);
      tabPage1.Controls.Add(cBox_non_linear);
      tabPage1.Controls.Add(cBox_Reg_on);
      tabPage1.Controls.Add(dataGridView1);
      tabPage1.Controls.Add(groupBox2);
      tabPage1.Location = new System.Drawing.Point(4, 22);
      tabPage1.Name = "tabPage1";
      tabPage1.Padding = new System.Windows.Forms.Padding(3);
      tabPage1.Size = new System.Drawing.Size(1356, 741);
      tabPage1.TabIndex = 0;
      tabPage1.Text = "Модель";
      tabPage1.UseVisualStyleBackColor = true;
      // 
      // chart2
      // 
      chart2.BackColor = System.Drawing.SystemColors.Control;
      chartArea1.Name = "ChartArea1";
      chart2.ChartAreas.Add(chartArea1);
      legend1.Enabled = false;
      legend1.Name = "Legend1";
      chart2.Legends.Add(legend1);
      chart2.Location = new System.Drawing.Point(849, 8);
      chart2.Name = "chart2";
      series1.ChartArea = "ChartArea1";
      series1.Legend = "Legend1";
      series1.Name = "Series1";
      chart2.Series.Add(series1);
      chart2.Size = new System.Drawing.Size(500, 350);
      chart2.TabIndex = 55;
      chart2.Text = "chart2";
      // 
      // chart4
      // 
      chart4.BackColor = System.Drawing.SystemColors.Control;
      chartArea2.Name = "ChartArea1";
      chart4.ChartAreas.Add(chartArea2);
      legend2.Enabled = false;
      legend2.Name = "Legend1";
      chart4.Legends.Add(legend2);
      chart4.Location = new System.Drawing.Point(849, 364);
      chart4.Name = "chart4";
      series2.ChartArea = "ChartArea1";
      series2.Legend = "Legend1";
      series2.Name = "Series1";
      series2.XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
      chart4.Series.Add(series2);
      chart4.Size = new System.Drawing.Size(500, 350);
      chart4.TabIndex = 52;
      chart4.Text = "chart1";
      // 
      // chart3
      // 
      chart3.BackColor = System.Drawing.SystemColors.Control;
      chartArea3.Name = "ChartArea1";
      chart3.ChartAreas.Add(chartArea3);
      legend3.Enabled = false;
      legend3.Name = "Legend1";
      chart3.Legends.Add(legend3);
      chart3.Location = new System.Drawing.Point(343, 364);
      chart3.Name = "chart3";
      series3.ChartArea = "ChartArea1";
      series3.Legend = "Legend1";
      series3.Name = "Series1";
      chart3.Series.Add(series3);
      chart3.Size = new System.Drawing.Size(500, 350);
      chart3.TabIndex = 53;
      chart3.Text = "chart1";
      // 
      // chart1
      // 
      chart1.BackColor = System.Drawing.SystemColors.Control;
      chartArea4.Name = "ChartArea1";
      chart1.ChartAreas.Add(chartArea4);
      legend4.Enabled = false;
      legend4.Name = "Legend1";
      chart1.Legends.Add(legend4);
      chart1.Location = new System.Drawing.Point(343, 8);
      chart1.Name = "chart1";
      series4.ChartArea = "ChartArea1";
      series4.Legend = "Legend1";
      series4.Name = "Series1";
      chart1.Series.Add(series4);
      chart1.Size = new System.Drawing.Size(500, 350);
      chart1.TabIndex = 54;
      chart1.Text = "chart1";
      // 
      // groupBox3
      // 
      groupBox3.BackColor = System.Drawing.SystemColors.Control;
      groupBox3.Controls.Add(drawing_on);
      groupBox3.Controls.Add(textBox_run_time);
      groupBox3.Controls.Add(label16);
      groupBox3.Controls.Add(Button_setCalcParam);
      groupBox3.Controls.Add(label4);
      groupBox3.Controls.Add(label3);
      groupBox3.Controls.Add(label2);
      groupBox3.Controls.Add(label1);
      groupBox3.Controls.Add(textBox_drawStCount);
      groupBox3.Controls.Add(textBox_t_stop);
      groupBox3.Controls.Add(textBox_t_start);
      groupBox3.Controls.Add(textBox_step);
      groupBox3.Location = new System.Drawing.Point(147, 8);
      groupBox3.Margin = new System.Windows.Forms.Padding(2);
      groupBox3.Name = "groupBox3";
      groupBox3.Padding = new System.Windows.Forms.Padding(2);
      groupBox3.Size = new System.Drawing.Size(191, 171);
      groupBox3.TabIndex = 50;
      groupBox3.TabStop = false;
      groupBox3.Text = "Параметры расчета";
      // 
      // textBox_run_time
      // 
      textBox_run_time.AutoSize = true;
      textBox_run_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      textBox_run_time.Location = new System.Drawing.Point(112, 147);
      textBox_run_time.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      textBox_run_time.Name = "textBox_run_time";
      textBox_run_time.Size = new System.Drawing.Size(0, 16);
      textBox_run_time.TabIndex = 11;
      // 
      // label16
      // 
      label16.AutoSize = true;
      label16.Location = new System.Drawing.Point(0, 149);
      label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label16.Name = "label16";
      label16.Size = new System.Drawing.Size(108, 13);
      label16.TabIndex = 10;
      label16.Text = "Время выполнения:";
      // 
      // Button_setCalcParam
      // 
      Button_setCalcParam.Location = new System.Drawing.Point(112, 109);
      Button_setCalcParam.Margin = new System.Windows.Forms.Padding(2);
      Button_setCalcParam.Name = "Button_setCalcParam";
      Button_setCalcParam.Size = new System.Drawing.Size(75, 25);
      Button_setCalcParam.TabIndex = 9;
      Button_setCalcParam.Text = "Принять";
      Button_setCalcParam.UseVisualStyleBackColor = true;
      Button_setCalcParam.Click += new System.EventHandler(Button_setCalcParam_Click);
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new System.Drawing.Point(4, 88);
      label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label4.Name = "label4";
      label4.Size = new System.Drawing.Size(99, 13);
      label4.TabIndex = 7;
      label4.Text = "Шагов инт. в шаге";
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new System.Drawing.Point(4, 67);
      label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label3.Name = "label3";
      label3.Size = new System.Drawing.Size(39, 13);
      label3.TabIndex = 6;
      label3.Text = "t_stop:";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new System.Drawing.Point(4, 44);
      label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label2.Name = "label2";
      label2.Size = new System.Drawing.Size(39, 13);
      label2.TabIndex = 5;
      label2.Text = "t_start:";
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(4, 20);
      label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(53, 13);
      label1.TabIndex = 4;
      label1.Text = "Шаг инт.:";
      // 
      // textBox_drawStCount
      // 
      textBox_drawStCount.Location = new System.Drawing.Point(112, 85);
      textBox_drawStCount.Margin = new System.Windows.Forms.Padding(2);
      textBox_drawStCount.Name = "textBox_drawStCount";
      textBox_drawStCount.Size = new System.Drawing.Size(76, 20);
      textBox_drawStCount.TabIndex = 3;
      textBox_drawStCount.Text = "10";
      textBox_drawStCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox_t_stop
      // 
      textBox_t_stop.Location = new System.Drawing.Point(112, 63);
      textBox_t_stop.Margin = new System.Windows.Forms.Padding(2);
      textBox_t_stop.Name = "textBox_t_stop";
      textBox_t_stop.Size = new System.Drawing.Size(76, 20);
      textBox_t_stop.TabIndex = 2;
      textBox_t_stop.Text = "100";
      textBox_t_stop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox_t_start
      // 
      textBox_t_start.Location = new System.Drawing.Point(112, 40);
      textBox_t_start.Margin = new System.Windows.Forms.Padding(2);
      textBox_t_start.Name = "textBox_t_start";
      textBox_t_start.Size = new System.Drawing.Size(76, 20);
      textBox_t_start.TabIndex = 1;
      textBox_t_start.Text = "0";
      textBox_t_start.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox_step
      // 
      textBox_step.Location = new System.Drawing.Point(112, 17);
      textBox_step.Margin = new System.Windows.Forms.Padding(2);
      textBox_step.Name = "textBox_step";
      textBox_step.Size = new System.Drawing.Size(76, 20);
      textBox_step.TabIndex = 0;
      textBox_step.Text = "0,005";
      textBox_step.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // groupBox1
      // 
      groupBox1.BackColor = System.Drawing.SystemColors.Control;
      groupBox1.Controls.Add(Button_setParam);
      groupBox1.Controls.Add(textBox_E);
      groupBox1.Controls.Add(textBox_gamma);
      groupBox1.Controls.Add(label_gamma);
      groupBox1.Controls.Add(label_E);
      groupBox1.Controls.Add(label_h_x);
      groupBox1.Controls.Add(label_B);
      groupBox1.Controls.Add(label7);
      groupBox1.Controls.Add(label_h_fi);
      groupBox1.Controls.Add(label_g);
      groupBox1.Controls.Add(label_R);
      groupBox1.Controls.Add(label_l);
      groupBox1.Controls.Add(label_mmal);
      groupBox1.Controls.Add(label_M);
      groupBox1.Controls.Add(textBox_h_fi);
      groupBox1.Controls.Add(textBox_g);
      groupBox1.Controls.Add(textBox_B);
      groupBox1.Controls.Add(textBox_R);
      groupBox1.Controls.Add(textBox_l);
      groupBox1.Controls.Add(textBox_h_x);
      groupBox1.Controls.Add(textBox_mmal);
      groupBox1.Controls.Add(textBox_M);
      groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      groupBox1.Location = new System.Drawing.Point(8, 8);
      groupBox1.Margin = new System.Windows.Forms.Padding(2);
      groupBox1.Name = "groupBox1";
      groupBox1.Padding = new System.Windows.Forms.Padding(2);
      groupBox1.Size = new System.Drawing.Size(135, 270);
      groupBox1.TabIndex = 48;
      groupBox1.TabStop = false;
      groupBox1.Text = "Параметры модели";
      // 
      // Button_setParam
      // 
      Button_setParam.Location = new System.Drawing.Point(55, 245);
      Button_setParam.Margin = new System.Windows.Forms.Padding(2);
      Button_setParam.Name = "Button_setParam";
      Button_setParam.Size = new System.Drawing.Size(75, 25);
      Button_setParam.TabIndex = 27;
      Button_setParam.Text = "Принять";
      Button_setParam.UseVisualStyleBackColor = true;
      Button_setParam.Click += new System.EventHandler(Button_setParam_Click);
      // 
      // textBox_E
      // 
      textBox_E.Location = new System.Drawing.Point(55, 222);
      textBox_E.Margin = new System.Windows.Forms.Padding(2);
      textBox_E.Name = "textBox_E";
      textBox_E.Size = new System.Drawing.Size(76, 20);
      textBox_E.TabIndex = 26;
      textBox_E.Text = "0,00767";
      // 
      // textBox_gamma
      // 
      textBox_gamma.Location = new System.Drawing.Point(55, 199);
      textBox_gamma.Margin = new System.Windows.Forms.Padding(2);
      textBox_gamma.Name = "textBox_gamma";
      textBox_gamma.Size = new System.Drawing.Size(76, 20);
      textBox_gamma.TabIndex = 25;
      textBox_gamma.Text = "4,481";
      // 
      // label_gamma
      // 
      label_gamma.AutoSize = true;
      label_gamma.Location = new System.Drawing.Point(4, 202);
      label_gamma.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_gamma.Name = "label_gamma";
      label_gamma.Size = new System.Drawing.Size(44, 13);
      label_gamma.TabIndex = 24;
      label_gamma.Text = "gamma:";
      // 
      // label_E
      // 
      label_E.AutoSize = true;
      label_E.Location = new System.Drawing.Point(4, 226);
      label_E.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_E.Name = "label_E";
      label_E.Size = new System.Drawing.Size(17, 13);
      label_E.TabIndex = 23;
      label_E.Text = "E:";
      // 
      // label_h_x
      // 
      label_h_x.AutoSize = true;
      label_h_x.Location = new System.Drawing.Point(4, 158);
      label_h_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_h_x.Name = "label_h_x";
      label_h_x.Size = new System.Drawing.Size(27, 13);
      label_h_x.TabIndex = 21;
      label_h_x.Text = "h_x:";
      // 
      // label_B
      // 
      label_B.AutoSize = true;
      label_B.Location = new System.Drawing.Point(4, 180);
      label_B.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_B.Name = "label_B";
      label_B.Size = new System.Drawing.Size(17, 13);
      label_B.TabIndex = 20;
      label_B.Text = "B:";
      // 
      // label7
      // 
      label7.AutoSize = true;
      label7.Location = new System.Drawing.Point(16, 158);
      label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label7.Name = "label7";
      label7.Size = new System.Drawing.Size(0, 13);
      label7.TabIndex = 19;
      // 
      // label_h_fi
      // 
      label_h_fi.AutoSize = true;
      label_h_fi.Location = new System.Drawing.Point(4, 135);
      label_h_fi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_h_fi.Name = "label_h_fi";
      label_h_fi.Size = new System.Drawing.Size(27, 13);
      label_h_fi.TabIndex = 18;
      label_h_fi.Text = "h_fi:";
      // 
      // label_g
      // 
      label_g.AutoSize = true;
      label_g.Location = new System.Drawing.Point(4, 112);
      label_g.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_g.Name = "label_g";
      label_g.Size = new System.Drawing.Size(16, 13);
      label_g.TabIndex = 17;
      label_g.Text = "g:";
      // 
      // label_R
      // 
      label_R.AutoSize = true;
      label_R.Location = new System.Drawing.Point(4, 89);
      label_R.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_R.Name = "label_R";
      label_R.Size = new System.Drawing.Size(18, 13);
      label_R.TabIndex = 16;
      label_R.Text = "R:";
      // 
      // label_l
      // 
      label_l.AutoSize = true;
      label_l.Location = new System.Drawing.Point(4, 67);
      label_l.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_l.Name = "label_l";
      label_l.Size = new System.Drawing.Size(12, 13);
      label_l.TabIndex = 15;
      label_l.Text = "l:";
      // 
      // label_mmal
      // 
      label_mmal.AutoSize = true;
      label_mmal.Location = new System.Drawing.Point(4, 44);
      label_mmal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_mmal.Name = "label_mmal";
      label_mmal.Size = new System.Drawing.Size(18, 13);
      label_mmal.TabIndex = 14;
      label_mmal.Text = "m:";
      // 
      // label_M
      // 
      label_M.AutoSize = true;
      label_M.Location = new System.Drawing.Point(4, 21);
      label_M.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      label_M.Name = "label_M";
      label_M.Size = new System.Drawing.Size(19, 13);
      label_M.TabIndex = 13;
      label_M.Text = "M:";
      // 
      // textBox_h_fi
      // 
      textBox_h_fi.Location = new System.Drawing.Point(55, 131);
      textBox_h_fi.Margin = new System.Windows.Forms.Padding(2);
      textBox_h_fi.Name = "textBox_h_fi";
      textBox_h_fi.Size = new System.Drawing.Size(76, 20);
      textBox_h_fi.TabIndex = 7;
      textBox_h_fi.Text = "0,0024";
      // 
      // textBox_g
      // 
      textBox_g.Location = new System.Drawing.Point(55, 108);
      textBox_g.Margin = new System.Windows.Forms.Padding(2);
      textBox_g.Name = "textBox_g";
      textBox_g.Size = new System.Drawing.Size(76, 20);
      textBox_g.TabIndex = 3;
      textBox_g.Text = "9,81";
      // 
      // textBox_B
      // 
      textBox_B.Location = new System.Drawing.Point(55, 176);
      textBox_B.Margin = new System.Windows.Forms.Padding(2);
      textBox_B.Name = "textBox_B";
      textBox_B.Size = new System.Drawing.Size(76, 20);
      textBox_B.TabIndex = 6;
      textBox_B.Text = "0,024";
      // 
      // textBox_R
      // 
      textBox_R.Location = new System.Drawing.Point(55, 85);
      textBox_R.Margin = new System.Windows.Forms.Padding(2);
      textBox_R.Name = "textBox_R";
      textBox_R.Size = new System.Drawing.Size(76, 20);
      textBox_R.TabIndex = 2;
      textBox_R.Text = "2,6";
      // 
      // textBox_l
      // 
      textBox_l.Location = new System.Drawing.Point(55, 63);
      textBox_l.Margin = new System.Windows.Forms.Padding(2);
      textBox_l.Name = "textBox_l";
      textBox_l.Size = new System.Drawing.Size(76, 20);
      textBox_l.TabIndex = 5;
      textBox_l.Text = "0,641";
      // 
      // textBox_h_x
      // 
      textBox_h_x.Location = new System.Drawing.Point(55, 154);
      textBox_h_x.Margin = new System.Windows.Forms.Padding(2);
      textBox_h_x.Name = "textBox_h_x";
      textBox_h_x.Size = new System.Drawing.Size(76, 20);
      textBox_h_x.TabIndex = 4;
      textBox_h_x.Text = "5,4";
      // 
      // textBox_mmal
      // 
      textBox_mmal.Location = new System.Drawing.Point(55, 40);
      textBox_mmal.Margin = new System.Windows.Forms.Padding(2);
      textBox_mmal.Name = "textBox_mmal";
      textBox_mmal.Size = new System.Drawing.Size(76, 20);
      textBox_mmal.TabIndex = 1;
      textBox_mmal.Text = "0,019";
      // 
      // textBox_M
      // 
      textBox_M.Location = new System.Drawing.Point(55, 17);
      textBox_M.Margin = new System.Windows.Forms.Padding(2);
      textBox_M.Name = "textBox_M";
      textBox_M.Size = new System.Drawing.Size(76, 20);
      textBox_M.TabIndex = 0;
      textBox_M.Text = "1,073";
      // 
      // roots_gbox
      // 
      roots_gbox.BackColor = System.Drawing.SystemColors.Control;
      roots_gbox.Controls.Add(button3);
      roots_gbox.Controls.Add(button2);
      roots_gbox.Controls.Add(btn_clear);
      roots_gbox.Controls.Add(label11);
      roots_gbox.Controls.Add(label10);
      roots_gbox.Controls.Add(tbox_lambda4_im);
      roots_gbox.Controls.Add(tbox_lambda4_re);
      roots_gbox.Controls.Add(Button_runCalc);
      roots_gbox.Controls.Add(tbox_lambda2_im);
      roots_gbox.Controls.Add(tbox_lambda2_re);
      roots_gbox.Controls.Add(label9);
      roots_gbox.Controls.Add(label8);
      roots_gbox.Controls.Add(label18);
      roots_gbox.Controls.Add(root1);
      roots_gbox.Controls.Add(tbox_lambda3_im);
      roots_gbox.Controls.Add(tbox_lambda1_re);
      roots_gbox.Controls.Add(tbox_lambda3_re);
      roots_gbox.Controls.Add(tbox_lambda1_im);
      roots_gbox.Location = new System.Drawing.Point(148, 184);
      roots_gbox.Name = "roots_gbox";
      roots_gbox.Size = new System.Drawing.Size(191, 232);
      roots_gbox.TabIndex = 51;
      roots_gbox.TabStop = false;
      roots_gbox.Text = "Желаемые корни хар. полинома";
      // 
      // button3
      // 
      button3.Location = new System.Drawing.Point(17, 146);
      button3.Name = "button3";
      button3.Size = new System.Drawing.Size(80, 25);
      button3.TabIndex = 46;
      button3.Text = "Сл. корни";
      button3.UseVisualStyleBackColor = true;
      button3.Click += new System.EventHandler(Button3_Click);
      // 
      // button2
      // 
      button2.Location = new System.Drawing.Point(104, 146);
      button2.Name = "button2";
      button2.Size = new System.Drawing.Size(80, 25);
      button2.TabIndex = 45;
      button2.Text = "Сбросить";
      button2.UseVisualStyleBackColor = true;
      button2.Click += new System.EventHandler(Button2_Click);
      // 
      // btn_clear
      // 
      btn_clear.Location = new System.Drawing.Point(0, 204);
      btn_clear.Name = "btn_clear";
      btn_clear.Size = new System.Drawing.Size(188, 25);
      btn_clear.TabIndex = 35;
      btn_clear.Text = "Очистить";
      btn_clear.UseVisualStyleBackColor = true;
      btn_clear.Click += new System.EventHandler(Btn_clear_Click);
      // 
      // label11
      // 
      label11.AutoSize = true;
      label11.Location = new System.Drawing.Point(6, 123);
      label11.Name = "label11";
      label11.Size = new System.Drawing.Size(56, 13);
      label11.TabIndex = 44;
      label11.Text = "lambda_4:";
      // 
      // label10
      // 
      label10.AutoSize = true;
      label10.Location = new System.Drawing.Point(6, 71);
      label10.Name = "label10";
      label10.Size = new System.Drawing.Size(56, 13);
      label10.TabIndex = 43;
      label10.Text = "lambda_2:";
      // 
      // tbox_lambda4_im
      // 
      tbox_lambda4_im.Location = new System.Drawing.Point(133, 120);
      tbox_lambda4_im.Name = "tbox_lambda4_im";
      tbox_lambda4_im.Size = new System.Drawing.Size(31, 20);
      tbox_lambda4_im.TabIndex = 42;
      tbox_lambda4_im.Text = "0";
      tbox_lambda4_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // tbox_lambda4_re
      // 
      tbox_lambda4_re.Location = new System.Drawing.Point(83, 120);
      tbox_lambda4_re.Name = "tbox_lambda4_re";
      tbox_lambda4_re.Size = new System.Drawing.Size(31, 20);
      tbox_lambda4_re.TabIndex = 41;
      tbox_lambda4_re.Text = "0";
      tbox_lambda4_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // Button_runCalc
      // 
      Button_runCalc.Location = new System.Drawing.Point(0, 174);
      Button_runCalc.Margin = new System.Windows.Forms.Padding(2);
      Button_runCalc.Name = "Button_runCalc";
      Button_runCalc.Size = new System.Drawing.Size(188, 25);
      Button_runCalc.TabIndex = 19;
      Button_runCalc.Text = "Рассчитать";
      Button_runCalc.UseVisualStyleBackColor = true;
      Button_runCalc.Click += new System.EventHandler(Button_runCalc_Click);
      // 
      // tbox_lambda2_im
      // 
      tbox_lambda2_im.Location = new System.Drawing.Point(133, 68);
      tbox_lambda2_im.Name = "tbox_lambda2_im";
      tbox_lambda2_im.Size = new System.Drawing.Size(31, 20);
      tbox_lambda2_im.TabIndex = 40;
      tbox_lambda2_im.Text = "0";
      tbox_lambda2_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // tbox_lambda2_re
      // 
      tbox_lambda2_re.Location = new System.Drawing.Point(83, 68);
      tbox_lambda2_re.Name = "tbox_lambda2_re";
      tbox_lambda2_re.Size = new System.Drawing.Size(31, 20);
      tbox_lambda2_re.TabIndex = 39;
      tbox_lambda2_re.Text = "0";
      tbox_lambda2_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // label9
      // 
      label9.AutoSize = true;
      label9.Location = new System.Drawing.Point(130, 22);
      label9.Name = "label9";
      label9.Size = new System.Drawing.Size(21, 13);
      label9.TabIndex = 38;
      label9.Text = "Im:";
      // 
      // label8
      // 
      label8.AutoSize = true;
      label8.Location = new System.Drawing.Point(80, 22);
      label8.Name = "label8";
      label8.Size = new System.Drawing.Size(24, 13);
      label8.TabIndex = 37;
      label8.Text = "Re:";
      // 
      // label18
      // 
      label18.AutoSize = true;
      label18.Location = new System.Drawing.Point(6, 97);
      label18.Name = "label18";
      label18.Size = new System.Drawing.Size(56, 13);
      label18.TabIndex = 35;
      label18.Text = "lambda_3:";
      // 
      // root1
      // 
      root1.AutoSize = true;
      root1.Location = new System.Drawing.Point(6, 45);
      root1.Name = "root1";
      root1.Size = new System.Drawing.Size(56, 13);
      root1.TabIndex = 33;
      root1.Text = "lambda_1:";
      // 
      // tbox_lambda3_im
      // 
      tbox_lambda3_im.Location = new System.Drawing.Point(133, 94);
      tbox_lambda3_im.Name = "tbox_lambda3_im";
      tbox_lambda3_im.Size = new System.Drawing.Size(31, 20);
      tbox_lambda3_im.TabIndex = 15;
      tbox_lambda3_im.Text = "0";
      tbox_lambda3_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      tbox_lambda3_im.TextChanged += new System.EventHandler(Tbox_lambda3_im_TextChanged);
      // 
      // tbox_lambda1_re
      // 
      tbox_lambda1_re.Location = new System.Drawing.Point(83, 42);
      tbox_lambda1_re.Name = "tbox_lambda1_re";
      tbox_lambda1_re.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
      tbox_lambda1_re.Size = new System.Drawing.Size(31, 20);
      tbox_lambda1_re.TabIndex = 12;
      tbox_lambda1_re.Text = "0";
      tbox_lambda1_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // tbox_lambda3_re
      // 
      tbox_lambda3_re.Location = new System.Drawing.Point(83, 94);
      tbox_lambda3_re.Name = "tbox_lambda3_re";
      tbox_lambda3_re.Size = new System.Drawing.Size(31, 20);
      tbox_lambda3_re.TabIndex = 14;
      tbox_lambda3_re.Text = "0";
      tbox_lambda3_re.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // tbox_lambda1_im
      // 
      tbox_lambda1_im.Location = new System.Drawing.Point(133, 42);
      tbox_lambda1_im.Name = "tbox_lambda1_im";
      tbox_lambda1_im.Size = new System.Drawing.Size(31, 20);
      tbox_lambda1_im.TabIndex = 13;
      tbox_lambda1_im.Text = "0";
      tbox_lambda1_im.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      tbox_lambda1_im.TextChanged += new System.EventHandler(Tbox_lambda1_im_TextChanged);
      // 
      // cBox_non_linear
      // 
      cBox_non_linear.AutoSize = true;
      cBox_non_linear.Checked = true;
      cBox_non_linear.CheckState = System.Windows.Forms.CheckState.Checked;
      cBox_non_linear.Location = new System.Drawing.Point(147, 421);
      cBox_non_linear.Name = "cBox_non_linear";
      cBox_non_linear.Size = new System.Drawing.Size(122, 17);
      cBox_non_linear.TabIndex = 56;
      cBox_non_linear.Text = "Линейная система";
      cBox_non_linear.UseVisualStyleBackColor = true;
      // 
      // cBox_Reg_on
      // 
      cBox_Reg_on.AutoSize = true;
      cBox_Reg_on.Checked = true;
      cBox_Reg_on.CheckState = System.Windows.Forms.CheckState.Checked;
      cBox_Reg_on.Location = new System.Drawing.Point(8, 421);
      cBox_Reg_on.Name = "cBox_Reg_on";
      cBox_Reg_on.Size = new System.Drawing.Size(129, 17);
      cBox_Reg_on.TabIndex = 58;
      cBox_Reg_on.Text = "Включить регулятор";
      cBox_Reg_on.UseVisualStyleBackColor = true;
      // 
      // dataGridView1
      // 
      dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridView1.Location = new System.Drawing.Point(8, 444);
      dataGridView1.Name = "dataGridView1";
      dataGridView1.RowHeadersVisible = false;
      dataGridView1.Size = new System.Drawing.Size(329, 277);
      dataGridView1.TabIndex = 57;
      // 
      // groupBox2
      // 
      groupBox2.BackColor = System.Drawing.SystemColors.Control;
      groupBox2.Controls.Add(Button_setInitVal);
      groupBox2.Controls.Add(labelfi);
      groupBox2.Controls.Add(labeldfi_dt);
      groupBox2.Controls.Add(labelx);
      groupBox2.Controls.Add(labeldx_dt);
      groupBox2.Controls.Add(textBox_fi);
      groupBox2.Controls.Add(textBox_dx_dt);
      groupBox2.Controls.Add(textBox_x);
      groupBox2.Controls.Add(textBox_dfi_dt);
      groupBox2.Location = new System.Drawing.Point(8, 282);
      groupBox2.Margin = new System.Windows.Forms.Padding(2);
      groupBox2.Name = "groupBox2";
      groupBox2.Padding = new System.Windows.Forms.Padding(2);
      groupBox2.Size = new System.Drawing.Size(135, 134);
      groupBox2.TabIndex = 49;
      groupBox2.TabStop = false;
      groupBox2.Text = "Исходное значение";
      // 
      // Button_setInitVal
      // 
      Button_setInitVal.Location = new System.Drawing.Point(55, 108);
      Button_setInitVal.Margin = new System.Windows.Forms.Padding(2);
      Button_setInitVal.Name = "Button_setInitVal";
      Button_setInitVal.Size = new System.Drawing.Size(75, 25);
      Button_setInitVal.TabIndex = 25;
      Button_setInitVal.Text = "Принять";
      Button_setInitVal.UseVisualStyleBackColor = true;
      Button_setInitVal.Click += new System.EventHandler(Button_setInitVal_Click);
      // 
      // labelfi
      // 
      labelfi.AutoSize = true;
      labelfi.Location = new System.Drawing.Point(4, 21);
      labelfi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      labelfi.Name = "labelfi";
      labelfi.Size = new System.Drawing.Size(15, 13);
      labelfi.TabIndex = 21;
      labelfi.Text = "fi:";
      // 
      // labeldfi_dt
      // 
      labeldfi_dt.AutoSize = true;
      labeldfi_dt.Location = new System.Drawing.Point(4, 44);
      labeldfi_dt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      labeldfi_dt.Name = "labeldfi_dt";
      labeldfi_dt.Size = new System.Drawing.Size(35, 13);
      labeldfi_dt.TabIndex = 22;
      labeldfi_dt.Text = "dfi/dt:";
      // 
      // labelx
      // 
      labelx.AutoSize = true;
      labelx.Location = new System.Drawing.Point(4, 67);
      labelx.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      labelx.Name = "labelx";
      labelx.Size = new System.Drawing.Size(33, 13);
      labelx.TabIndex = 23;
      labelx.Text = "x - x*:";
      // 
      // labeldx_dt
      // 
      labeldx_dt.AutoSize = true;
      labeldx_dt.Location = new System.Drawing.Point(4, 88);
      labeldx_dt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      labeldx_dt.Name = "labeldx_dt";
      labeldx_dt.Size = new System.Drawing.Size(35, 13);
      labeldx_dt.TabIndex = 24;
      labeldx_dt.Text = "dx/dt:";
      // 
      // textBox_fi
      // 
      textBox_fi.Location = new System.Drawing.Point(55, 17);
      textBox_fi.Margin = new System.Windows.Forms.Padding(2);
      textBox_fi.Name = "textBox_fi";
      textBox_fi.Size = new System.Drawing.Size(76, 20);
      textBox_fi.TabIndex = 8;
      textBox_fi.Text = "0,523";
      textBox_fi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox_dx_dt
      // 
      textBox_dx_dt.Location = new System.Drawing.Point(55, 85);
      textBox_dx_dt.Margin = new System.Windows.Forms.Padding(2);
      textBox_dx_dt.Name = "textBox_dx_dt";
      textBox_dx_dt.Size = new System.Drawing.Size(76, 20);
      textBox_dx_dt.TabIndex = 11;
      textBox_dx_dt.Text = "0";
      textBox_dx_dt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox_x
      // 
      textBox_x.Location = new System.Drawing.Point(55, 63);
      textBox_x.Margin = new System.Windows.Forms.Padding(2);
      textBox_x.Name = "textBox_x";
      textBox_x.Size = new System.Drawing.Size(76, 20);
      textBox_x.TabIndex = 9;
      textBox_x.Text = "0,05";
      textBox_x.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox_dfi_dt
      // 
      textBox_dfi_dt.Location = new System.Drawing.Point(55, 40);
      textBox_dfi_dt.Margin = new System.Windows.Forms.Padding(2);
      textBox_dfi_dt.Name = "textBox_dfi_dt";
      textBox_dfi_dt.Size = new System.Drawing.Size(76, 20);
      textBox_dfi_dt.TabIndex = 10;
      textBox_dfi_dt.Text = "0";
      textBox_dfi_dt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // tabPage2
      // 
      tabPage2.Controls.Add(label5);
      tabPage2.Controls.Add(groupBox4);
      tabPage2.Controls.Add(pBox_Vmax_criterion);
      tabPage2.Controls.Add(pBox_h2_criterion);
      tabPage2.Controls.Add(pBox_h1_criterion);
      tabPage2.Controls.Add(pBox_H_criterion);
      tabPage2.Controls.Add(pBox_T_criterion);
      tabPage2.Controls.Add(func_num_text);
      tabPage2.Controls.Add(ymax_t);
      tabPage2.Location = new System.Drawing.Point(4, 22);
      tabPage2.Name = "tabPage2";
      tabPage2.Padding = new System.Windows.Forms.Padding(3);
      tabPage2.Size = new System.Drawing.Size(1356, 741);
      tabPage2.TabIndex = 1;
      tabPage2.Text = "Линии равного уровня";
      tabPage2.UseVisualStyleBackColor = true;
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.BackColor = System.Drawing.Color.PeachPuff;
      label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label5.ForeColor = System.Drawing.SystemColors.ControlText;
      label5.Location = new System.Drawing.Point(280, 6);
      label5.Name = "label5";
      label5.Size = new System.Drawing.Size(290, 62);
      label5.TabIndex = 24;
      label5.Text = "-mu2 < Re(lambda_i) < -mu1;\r\n|Im(lambda_i)| < -sigma * Re(lambda_i),\r\ni = (1, ..." +
    ", 4);";
      // 
      // groupBox4
      // 
      groupBox4.BackColor = System.Drawing.SystemColors.Control;
      groupBox4.Controls.Add(label_mu1);
      groupBox4.Controls.Add(xmin_t);
      groupBox4.Controls.Add(label_M3);
      groupBox4.Controls.Add(xmax_t);
      groupBox4.Controls.Add(label_M2);
      groupBox4.Controls.Add(label_mu2);
      groupBox4.Controls.Add(label_M1);
      groupBox4.Controls.Add(button1);
      groupBox4.Controls.Add(ymin_t);
      groupBox4.Controls.Add(label_N);
      groupBox4.Controls.Add(label_sigma);
      groupBox4.Controls.Add(DL_N);
      groupBox4.Controls.Add(DL_M2);
      groupBox4.Controls.Add(DL_M1);
      groupBox4.Controls.Add(DL_M3);
      groupBox4.Location = new System.Drawing.Point(6, 6);
      groupBox4.Name = "groupBox4";
      groupBox4.Size = new System.Drawing.Size(268, 153);
      groupBox4.TabIndex = 23;
      groupBox4.TabStop = false;
      groupBox4.Text = "Параметры отрисовки:";
      // 
      // label_mu1
      // 
      label_mu1.AutoSize = true;
      label_mu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_mu1.Location = new System.Drawing.Point(6, 16);
      label_mu1.Name = "label_mu1";
      label_mu1.Size = new System.Drawing.Size(44, 20);
      label_mu1.TabIndex = 16;
      label_mu1.Text = "mu1:";
      // 
      // xmin_t
      // 
      xmin_t.Location = new System.Drawing.Point(67, 18);
      xmin_t.Name = "xmin_t";
      xmin_t.Size = new System.Drawing.Size(75, 20);
      xmin_t.TabIndex = 8;
      xmin_t.Text = "6";
      xmin_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // label_M3
      // 
      label_M3.AutoSize = true;
      label_M3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_M3.Location = new System.Drawing.Point(148, 94);
      label_M3.Name = "label_M3";
      label_M3.Size = new System.Drawing.Size(35, 20);
      label_M3.TabIndex = 22;
      label_M3.Text = "M3:";
      // 
      // xmax_t
      // 
      xmax_t.Location = new System.Drawing.Point(67, 44);
      xmax_t.Name = "xmax_t";
      xmax_t.Size = new System.Drawing.Size(75, 20);
      xmax_t.TabIndex = 6;
      xmax_t.Text = "0,1";
      xmax_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // label_M2
      // 
      label_M2.AutoSize = true;
      label_M2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_M2.Location = new System.Drawing.Point(148, 68);
      label_M2.Name = "label_M2";
      label_M2.Size = new System.Drawing.Size(35, 20);
      label_M2.TabIndex = 21;
      label_M2.Text = "M2:";
      // 
      // label_mu2
      // 
      label_mu2.AutoSize = true;
      label_mu2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_mu2.Location = new System.Drawing.Point(6, 42);
      label_mu2.Name = "label_mu2";
      label_mu2.Size = new System.Drawing.Size(44, 20);
      label_mu2.TabIndex = 17;
      label_mu2.Text = "mu2:";
      // 
      // label_M1
      // 
      label_M1.AutoSize = true;
      label_M1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_M1.Location = new System.Drawing.Point(148, 42);
      label_M1.Name = "label_M1";
      label_M1.Size = new System.Drawing.Size(35, 20);
      label_M1.TabIndex = 20;
      label_M1.Text = "M1:";
      // 
      // button1
      // 
      button1.Location = new System.Drawing.Point(2, 122);
      button1.Name = "button1";
      button1.Size = new System.Drawing.Size(262, 25);
      button1.TabIndex = 10;
      button1.Text = "Рассчитать";
      button1.UseVisualStyleBackColor = true;
      button1.Click += new System.EventHandler(button1_Click);
      // 
      // ymin_t
      // 
      ymin_t.Location = new System.Drawing.Point(67, 70);
      ymin_t.Name = "ymin_t";
      ymin_t.Size = new System.Drawing.Size(75, 20);
      ymin_t.TabIndex = 7;
      ymin_t.Text = "1";
      ymin_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // label_N
      // 
      label_N.AutoSize = true;
      label_N.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_N.Location = new System.Drawing.Point(159, 16);
      label_N.Name = "label_N";
      label_N.Size = new System.Drawing.Size(24, 20);
      label_N.TabIndex = 19;
      label_N.Text = "N:";
      // 
      // label_sigma
      // 
      label_sigma.AutoSize = true;
      label_sigma.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      label_sigma.Location = new System.Drawing.Point(6, 68);
      label_sigma.Name = "label_sigma";
      label_sigma.Size = new System.Drawing.Size(55, 20);
      label_sigma.TabIndex = 18;
      label_sigma.Text = "sigma:";
      // 
      // DL_N
      // 
      DL_N.Location = new System.Drawing.Point(189, 18);
      DL_N.Name = "DL_N";
      DL_N.Size = new System.Drawing.Size(75, 20);
      DL_N.TabIndex = 3;
      DL_N.Text = "50";
      DL_N.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // DL_M2
      // 
      DL_M2.Location = new System.Drawing.Point(189, 70);
      DL_M2.Name = "DL_M2";
      DL_M2.Size = new System.Drawing.Size(75, 20);
      DL_M2.TabIndex = 1;
      DL_M2.Text = "5";
      DL_M2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // DL_M1
      // 
      DL_M1.Location = new System.Drawing.Point(189, 44);
      DL_M1.Name = "DL_M1";
      DL_M1.Size = new System.Drawing.Size(75, 20);
      DL_M1.TabIndex = 2;
      DL_M1.Text = "10";
      DL_M1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // DL_M3
      // 
      DL_M3.Location = new System.Drawing.Point(189, 96);
      DL_M3.Name = "DL_M3";
      DL_M3.Size = new System.Drawing.Size(75, 20);
      DL_M3.TabIndex = 4;
      DL_M3.Text = "3";
      DL_M3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // pBox_Vmax_criterion
      // 
      pBox_Vmax_criterion.BackColor = System.Drawing.SystemColors.Control;
      pBox_Vmax_criterion.Location = new System.Drawing.Point(1003, 362);
      pBox_Vmax_criterion.Name = "pBox_Vmax_criterion";
      pBox_Vmax_criterion.Size = new System.Drawing.Size(350, 350);
      pBox_Vmax_criterion.TabIndex = 15;
      pBox_Vmax_criterion.TabStop = false;
      // 
      // pBox_h2_criterion
      // 
      pBox_h2_criterion.BackColor = System.Drawing.SystemColors.Control;
      pBox_h2_criterion.Location = new System.Drawing.Point(647, 362);
      pBox_h2_criterion.Name = "pBox_h2_criterion";
      pBox_h2_criterion.Size = new System.Drawing.Size(350, 350);
      pBox_h2_criterion.TabIndex = 14;
      pBox_h2_criterion.TabStop = false;
      // 
      // pBox_h1_criterion
      // 
      pBox_h1_criterion.BackColor = System.Drawing.SystemColors.Control;
      pBox_h1_criterion.Location = new System.Drawing.Point(291, 362);
      pBox_h1_criterion.Name = "pBox_h1_criterion";
      pBox_h1_criterion.Size = new System.Drawing.Size(350, 350);
      pBox_h1_criterion.TabIndex = 13;
      pBox_h1_criterion.TabStop = false;
      // 
      // pBox_H_criterion
      // 
      pBox_H_criterion.BackColor = System.Drawing.SystemColors.Control;
      pBox_H_criterion.Location = new System.Drawing.Point(1003, 6);
      pBox_H_criterion.Name = "pBox_H_criterion";
      pBox_H_criterion.Size = new System.Drawing.Size(350, 350);
      pBox_H_criterion.TabIndex = 12;
      pBox_H_criterion.TabStop = false;
      // 
      // pBox_T_criterion
      // 
      pBox_T_criterion.BackColor = System.Drawing.SystemColors.Control;
      pBox_T_criterion.Location = new System.Drawing.Point(647, 6);
      pBox_T_criterion.Name = "pBox_T_criterion";
      pBox_T_criterion.Size = new System.Drawing.Size(350, 350);
      pBox_T_criterion.TabIndex = 0;
      pBox_T_criterion.TabStop = false;
      pBox_T_criterion.Paint += new System.Windows.Forms.PaintEventHandler(pic_Paint);
      // 
      // func_num_text
      // 
      func_num_text.Location = new System.Drawing.Point(392, 205);
      func_num_text.Name = "func_num_text";
      func_num_text.Size = new System.Drawing.Size(100, 20);
      func_num_text.TabIndex = 9;
      func_num_text.Text = "2";
      func_num_text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // ymax_t
      // 
      ymax_t.Location = new System.Drawing.Point(498, 205);
      ymax_t.Name = "ymax_t";
      ymax_t.Size = new System.Drawing.Size(75, 20);
      ymax_t.TabIndex = 5;
      ymax_t.Text = "10";
      ymax_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // drawing_on
      // 
      drawing_on.AutoSize = true;
      drawing_on.Checked = true;
      drawing_on.CheckState = System.Windows.Forms.CheckState.Checked;
      drawing_on.Location = new System.Drawing.Point(5, 104);
      drawing_on.Name = "drawing_on";
      drawing_on.Size = new System.Drawing.Size(81, 17);
      drawing_on.TabIndex = 47;
      drawing_on.Text = "Отрисовка";
      drawing_on.UseVisualStyleBackColor = true;
      // 
      // PortalCraneModel
      // 
      AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      AutoSize = true;
      ClientSize = new System.Drawing.Size(1364, 755);
      Controls.Add(tabControl1);
      Margin = new System.Windows.Forms.Padding(2);
      Name = "PortalCraneModel";
      SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      Text = "PortalCraneModel";
      tabControl1.ResumeLayout(false);
      tabPage1.ResumeLayout(false);
      tabPage1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(chart2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(chart4)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(chart3)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(chart1)).EndInit();
      groupBox3.ResumeLayout(false);
      groupBox3.PerformLayout();
      groupBox1.ResumeLayout(false);
      groupBox1.PerformLayout();
      roots_gbox.ResumeLayout(false);
      roots_gbox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(dataGridView1)).EndInit();
      groupBox2.ResumeLayout(false);
      groupBox2.PerformLayout();
      tabPage2.ResumeLayout(false);
      tabPage2.PerformLayout();
      groupBox4.ResumeLayout(false);
      groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(pBox_Vmax_criterion)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_h2_criterion)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_h1_criterion)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_H_criterion)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(pBox_T_criterion)).EndInit();
      ResumeLayout(false);

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
      DrawCriteria = new double[criteria.drawCount * 5];

      // определяем размер управляемой структуры
      int sizeStruct = Marshal.SizeOf(typeof(PortalCraneModel.TAllDrawPoints));

      // выделяем память под неуправляемую структуру
      ptrCriteria = Marshal.AllocHGlobal(sizeStruct);

      // копируем данные из неуправляемой в управляемую
      Marshal.StructureToPtr(criteria, ptrCriteria, false);

      // выделяем память под внутренний неуправляемый массив в неупр структуре
      PortalCraneModel.InitAllPointsArray(ptrCriteria);

      is_calc_criteria = true;

      Draw_Line.SetDat(XMin, XMax, YMin, YMax, false, System.Convert.ToInt32(func_num_text.Text));

      is_calc_criteria = false;

      pBox_T_criterion.Invalidate();
    }

    private void pic_Paint(object sender, PaintEventArgs e)
    {
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
      Draw_Line.SendLines(e.Graphics, pBox_T_criterion);
    }
  }
}


