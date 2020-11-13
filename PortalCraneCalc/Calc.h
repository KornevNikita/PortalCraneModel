#pragma once
#include <vector>
#include <complex>
using namespace std;

struct point 
{
  double fi, dfi_dt, x, dx_dt, t;

  point() : fi(0), dfi_dt(0), x(0), dx_dt(0), t(0) {};

  point(double _fi, double _dfi_dt, double _x, double _dx_dt, double _t) :
    fi(_fi), dfi_dt(_dfi_dt), x(_x), dx_dt(_dx_dt), t(_t) {};
};

struct TAllDrawPoints {
  point* allDrawPoints;

  unsigned drawCount;

  void AllocMem(unsigned _drawCount)
  {
    allDrawPoints = new point[_drawCount];
    drawCount = _drawCount;
  }

  void FreeMem() {
    delete allDrawPoints;
    allDrawPoints = 0;
  }
};

void f(const std::vector<double>& _X, std::vector<double>& _k,
  bool system, bool reg_on);

void calc_coeffs(const vector<complex<double>>& p, vector<double>& g);

void init_matrix_A();

/* ============================= Export functions: ============================== */

extern "C" __declspec(dllexport)
void SetModelParams(double _M, double _m, double _l, double _R, double _g,
  double _h_fi, double _h_x, double _B, double _gamma, double _E);

extern "C" __declspec(dllexport)
void SetModelLambdas(double _p1_re, double _p1_im, double _p2_re, double _p2_im,
  double _p3_re, double _p3_im, double _p4_re, double _p4_im);

extern "C" __declspec(dllexport)
void SetCalcParams(double _dt, double _t_start, double _t_stop,
  int _drawStCount, bool _inDinamic);

extern "C" __declspec(dllexport)
void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt);

extern "C" __declspec(dllexport)
int GetAllDrawPointsCount();

extern "C" __declspec(dllexport)
void InitAllPointsArray(TAllDrawPoints* allDrawData);

extern "C" __declspec(dllexport)
void DeleteAllPointsArray(TAllDrawPoints* allDrawData);

extern "C" __declspec(dllexport)
void GetAllDrawPoints(TAllDrawPoints* ptrAllDrawPoints,
  bool system, bool reg_on);

extern "C" __declspec(dllexport)
void Calc_regulator();

extern "C" __declspec(dllexport)
void Calc_criteria();
