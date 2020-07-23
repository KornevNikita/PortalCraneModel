#pragma once

#include <vector>
#include "Calc.h"

class TDinModel {
  unsigned n; // размерость системы
  std::vector<double> X; // текущее состояние

public:
  TDinModel(unsigned _n, const point& p) : n(_n)
  {
    X.push_back(p.fi);
    X.push_back(p.dfi_dt);
    X.push_back(p.x);
    X.push_back(p.dx_dt);
    X.push_back(p.t);
  }

  point RK4(bool system, bool reg_on);
};