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
int N; // ����� ��������� �����
int M; // ����� ����� �������
int M1; // ����� �������� �����
int M2; // ����� ���������
int M3; // ����� "�����������"

extern "C" __declspec(dllexport)
void SetDat(double _a0, double _b0, double _a1, double _b1, int F_Num);