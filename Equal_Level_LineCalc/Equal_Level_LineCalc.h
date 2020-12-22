#pragma once

#include <vector>
#include "Functions.h"

struct Node
{
  double x, y;  // coordinates
  double Q;     // function value
};

double XMin, XMax, YMin, YMax;

Node* pDat;
double* pQ;
int N; // число разбиений сетки
int M; // общее число уровней
int M1; // число основных узлов
int M2; // число подуузлов
int M3; // число "подподузлов"

extern "C" __declspec(dllexport)
void SetDat(double _a0, double _b0, double _a1, double _b1, int F_Num);