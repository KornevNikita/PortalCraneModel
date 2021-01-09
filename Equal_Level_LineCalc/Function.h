#pragma once
#include <cmath>
#include <vector>

#include "Calc.h"

class Function
{
  int func_index = 1;

public:

  void Set_func_index(int _func_index);

  criteria Get_value(std::vector<double>& _x, bool system);
};
