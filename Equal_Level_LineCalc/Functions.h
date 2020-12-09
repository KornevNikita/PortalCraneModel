#pragma once
#include <cmath>
#include <vector>

class Function
{
  int func_index = 1;

public:

  void Set_func_index(int _func_index);

  double Get_value(std::vector<double>& _x);
};