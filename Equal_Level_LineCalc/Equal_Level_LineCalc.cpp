#include "pch.h"

#include "Equal_Level_LineCalc.h"

void SetBorders(double _XMin, double _XMax, double _YMin, double _YMax)
{
  if (_XMin != NULL && _XMax != NULL && _YMin != NULL && _YMax != NULL)
  {
    XMin = _XMin;
    XMax = _XMax;
    YMin = _YMin;
    YMax = _YMax;
  }
}

void CreateDat(int _N, int _M1, int _M2, int _M3)
{
  N = _N;
  M1 = _M1;
  M2 = _M2;
  M3 = _M3;
  M = M1 + M2 + M3 - 1;

  if ((pDat = new Node<criteria>[(N + 1) * (N + 1)]) == nullptr)
    return;
  else
    if ((pQ = new criteria[M + 1]) == nullptr)
      return;
}

void SetDat(int F_Num, bool system)
{
  {
    Function F;

    if (pDat != nullptr && pQ != nullptr)
    {
      double Qmin, Qmax;
      criteria QQ;
      double hx = (XMax - XMin) / N; // вычисление шага по x
      double hy = (YMax - YMin) / N; // вычисление шага по y
      std::vector<double> x(2);

      // обход сетки
      Qmin = 1.7976931348623158e+308;
      Qmax = 2.2250738585072014e-308;

      F.Set_func_index(F_Num);
      for (int i = 0; i <= N; i++)
        for (int j = 0; j <= N; j++)
        {
          // заполнение структуры сетки
          // координаты узла сетки
          x[0] = pDat[(N + 1) * i + j].x = XMin + hx * i;
          x[1] = pDat[(N + 1) * i + j].y = YMin + hy * j;

          QQ = pDat[(N + 1) * i + j].Q = F.Get_value(x, system); // schitaem znachenie v (x,y)
        }
    }
  }
}
