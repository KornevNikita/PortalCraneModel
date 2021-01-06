#pragma once

#include <vector>

#include "Function.h"
#include "Calc.h"

template <typename T>
struct Node
{
  double x, y;  // coordinates
  T Q;     // function value
};

double XMin, XMax, YMin, YMax; // calculation area
double alpha, sigma;
bool calc_in_modified_variables = false;

Node<criteria>* pDat;
double* pQ;
int N; // число разбиений сетки
int M; // общее число уровней
int M1; // число основных узлов
int M2; // число подуузлов
int M3; // число "подподузлов"

extern "C" __declspec(dllexport)
void SetBorders(double _XMin, double _XMax, double _YMin, double _YMax);

extern "C" __declspec(dllexport)
void CreateDat(int _N, int _M1, int _M2, int _M3);

extern "C" __declspec(dllexport)
void SetDat(int F_Num, bool system);

extern "C" __declspec(dllexport)
void SetSubLevels(int shift);

extern "C" __declspec(dllexport)
void Get_pDat_and_pQ(TAllDrawPoints<criteria>* ptr);

extern "C" __declspec(dllexport)
void Set_calc_in_init_variables();

extern "C" __declspec(dllexport)
void Set_calc_in_modified_variables();

extern "C" __declspec(dllexport)
void Set_alpha(double _alpha);

extern "C" __declspec(dllexport)
void Set_sigma(double _sigma);