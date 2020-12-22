#pragma once

#include <vector>
#include <string>
#include "Function.h"

std::string dll = "PortalCraneCalc.dll";

struct Node
{
  double x, y;  // coordinates
  double Q;     // function value
};

double XMin, XMax, YMin, YMax; // calculation area

Node* pDat;
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
void SetDat(double _a0, double _b0, double _a1, double _b1, int F_Num);
