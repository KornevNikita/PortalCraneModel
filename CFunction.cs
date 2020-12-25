using System;

namespace Contour_line
{
  class CFunction
  {
    public int curInd = 0;
    private int index = 5;
    private double C1 = 0;
    private double C2 = 0;
    private double M1 = 0;
    private double M2 = 0;
    private double[] W1 = new double[2];
    private double[] W2 = new double[2];

    public double GetValue(double[] _x)
    {
      switch (index)
      {
        case 2: //(4 - 2.1*y1^2 + y1^4/3)*y1^2 + y1*y2 + (4*y2^2 - 4)*y2^2
          return ((4 - 2.1 * _x[0] * _x[0] + Math.Pow(_x[0], 4) / 3) * _x[0] * _x[0] + _x[0] * _x[1] + (4 * _x[1] * _x[1] - 4) * _x[1] * _x[1]);

        case 8:
          {
            PortalCraneModel.PortalCraneModel.SetModelLambdas(
              _x[0], _x[1],
              _x[0], -1 * _x[1],
              _x[0], _x[1],
              _x[0], -1 * _x[1]);

            PortalCraneModel.PortalCraneModel.Calc_regulator();

            PortalCraneModel.PortalCraneModel.Calc_criteria_eque_lines(
              PortalCraneModel.PortalCraneModel.ptrCriteria,
              PortalCraneModel.PortalCraneModel.cBox_non_linear.Checked);

            return 0;
          }

        case 10:
          {
            return Math.Exp(_x[1] * _x[1] - _x[0]) +
              5 * Math.Pow(_x[0] - _x[1] * _x[1], 2) +
              Math.Pow(_x[1] - 1, 2);
          }

        default: return 0; // тут мы вернем 0;
      }
    }
    public void set(int _ind)
    {
      index = _ind;
    } //Выбрать функцию;

    public void set_func(double m1, double m2, double c1, double c2, double[] w1, double[] w2)
    {
      M1 = m1;
      M2 = m2;
      C1 = c1;
      C2 = c2;
      W1 = w1;
      W2 = w2;

    } //Задать функцию;
  }
}
