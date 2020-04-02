#pragma once
#include <vector>
#include <complex>
using namespace std;

struct point 
{
  double fi;
  double dfi_dt;
  double x;
  double dx_dt;
  double t;

  point() : fi(0), dfi_dt(0), x(0), dx_dt(0), t(0) {};
  point(double _fi, double _dfi_dt, double _x, double _dx_dt, double _t) : fi(_fi), dfi_dt(_dfi_dt), x(_x), dx_dt(_dx_dt), t(_t) {};
};

class TDinModel {
  int n; // размерость системы
  std::vector<double> X; // текущее состояние

public:
  TDinModel(int _n, const point& p) : n(_n) 
  {
    X.push_back(p.fi);
    X.push_back(p.dfi_dt);
    X.push_back(p.x);
    X.push_back(p.dx_dt);
    X.push_back(p.t);
  }

  point RK4();
};

struct TAllDrawPoints {
  point* allDrawPoints;
  int drawCount;
  void AllocMem(int _drawCount)
  {
    allDrawPoints = new point[_drawCount];
    drawCount = _drawCount;
  }
  void FreeMem() {
    delete allDrawPoints;
    allDrawPoints = 0;
  }
};

void mult(std::vector<std::vector<double>>& op1,
  std::vector<std::vector<double>>& op2,
  std::vector<std::vector<double>>& res);

void f(const std::vector<double>& _X, std::vector<double>& _k);
void calc_coeffs(const vector<complex<double>>& p, vector<double>& g);
vector<vector<double>> inv(const vector<vector<double>>& A);
void transp(const vector<vector<double>>& P, vector<complex<double>>& P_T);

extern "C" __declspec(dllexport)
void SetModelParams(double _M, double _m, double _l, double _R, double _g, double _h_fi, double _h_x, double _B, double _gamma, double _E);

extern "C" __declspec(dllexport)
void SetCalcParams(double _dt, double _t_start, double _t_stop, int _drawStCount, bool _inDinamic);

extern "C" __declspec(dllexport)
void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt);

extern "C" __declspec(dllexport)
void SetScaleParams(double _xMax, double _yMax);

extern "C" __declspec(dllexport)
int GetAllDrawPointsCount();

extern "C" __declspec(dllexport)
void InitAllPointsArray(TAllDrawPoints* allDrawData);

extern "C" __declspec(dllexport)
void DeleteAllPointsArray(TAllDrawPoints* allDrawData);

extern "C" __declspec(dllexport)
void GetAllDrawPoints(TAllDrawPoints* ptrAllDrawPoints);