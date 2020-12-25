#include "pch.h"

#include "Function.h"

void Function::Set_func_index(int _func_index)
{
  if (_func_index != NULL)
    func_index = _func_index;
}

criteria Function::Get_value(std::vector<double>& _x, bool system)
{
  // If you want to add a new function, add a case for it to the switch
  switch (func_index)
  {
    case 1:
    {
      /*SetModelLambdas(_x[0], _x[1],
                      _x[0], -1 * _x[1],
                      _x[0], _x[1],
                      _x[0], -1 * _x[1]);

      Calc_regulator();*/

      //return Calc_criteria_eque_lines(system);
      // zaplatka
      criteria aaa;
      return aaa;
    }

    //case 2: 
    //      // (4 - 2.1 * x1^2 + x1^4 / 3) * x1^2
    //  return (4 - 2.1 * _x[0] * _x[0] + pow(_x[0], 4) / 3) * _x[0] * _x[0]
    // // + x1 * x2 + (4 * x2^2 - 4) * x2^2
    //    + _x[0] * _x[1] + (4 * _x[1] * _x[1] - 4) * _x[1] * _x[1];

    //default:
    //  return 0;
  }
}
