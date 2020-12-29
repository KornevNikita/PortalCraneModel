#include "pch.h"

#include "Equal_Level_LineCalc.h"

#include <iostream>
#include <fstream>
using namespace std;

size_t criteria_count = 0;

void SetBorders(double _XMin, double _XMax, double _YMin, double _YMax)
{
  if (_XMin != NULL && _XMax != NULL && _YMin != NULL && _YMax != NULL)
  {
    XMin = _XMin;
    XMax = _XMax;
    YMin = _YMin;
    YMax = _YMax;
  }
  std::ofstream fout("eq-lvl-log.txt", ios::app);
  fout << "Setted borders" << endl;
}

void CreateDat(int _N, int _M1, int _M2, int _M3)
{
  N = _N;
  M1 = _M1;
  M2 = _M2;
  M3 = _M3;
  M = M1 + M2 + M3;
  
  std::ofstream fout("eq-lvl-log.txt", ios::app);
  fout << "Data created" << endl;

  if ((pDat = new Node<criteria>[(N + 1) * (N + 1)]) == nullptr)
    return;
  else
    /*if ((pQ = new double[(M + 1) * 5]) == nullptr)*/
    if ((pQ = new double[M * 5]) == nullptr)
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

      std::ofstream fout("eq-lvl-log.txt", ios::app);
      fout.precision(12);
      fout << "criteria:" << endl;

      F.Set_func_index(F_Num);
      for (int i = 0; i <= N; i++)
        for (int j = 0; j <= N; j++)
        {
          // заполнение структуры сетки
          // координаты узла сетки
          x[0] = pDat[(N + 1) * i + j].x = XMin + hx * i;
          x[1] = pDat[(N + 1) * i + j].y = YMin + hy * j;

          QQ = pDat[(N + 1) * i + j].Q = F.Get_value(x, system); // schitaem znachenie v (x,y)
          
          fout << criteria_count << ") ";
          fout << "(" << x[0] << ", " << x[1] << ") " <<
            "(" << x[0] << ", " << -1. * x[1] << ") " <<
            "(" << x[0] << ", " << x[1] << ") " <<
            "(" << x[0] << ", " << -1. * x[1] << ") " << endl;
          fout << QQ;
          fout << endl;

          criteria_count++;
        }
    }
  }
}

void SetSubLevels(int shift)
{
  double Qmin = 1.7976931348623158e+308,
    Qmax = 2.2250738585072014e-308, QQ;

  std::ofstream fout("eq-lvl-log.txt", ios::app);
  fout.precision(12);
  for (int i = 0; i <= N; i++)
    for (int j = 0; j <= N; j++)
    {
      switch (shift)
      {
      case 0:
        QQ = pDat[(N + 1) * i + j].Q.T;
        break;
      case 1:
        QQ = pDat[(N + 1) * i + j].Q.H;
        break;
      case 2:
        QQ = pDat[(N + 1) * i + j].Q.h1;
        break;
      case 3:
        QQ = pDat[(N + 1) * i + j].Q.h2;
        break;
      case 4:
        QQ = pDat[(N + 1) * i + j].Q.Vmax;
        break;
      }

      if ((i == 0) && (j == 0) || (QQ < Qmin)) { Qmin = QQ; }
      if ((i == 0) && (j == 0) || (QQ > Qmax)) { Qmax = QQ; }
    }

  double hQ1 = (Qmax - Qmin) / M1; // шаг функции по уровням
  int ku = 0; // позиция в сетке уровней   
  for (int i = 0; i < M1; i++) // вычисление значений функции на основных уровнях 
  {
    pQ[ku + shift] = Qmax - hQ1 * i;
    ku += 5;
  }

  double hQ2 = hQ1 / (M2 + 1); // шаг функции по подуровням
  for (int i = 1; i <= M2; i++) // вычисление значений функции на подуровнях
  {
    pQ[ku + shift] = pQ[M1 - 1 - (4 - shift)] - hQ2 * i;
    ku += 5;
  }

  for (int i = 1; i <= M3; i++) // вычисление значений функции на "под-подуровнях"
  {
    pQ[ku + shift] = pQ[M1 + M2 - 1 - (4 - shift)] - (hQ2 / (M3 + 1)) * i;
    ku += 5;
  }
}

void Get_pDat_and_pQ(TAllDrawPoints<criteria>* ptr)
{
  // put pDat
  for (int i = 0; i < (N + 1) * (N + 1); i++)
    ptr->allDrawPoints[i] = pDat[i].Q;

  //put pQ
  size_t count = 0;
  for (int i = 0; i < M; ++i)
  {
    criteria temp(pQ[i * 5], pQ[i * 5 + 1], pQ[i * 5 + 2], pQ[i * 5 + 3], pQ[i * 5 + 4]);
    ptr->allDrawPoints[(N + 1) * (N + 1) + count++] = temp;
  }
}