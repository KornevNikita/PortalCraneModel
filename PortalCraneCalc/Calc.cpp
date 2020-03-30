#include "pch.h"
#include "Calc.h"

double M, m, l, R, g, h_fi, h_x, B, gamma, E; // параметры модели
double fi, dfi_dt, x, dx_dt; // начальное состо€ние
double dt, t_start, t_stop; //
int drawStCount; //
bool inDinamic; // параметры расчета
double xMax, yMax; // параметры масштаба

void f(const std::vector<double>& _X, std::vector<double>& _k)
{
  _k[0] = _X[1]; // fi

  _k[1] = -_X[1] * (M + m) * h_fi / (M * m * l * l) 
    - _X[0] * (M + m) * g / (M * l) 
    + _X[3] * (gamma * E / (R * B) + h_x) / (M * l) 
    - gamma / (M * R * l); // dfi_dt

  _k[2] = _X[3]; // x

  _k[3] = _X[1] * h_fi / (M * l) 
    + _X[0] * m * g / M 
    - _X[3] * (gamma * E / (R * B) + h_x) / M 
    + gamma / (M * R); // dx_dt
}


point TDinModel::RK4()
{
  static std::vector<double> k1(4), k2(4), k3(4), k4(4), temp(4);
  for (int i = 1; i < drawStCount; i++) {
    f(X, k1);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k1[j] * 0.5 * dt;
    f(temp, k2);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k2[j] * 0.5 * dt;
    f(temp, k3);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k3[j] * dt;
    f(temp, k4);
    for (int j = 0; j < n; j++)
      X[j] = X[j] + dt / 6 * (k1[j] + 2 * k2[j] + 2 * k3[j] + k4[j]);
  }
  X[4] += dt * drawStCount;
  point res(X[0], X[1], X[2], X[3], X[4]);
  return res;
}

void SetModelParams(double _M, double _m, double _l, double _R, double _g, double _h_fi, double _h_x, double _B, double _gamma, double _E)
{
  M = _M, m = _m, l = _l, R = _R, g = _g, h_fi = _h_fi, h_x = _h_x, B = _B, gamma = _gamma, E = _E;
}

void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt)
{
  fi = _fi, dfi_dt = _dfi_dt, x = _x, dx_dt = _dx_dt;
}

void SetCalcParams(double _dt, double _t_start, double _t_stop, int _drawStCount, bool _inDinamic)
{
  dt = _dt, t_start = _t_start, t_stop = _t_stop, drawStCount = _drawStCount, inDinamic = _inDinamic;
}

void SetScaleParams(double _xMax, double _yMax)
{
  xMax = _xMax;
  yMax = _yMax;
}

int GetAllDrawPointsCount()
{
  return static_cast<int>((t_stop - t_start) / (drawStCount * dt));
}

//¬ыделение пам€ти под внутренний массив структуры TAllDrawPoints
//–азмер массива должен быть уже заложен в поле drawCount
void InitAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->AllocMem(allDrawData->drawCount);
}
//ќсвобождение пам€ти от внутреннего массива в структуре TAllDrawPoints
void DeleteAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->FreeMem();
}
//«аполнение массива всеми отображаемыми точками (count штук)
void GetAllDrawPoints(TAllDrawPoints* allDrawData)
{
  point drawPoint(fi, dfi_dt, x, dx_dt, t_start);
  TDinModel model(4, drawPoint);
  allDrawData->allDrawPoints[0] = drawPoint;

  for (int i = 1; i < allDrawData->drawCount; i++)
  {
    allDrawData->allDrawPoints[i] = model.RK4();
  }
}