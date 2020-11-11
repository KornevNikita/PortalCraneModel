#include "pch.h"

#include "TDinModel.h"
#include "Calc.h"

#include <fstream>
#include <complex>
#include <fstream>
using namespace std;

#define _USE_MATH_DEFINES
#include <math.h>

size_t dim = 4; // razmernost' sistemi
double M, m, l, R, g, h_fi, h_x, Beta, gamma, E; // parametri sistemi
double fi, dfi_dt, x, dx_dt; // nachalnie usloviya
double dt, t_start, t_stop; // parametri rasscheta
int drawStCount; // chislo tochek, kotorie budut otrisovani
bool inDinamic; // risovat v dinamike (poka ne realizovano)
vector<double> reg(dim); // regulator
vector<complex<double>> p(dim);  // zhelaemie korni 

vector<point> all_points; // massiv dlya hraneniya poluchennih tochek
double V = 0; // napryzhenie(regulator)

double a21, a22, a24, a41, a42, a44, b2, b4; // coefficinti matrici A & vectora b (forma Koshi)

void mult(std::vector<std::vector<double>>& op1,
  std::vector<std::vector<double>>& op2,
  std::vector<std::vector<double>>& res)
{
  std::ofstream fout;
  fout.open("mult.txt");
  double sum = 0;
  for (int i = 0; i < res.size(); i++)
  {
    for (int j = 0; j < res[0].size(); j++)
    {
      for (int k = 0; k < op1.size(); k++)
      {
        res[i][j] += op1[i][k] * op2[k][j];
      }
      fout << res[i][j] << " ";
    }
    fout << std::endl;
  }
}

void f(const std::vector<double>& _X, std::vector<double>& _k, bool system, bool reg_on)
{
  double v_t; // regulator
  if (system == true) // linear system
  {
    // sm. str. 118 (2014 god):
    // x = (x1, x2, x3, x4) = (fi, fi_dt, x - x*, x_dt),
    // k = (k1, k2, k3, k4) - coefficienti regulatora;
    // x_dt = Ax + bU(t), 
    // gde A - matrix:

    // (  0   1  0  0  )
    // ( a21 a22 0 a24 )
    // (  0   0  0  1  )
    // ( a41 a42 0 a44 );

    // b - vector: (0, b2, 0, b4),
    // U(t) = k^T * x -- scalar,
    // bU(t) = (0, b2 * U(t), 0, b4 * U(t));

    // Ax:
    // (  0   1  0  0  )     (   fi   )     (               fi_dt                 )
    // ( a21 a22 0 a24 )  *  ( fi_dt  )  =  ( a21 * fi + a22 * fi_dt + a24 * x_dt )
    // (  0   0  0  1  )     ( x - x* )     (                x_dt                 )
    // ( a41 a42 0 a44 )     (  x_dt  )     ( a41 * fi + a42 * fi_dt + a44 * x_dt )

    // U(t) = v(t) = k^T * x = k1 * fi + k2 * fi_dt + k3 * (x - x*) + k4 * x_dt:
    v_t = reg_on ? (reg[0] * _X[0]) + (reg[1] * _X[1]) + (reg[2] * _X[2]) + (reg[3] * _X[3]) : 0;

    // x_dt = Ax + bU(t):
    _k[0] = _X[1]; // fi_dt
    _k[1] = a21 * _X[0] + a22 * _X[1] + a24 * _X[3] + b2 * v_t; // (fi_dt)dt
    _k[2] = _X[3]; // x_dt
    _k[3] = a41 * _X[0] + a42 * _X[1] + a44 * _X[3] + b4 * v_t; // (x_dt)dt

    // =============================================================

    //_k[0] = _X[1]; // fi
    //_k[1] = (-1.0) * _X[0] * (M + m) * g / (M * l) +
    //  (-1.0) * _X[1] * (M + m) * h_fi / (M * m * l * l) +
    //  _X[3] * (gamma * E / (R * Beta) + h_x) / (M * l) +
    //  (-1.0) * gamma * v_t / (M * R * l); // dfi_dt
    //_k[2] = _X[3]; // x
    //_k[3] = _X[0] * m * g / M +
    //  _X[1] * h_fi / (M * l) +
    //  (-1.0) * _X[3] * (gamma * E / (R * Beta) + h_x) / M +
    //  gamma * v_t / (M * R); // dx_dt
  }

  else // non-linear system
  {
    double f_t; // sila tyagi
    v_t = reg_on ?
      reg[0] * ((_X[0] + M_PI) / (2 * M_PI) - M_PI) + reg[1] * _X[1] + reg[2] * _X[2] + reg[3] * _X[3] : 0;
    f_t = gamma / R * (v_t - E * _X[3] / Beta);

    _k[0] = _X[1]; // fi_dt

    _k[1] = f_t - h_x * _X[3]
      - (M + m) * (((-1.) * h_fi * _X[1]) / (m * l * cos(_X[0])) - g * tan(_X[0]))
      + m * l * _X[1] * _X[1] * sin(_X[0]);
    _k[1] /= m * l * cos(_X[0]) - (M + m) * l / cos(_X[0]);

    _k[2] = _X[3]; // x

    _k[3] = (-1.) * h_fi * _X[1] / (m * l * cos(_X[0]))
      - l * _k[1] / cos(_X[0])
      - g * tan(_X[0]);
  }
  V = v_t;
}

void SetModelParams(double _M, double _m, double _l, double _R, double _g, 
  double _h_fi, double _h_x, double _Beta, double _gamma, double _E,
  double _p1_re, double _p1_im, double _p2_re, double _p2_im,
  double _p3_re, double _p3_im, double _p4_re, double _p4_im)
{
  std::ofstream fout;
  fout.open("korni.txt", ios_base::trunc);
  fout << "Vhodnie parametri:" << endl
    << _p1_re << " " << _p1_im << endl << _p2_re << " " << _p2_im << endl
    << _p3_re << " " << _p3_im << endl << _p4_re << " " << _p4_im << endl;

  M = _M, m = _m, l = _l, R = _R, g = _g,
    h_fi = _h_fi, h_x = _h_x, Beta = _Beta, gamma = _gamma, E = _E;
  
  complex<double> p1(_p1_re, _p1_im), p2(_p2_re, _p2_im),
  p3(_p3_re, _p3_im), p4(_p4_re, _p4_im);

  p[0] = p1, p[1] = p2, p[2] = p3, p[3] = p4;

  fout << p[0] << endl << p[1] << endl << p[2] << endl << p[3] << endl;

  init_matrix_A();

  calc_regulator();
}

void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt)
{
  fi = _fi, dfi_dt = _dfi_dt, x = _x, dx_dt = _dx_dt;
}

void SetCalcParams(double _dt, double _t_start, double _t_stop, int _drawStCount, bool _inDinamic)
{
  dt = _dt, t_start = _t_start, t_stop = _t_stop, drawStCount = _drawStCount, inDinamic = _inDinamic;
}

int GetAllDrawPointsCount()
{
  return static_cast<int>((t_stop - t_start) / (drawStCount * dt));
}

void InitAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->AllocMem(allDrawData->drawCount);
}

void DeleteAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->FreeMem();
}

void GetAllDrawPoints(TAllDrawPoints* allDrawData, bool system, bool reg_on)
{
  point drawPoint(fi, dfi_dt, x, dx_dt, t_start);
  TDinModel model(4, drawPoint);

  all_points.clear();

  allDrawData->allDrawPoints[0] = drawPoint;
  all_points.push_back(drawPoint);

  std::ofstream fout1, fout2;
  fout1.open("values.txt", ios_base::trunc);
  fout1 << drawPoint.fi << " " << drawPoint.dfi_dt << " " << drawPoint.x << " " << drawPoint.dx_dt << endl;
  fout2.open("values_in_local_vault.txt", ios_base::trunc);
  fout2 << all_points[0].fi << " " << all_points[0].dfi_dt << " " << all_points[0].x << " " << all_points[0].dx_dt << endl;

  for (unsigned i = 1; i < allDrawData->drawCount; i++)
  {
    drawPoint = model.RK4(system, reg_on);

    all_points.push_back(drawPoint); // polozhili tochku v local hranilishe
    allDrawData->allDrawPoints[i] = drawPoint; // v c#

    fout1 << drawPoint.fi << " " << drawPoint.dfi_dt << " " << drawPoint.x << " " << drawPoint.dx_dt << endl;
    fout2 << all_points[i].fi << " " << all_points[i].dfi_dt << " " << all_points[i].x << " " << all_points[i].dx_dt << endl;
  }
}

void calc_coeffs(const vector<complex<double>>& p, vector<double>& g)
{
  if (dim == 2)
  {
    complex<double> temp = p[0] * p[1];
    g[0] = temp.real();
    temp = (-1.0) * (p[0] + p[1]);
    g[1] = temp.real();
  }

  if (dim == 4)
  {
    // g0 + g1*p + g2*p^2 + g3*p^3 + p^4
    complex<double> temp;

    // p0 * p1 * p2 * p3 = g0 / g4, (g4 = 1)
    temp = p[0] * p[1] * p[2] * p[3];

    g[0] = temp.real();

    // p0 * p1 * p2 + p0 * p1 * p3 + 
    // p0 * p2 * p3 + p1 * p2 * p3 = (-g1) / g4, (g4 = 1)
    temp = (-1.0) *
      (p[0] * p[1] * p[2] +
        p[0] * p[1] * p[3] +
        p[0] * p[2] * p[3] +
        p[1] * p[2] * p[3]);

    g[1] = temp.real();

    // p0 * p1 + p0 * p2 + p0 * p3 + 
    // p1 * p2 + p1 * p3 + p2 * p3 = g2 / g4, (g4 = 1)
    temp = p[0] * p[1] +
      p[0] * p[2] +
      p[0] * p[3] +
      p[1] * p[2] +
      p[1] * p[3] +
      p[2] * p[3];

    g[2] = temp.real();

    temp = (-1.0) * (p[0] + p[1] + p[2] + p[3]);

    g[3] = temp.real();  // p0 + p1 + p2 + p3 = (-a3) / a4
  }
}

vector<vector<double>> inv(const vector<vector<double>>& _A)
{
  std::ofstream fout;
  fout.open("inv.txt", ios_base::trunc);

  size_t n = _A.size();

  fout << "Ishodnaya:" << endl << "size = " << n << endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << _A[i][j] << "\t";
    fout << endl;
  }
  fout << endl;

  vector<vector<double>> A(_A);
  vector<vector<double>> E(n, vector<double>(n));
  for (int i = 0; i < n; i++)
    E[i][i] = 1;

  // ïðÿìîé õîä
  for (size_t i = 0; i < n; i++)
  {
    double pivot;
    pivot = A[i][i];

    if (pivot == 0) // åñëè ýëåìåíò íà äèàãîíàëè = 0, èùåì â ýòîì ñòîëáöå íå ðàâíûé 0
    {
      for (int j = 0; j < n; j++)
      {
        if (A[j][i] != 0)
        {
          pivot = A[j][i];
          vector<double> temp = A[j];
          A[j] = A[i];
          A[i] = temp;

          temp = E[j];
          E[j] = E[i];
          E[i] = temp;
          break;
        }
      }
      if (pivot == 0) 
        throw("no inv matrix");
    }

    for (size_t j = 0; j < n; j++)
    {
      A[i][j] /= pivot;
      E[i][j] /= pivot;
    }

    for (size_t j = i + 1; j < n; j++)
    {
      pivot = A[j][i];
      for (int k = 0; k < n; k++)
      {
        A[j][k] -= A[i][k] * pivot;
        E[j][k] -= E[i][k] * pivot;
      }
    }

    fout << "i = " << i << endl;
    fout << "==================================================" << endl
      << "A:" << endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << A[i][j] << "\t";
      fout << endl;
    }
    fout << endl;

    fout << "==================================================" << endl
      << "E:" << endl;
    fout << "i = " << i << endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << E[i][j] << "\t";
      fout << endl;
    }
    fout << endl
      << "==================================================" << endl;
  }

  // obratniy hod
  fout << endl << "Obratniy hod:" << endl;
  for (int i = static_cast<int>(n) - 1; i > 0; i--)
  {
    for (int j = i - 1; j >= 0; j--)
    {
      double pivot = A[j][i];
      fout << "pivot = " << pivot << endl;
      for (int k = 0; k < n; k++)
      {
        fout << E[j][k] << " " << E[i][k] * pivot << " " << E[j][k] - E[i][k] * pivot << endl;
        A[j][k] -= A[i][k] * pivot;
        E[j][k] -= E[i][k] * pivot;
        fout << E[j][k] << endl;
      }
    }
    fout << "i = " << i << endl;
    fout << "==================================================" << endl
      << "A:" << endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << A[i][j] << "\t";
      fout << endl;
    }
    fout << endl;

    fout << "==================================================" << endl
      << "E:" << endl;
    fout << "i = " << i << endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << E[i][j] << "\t";
      fout << endl;
    }
    fout << endl
      << "==================================================" << endl;
  }

  fout << "Ishodnaya preobrazovannaya k edinichnoy:" << endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << A[i][j] << "\t";
    fout << endl;
  }
  fout << endl;

  fout << "Result:" << endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << E[i][j] << "\t";
    fout << endl;
  }
  
  fout << endl << "Proverka A * A^-1 = E:" << endl;
  vector <vector<double>> res(dim, vector<double>(dim)), proverka(_A);

  mult(proverka, E, res);

  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << res[i][j] << "\t";
    fout << endl;
  }

  fout.close();
  return E;
}

void transp(const vector<vector<double>>& P, vector<vector<double>>& P_T)
{

  std::ofstream fout;
  fout.open("transp.txt", ios_base::trunc);

  int n = static_cast<int>(P.size());
  for (int i = 0; i < P.size(); i++)
    for (int j = 0; j < P.size(); j++)
      P_T[i][j] = P[j][i];

  fout << "Ishodnaya matrica:" << endl;
  for (int i = 0; i < dim; i++)
  {
    for (int j = 0; j < dim; j++)
    {
      fout << P[i][j] << " ";
    }
    fout << endl;
  }

  fout << endl;

  fout << "Transponirovannaya matrica:" << endl;
  for (int i = 0; i < dim; i++)
  {
    for (int j = 0; j < dim; j++)
    {
      fout << P_T[i][j] << " ";
    }
    fout << endl;
  }
}

void calc_regulator()
{
  std::ofstream fout;
  fout.open("calc_reg.txt", ios_base::trunc);
  fout << "Zadannie korni p:" << endl;
  for (int i = 0; i < dim; ++i)
    fout << p[i].real() << " + " << p[i].imag() << "i" << endl;
  fout << endl;

  vector<vector<double>> A(dim, vector<double>(dim)), B(dim, vector<double>(1)); // sistema v forme Koshi (str. 118)
  vector<double> a(dim); // |A - pE| = a(l) = l^4 + l^3 * a3 + l^2 * a2 + l * a1 + a0
  vector<vector<double>> delta_A(dim, vector<double>(dim)), delta_B(dim, vector<double>(1)); // normalnaya kanon. forma

  A[0][1] = 1;
  A[1][0] = a21;
  A[1][1] = a22;
  A[1][3] = a24;
  A[2][3] = 1;
  A[3][0] = a41;
  A[3][1] = a42;
  A[3][3] = a44;

  B[1][0] = b2;
  B[3][0] = b4;

  fout << "A|b:" << endl;
  for (int i = 0; i < dim; ++i)
  {
    fout << "(";
    for (int j = 0; j < dim - 1; ++j)
      fout << A[i][j] << " ";
    fout << A[i][dim - 1] << " | " << B[i][0] << ")" << endl;
  }

  a[0] = 0;
  a[1] = a21 * a44 - a24 * a41;
  a[2] = a22 * a44 - a24 * a42 - a21;
  a[3] = (-1.0) * a44 - a22;

  fout << "|A - pE| = a(l) = l^4 + l^3 * a3 + l^2 * a2 + l * a1 + a0:" << endl;
  for (int i = 0; i < dim; ++i)
    fout << "a" << i << " = " << a[i] << endl;

  delta_A[0][1] = 1;
  delta_A[1][2] = 1;
  delta_A[2][3] = 1;
  delta_A[3][0] = (-1.0) * a[0];
  delta_A[3][1] = (-1.0) * a[1];
  delta_A[3][2] = (-1.0) * a[2];
  delta_A[3][3] = (-1.0) * a[3];

  delta_B[3][0] = 1;

  fout << endl << "~A|~b:" << endl;
  for (int i = 0; i < dim; ++i)
  {
    fout << "(";
    for (int j = 0; j < dim - 1; ++j)
      fout << delta_A[i][j] << " ";
    fout << delta_A[i][dim - 1] << " | " << delta_B[i][0] << ")" << endl;
  }


  //// dlya 2-mernoy sistemi:
  //double a11 = 1, a12 = 3, 
  //  a21 = 2, a22 = 5, 
  //  b1 = 1, b2 = 2;
  //fout << "A:" << endl
  //  << "(" << a11 << " " << a12 << ")" << endl
  //  << "(" << a21 << " " << a22 << ")" << endl
  //  << "b: " << "(" << b1 << ", " << b2 << ")" << endl << endl;

 /* a[0] = a11 * a22 - a21 * a12;
  a[1] = (-1.) * a11 - a22;
  fout << "|A - pE| = a(l) = l^4 + l^3 * a3 + l^2 * a2 + l * a1 + a0:" << endl;
  for (int i = 0; i < dim; ++i)
    fout << "a" << i << " = " << a[i] << endl;
  fout << endl;*/

  vector<double> coeff_g(dim); // koefficienti har. polinoma pri zadannih kornyah p1-p4: g0 + g1*p + g2*p^2 + g3*p^3 + p^4

  calc_coeffs(p, coeff_g); // ishem coeff_g po T. Vieta
  fout << endl << "Koefficienti dlya korney p:" << endl;
  for (int i = 0; i < dim; ++i)
    fout << "a*[" << i << "] = " << coeff_g[i] << endl;
  fout << endl;

  /*A[0][0] = a11;
  A[0][1] = a12;
  A[1][0] = a21;
  A[1][1] = a22;
  B[0][0] = b1;
  B[1][0] = b2;

  delta_A[0][1] = 1;
  delta_A[1][0] = (-1.) * a[0];
  delta_A[1][1] = (-1.) * a[1];
  delta_B[1][0] = 1;*/

  // R = (B; AB; A^2B; A^3B)
  // ~R = (~B; ~A~B; ~A^2~B; ~A^3~B);
  vector<vector<double>> matrix_R(dim, vector<double>(dim)), delta_R(dim, vector<double>(dim)); // matrici upravlyaemosti v novom i starom bazisah
  vector<vector<double>> tempvec(dim, vector<double>(1)), tempmatrix1(dim, vector<double>(dim)), tempmatrix2(dim, vector<double>(dim)),
    delta_tempvec(dim, vector<double>(1)), delta_tempmatrix1(dim, vector<double>(dim)), delta_tempmatrix2(dim, vector<double>(dim));

  // B, ~B
  for (int i = 0; i < dim; i++)
  {
    matrix_R[i][0] = B[i][0];
    delta_R[i][0] = delta_B[i][0];
  }

  // AB, ~A~B
  mult(A, B, tempvec);
  mult(delta_A, delta_B, delta_tempvec);

  fout << "A * b:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << tempvec[i][0] << ", ";
  fout << tempvec[dim - 1][0] << ")" << endl;

  fout << endl << "~A * ~b:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << delta_tempvec[i][0] << ", ";
  fout << delta_tempvec[dim - 1][0] << ")" << endl;

  for (int i = 0; i < dim; i++)
  {
    matrix_R[i][1] = tempvec[i][0];
    delta_R[i][1] = delta_tempvec[i][0];
  }

  // A^2B, ~A^2~B
  mult(A, A, tempmatrix1);
  mult(delta_A, delta_A, delta_tempmatrix1);

  mult(tempmatrix1, B, tempvec);
  mult(delta_tempmatrix1, delta_B, delta_tempvec);

  for (int i = 0; i < dim; i++)
  {
    matrix_R[i][2] = tempvec[i][0];
    delta_R[i][2] = delta_tempvec[i][0];
  }

  fout << endl << "A^2 * b:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << tempvec[i][0] << ", ";
  fout << tempvec[dim - 1][0] << ")" << endl;

  fout << endl << "~A^2 * ~b:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << tempvec[i][0] << ", ";
  fout << delta_tempvec[dim - 1][0] << ")" << endl;

  // A^3B, ~A^3~B
  mult(tempmatrix1, A, tempmatrix2);
  mult(delta_tempmatrix1, delta_A, delta_tempmatrix2);

  mult(tempmatrix2, B, tempvec);
  mult(delta_tempmatrix2, delta_B, delta_tempvec);

  for (int i = 0; i < dim; i++)
  {
    matrix_R[i][3] = tempvec[i][0];
    delta_R[i][3] = delta_tempvec[i][0];
  }

  fout << endl << "A^3 * b:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << tempvec[i][0] << ", ";
  fout << tempvec[dim - 1][0] << ")" << endl;

  fout << endl << "~A^3 * ~b:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << tempvec[i][0] << ", ";
  fout << delta_tempvec[dim - 1][0] << ")" << endl;

  fout << endl << "~U:" << endl;
  for (int i = 0; i < dim; ++i)
  {
    fout << "(";
    for (int j = 0; j < dim - 1; ++j)
      fout << delta_R[i][j] << " ";
    fout << delta_R[i][dim - 1] << ")" << endl;
  }


  fout << endl << "U:" << endl;
  for (int i = 0; i < dim; ++i)
  {
    fout << "(";
    for (int j = 0; j < dim - 1; ++j)
      fout << matrix_R[i][j] << " ";
    fout << matrix_R[i][dim - 1] << ")" << endl;
  }

  // R^T * k = ~R^T * ~k
  // Naydem pravuyu chast ~R^T * ~k, gde ~k = (a - a*):
  vector<vector<double>> delta_R_tr(dim, vector<double>(dim)),
    delta_k(dim, vector<double>(1));

  // ~R^T
  transp(delta_R, delta_R_tr);
  fout << endl << "~U^T:" << endl;
  for (int i = 0; i < dim; ++i)
  {
    fout << "(";
    for (int j = 0; j < dim - 1; ++j)
      fout << delta_R_tr[i][j] << " ";
    fout << delta_R_tr[i][dim - 1] << ")" << endl;
  }

  // ~k = a - a*
  fout << endl << "~k = a - a*" << endl;
  for (int i = 0; i < dim; ++i)
    delta_k[i][0] = a[i] - coeff_g[i];
  fout << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << delta_k[i][0] << ", ";
  fout << delta_k[dim - 1][0] << ")" << endl;

  // ~R^T * ~k
  vector<vector<double>> delta_R_tr_delta_k(dim, vector<double>(1));
  mult(delta_R_tr, delta_k, delta_R_tr_delta_k);
  fout << endl << "~U^T * ~k:" << endl
    << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << delta_R_tr_delta_k[i][0] << ", ";
  fout << delta_R_tr_delta_k[dim - 1][0] << ")" << endl;

  // Naydem levuyu chast:
  vector<vector<double>> R_tr(dim, vector<double>(dim));

  // R^T:
  transp(matrix_R, R_tr);

  // Gauss: R^T * k = ~R^T * ~k
  vector<double> d_R_tr_d_k(4);
  for (int i = 0; i < dim; ++i)
    d_R_tr_d_k[i] = delta_R_tr_delta_k[i][0];
  Gauss(R_tr, d_R_tr_d_k, reg);

  fout << endl << "regulator:" << endl
    << "(";
  for (int i = 0; i < dim - 1; i++)
    fout << reg[i] << ", ";
  fout << reg[dim - 1] << ")" << endl;
}

void init_matrix_A()
{
  a21 = (-1.) * (M + m) * g / (M * l),
    a22 = (-1.) * (M + m) * h_fi / (M * m * l * l),
    a24 = (gamma * E / (R * Beta) + h_x) / (M * l),
    a41 = m * g / M,
    a42 = h_fi / (M * l),
    a44 = (-1.) * (gamma * E / (R * Beta) + h_x) / M,
    b2 = (-1.) * gamma / (M * R * l),
    b4 = gamma / (M * R);
}

void Gauss(vector<vector<double>>& _A, vector<double>& _b, vector<double>& res)
{
  vector<vector<double>> A(_A); 
  vector<double>b(_b);
  std::ofstream fout;
  fout.open("Gauss.txt", ios_base::trunc);

  int n = static_cast<int>(_A.size());

  // pryamoy hod:
  fout << "size = " << n << endl << "Ishodnaya:" << endl << endl;
  for (int i = 0; i < n; i++)
  {
    fout << "(";
    for (int j = 0; j < n - 1; j++)
      fout << A[i][j] << " ";
    fout << A[i][n - 1] << " | " << b[i] << ")" << endl;
  }

  double pivot;
  for (int i = 0; i < n; i++)
  {
    pivot = A[i][i];
    if (pivot == 0)
    {
      for (int j = 0; j < n; j++)
      {
        if (A[j][i] != 0)
        {
          pivot = A[j][i];
          vector<double> temp = A[j];
          A[j] = A[i];
          A[i] = temp;

          double temp_double;
          temp_double = b[j];
          b[j] = b[i];
          b[i] = temp_double;
          break;
        }
      }
      if (pivot == 0)
        res[i] = 0;
    }

    for (int j = 0; j < n; j++)
      A[i][j] /= pivot;
    b[i] /= pivot;

    for (int j = i + 1; j < n; j++)
    {
      pivot = A[j][i];
      for (int k = 0; k < n; k++)
        A[j][k] -= A[i][k] * pivot;
      b[j] -= b[i] * pivot;
    }

    fout << endl;
    for (int i = 0; i < dim; ++i)
    {
      fout << "(";
      for (int j = 0; j < dim - 1; ++j)
        fout << A[i][j] << " ";
      fout << A[i][dim - 1] << " | " << b[i] << ")" << endl;
    }
  }
  
  // Obratniy hod:
  res[n - 1] = b[n - 1];
  for (int i = n - 2; i >= 0; --i)
  {
    double sum = 0;
    for (int j = i + 1; j < n; ++j)
      sum += A[i][j] * res[j];
    fout << "sum = " << sum << endl;
    res[i] = b[i] - sum;
  }

  fout << endl << "res:" << endl;
    fout << "(";
    for (int i = 0; i < dim - 1; ++i)
      fout << res[i] << ", ";
    fout << res[n - 1] << ")" << endl;
  }