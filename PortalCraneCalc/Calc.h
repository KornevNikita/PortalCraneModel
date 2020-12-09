#pragma once

#include <vector>
#include <complex>
#include <fstream>
using namespace std;

/* ============================== Structures: ==============================  */

struct point 
{
  double fi, dfi_dt, x, dx_dt, t;

  point() : fi(0), dfi_dt(0), x(0), dx_dt(0), t(0) {};

  point(double _fi, double _dfi_dt, double _x, double _dx_dt, double _t) :
    fi(_fi), dfi_dt(_dfi_dt), x(_x), dx_dt(_dx_dt), t(_t) {};

  friend std::ofstream& operator<< (std::ofstream& out, const point& p)
  {
    out << p.t << "\t" << p.fi << "\t" << p.dfi_dt
      << "\t" << p.x << "\t" << p.dx_dt << endl;

    return out;
  }
};

struct criteria
{
  double T, H, h1, h2, Vmax;

  criteria() : T(0), H(0), h1(0), h2(0), Vmax(0) {};

  criteria(double _T, double _H, double _h1, double _h2, double _Vmax) :
    T(_T), H(_H), h1(_h1), h2(_h2), Vmax(_Vmax) {};

  friend std::ofstream& operator<< (std::ofstream& out, const criteria& c)
  {
    out << c.T << "\t" << c.H << "\t" << c.h1 << "\t"
      << c.h2 << "\t" << c.Vmax << endl;

    return out;
  }
};

template <typename T>
struct TAllDrawPoints {
  T* allDrawPoints;
  unsigned drawCount;

  void AllocMem(unsigned _drawCount)
  {
    allDrawPoints = new point[_drawCount];
    drawCount = _drawCount;
  }

  void FreeMem() {
    delete allDrawPoints;
    drawCount = 0;
  }
};

/* =========================== End of structures ============================ */

/* =========================== Internal functions: ========================== */

void f(const std::vector<double>& _X, std::vector<double>& _k,
  bool system, bool reg_on);

void calc_coeffs(const vector<complex<double>>& p, vector<double>& g);

void init_matrix_A();

void Calc_criteria(criteria& c);

/* ======================== End of nternal functions: ======================= */

/* =========================== Export functions: ============================ */

extern "C" __declspec(dllexport)
void SetModelParams(double _M, double _m, double _l, double _R, double _g,
  double _h_fi, double _h_x, double _B, double _gamma, double _E);

extern "C" __declspec(dllexport)
void SetModelLambdas(double _p1_re, double _p1_im, double _p2_re, double _p2_im,
  double _p3_re, double _p3_im, double _p4_re, double _p4_im);

extern "C" __declspec(dllexport)
void SetCalcParams(double _dt, double _t_start, double _t_stop,
  int _drawStCount);

extern "C" __declspec(dllexport)
void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt);

extern "C" __declspec(dllexport)
int GetAllDrawPointsCount();

extern "C" __declspec(dllexport)
void InitAllPointsArray(TAllDrawPoints<point>* allDrawData);

extern "C" __declspec(dllexport)
void DeleteAllPointsArray(TAllDrawPoints<point>* allDrawData);

extern "C" __declspec(dllexport)
void GetAllDrawPoints(TAllDrawPoints<point>* ptrAllDrawPoints,
  bool system, bool reg_on);

extern "C" __declspec(dllexport)
criteria Calc_criteria_eque_lines(bool system);

extern "C" __declspec(dllexport)
void Calc_regulator();

/* ======================== End of export functions ========================= */

/* =========================== Import functions: ============================ */


