#include "Functions.h"

void Function::Set_func_index(int _func_index)
{
  if (_func_index != NULL)
    func_index = _func_index;
}

double Function::Get_value(std::vector<double>& _x)
{
  // If you want to add a new function, add a case for it to the switch
  switch (func_index)
  {
    case 1:
    {
      /*PortalCraneModel.PortalCraneModel.SetModelLambdas(
        _x[0], _x[1],
        _x[0], -1 * _x[1],
        _x[0], _x[1],
        _x[0], -1 * _x[1]);

      PortalCraneModel.PortalCraneModel.Calc_regulator();

      PortalCraneModel.PortalCraneModel.Calc_criteria_eque_lines(
        PortalCraneModel.PortalCraneModel.ptrCriteria,
        PortalCraneModel.PortalCraneModel.cBox_non_linear.Checked);*/

      return 0;
    }

    case 2: 
          // (4 - 2.1 * x1^2 + x1^4 / 3) * x1^2
      return (4 - 2.1 * _x[0] * _x[0] + pow(_x[0], 4) / 3) * _x[0] * _x[0]
     // + x1 * x2 + (4 * x2^2 - 4) * x2^2
        + _x[0] * _x[1] + (4 * _x[1] * _x[1] - 4) * _x[1] * _x[1];

    default:
      return 0;
  }
}
