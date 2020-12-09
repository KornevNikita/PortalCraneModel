using System;

namespace Contour_line
{
  class CFunction
  {
    private int index = 5;

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

        default: return 0; // тут мы вернем 0;
      }
    }
    public void set(int _ind)
    {
      index = _ind;
    } //Выбрать функцию;
  }
}
