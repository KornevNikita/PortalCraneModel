#include "pch.h"

#include "TDinModel.h"

extern int drawStCount;
extern double dt;

point TDinModel::RK4(bool system, bool reg_on)
{
  static std::vector<double> k1(4), k2(4), k3(4), k4(4), temp(4);

  for (int i = 0; i < drawStCount; i++) {
    f(X, k1, system, reg_on);
    for (unsigned j = 0; j < n; j++)
      temp[j] = X[j] + k1[j] * 0.5 * dt;

    f(temp, k2, system, reg_on);
    for (unsigned j = 0; j < n; j++)
      temp[j] = X[j] + k2[j] * 0.5 * dt;

    f(temp, k3, system, reg_on);
    for (unsigned j = 0; j < n; j++)
      temp[j] = X[j] + k3[j] * dt;

    f(temp, k4, system, reg_on);

    for (unsigned j = 0; j < n; j++)
      X[j] = X[j] + dt / 6.0 * (k1[j] + 2.0 * k2[j] + 2.0 * k3[j] + k4[j]);
  }

  X[4] += dt * drawStCount;

  return point{ X[0], X[1], X[2], X[3], X[4] };
}