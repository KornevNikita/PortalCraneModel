#include "Equal_Level_LineCalc.h"

void SetDat(double _a0, double _b0, double _a1, double _b1, int F_Num)
{
  {
    Function F;

    if (pDat != nullptr && pQ != nullptr)
    {
      double Qmin, Qmax, QQ;
      double hx = (_b0 - _a0) / N; // вычисление шага по x
      double hy = (_b1 - _a1) / N; // вычисление шага по y
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
          x[0] = pDat[(N + 1) * i + j].x = _a0 + hx * i;
          x[1] = pDat[(N + 1) * i + j].y = _a1 + hy * j;

          QQ = pDat[(N + 1) * i + j].Q = F.Get_value(x); // schitaem znachenie v (x,y)
        }
    }
  }
}