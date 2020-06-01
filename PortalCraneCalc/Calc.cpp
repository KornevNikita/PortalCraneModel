#include "pch.h"
#include "Calc.h"
#include <fstream>
#include <complex>
#include <fstream>
using namespace std;

#define _USE_MATH_DEFINES
#include <math.h>

double M, m, l, R, g, h_fi, h_x, Beta, gamma, E; // parametri sistemi
double fi, dfi_dt, x, dx_dt; // nachalnie usloviya
double dt, t_start, t_stop; // parametri rasscheta
int drawStCount; // chislo tochek, kotorie budut otrisovani
bool inDinamic; // risovat v dinamike (poka ne realizovano)
vector<double> reg(4); // regulator
vector<complex<double>> p(4);  // zhelaemie korni 

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
    v_t = (reg_on == true) ? (reg[0] * _X[0]) + (reg[1] * _X[1]) + (reg[2] * _X[2]) + (reg[3] * _X[3]) : 1;

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
    v_t = (reg_on == true) ?
      reg[0] * ((_X[0] + M_PI) / 2 * M_PI - M_PI) + reg[1] * _X[1] + reg[2] * _X[2] + reg[3] * _X[3] : 1;
    f_t = gamma / R * (v_t - E * _X[3] / Beta);

    _k[0] = _X[1]; // fi
    _k[2] = _X[3]; // x

    _k[3] = (f_t - h_x * _X[3] + (h_fi * _X[1] * cos(_X[0]) / l) +
      m * g * cos(_X[0]) * sin(_X[0]) + m * l * _X[1] * _X[1] * sin(_X[0])) /
      (M + m * (1 - pow(cos(_X[0]), 2))
        );

    _k[1] = -h_fi * _X[1] - m * l * _k[3] * cos(_X[0]) - m * g * l * sin(_X[0]) / (m * l * l);
  }
}

point TDinModel::RK4(bool system, bool reg_on)
{
  std::vector<double> k1(4), k2(4), k3(4), k4(4), temp(4);
  for (int i = 0; i < drawStCount; i++) {
    f(X, k1, system, reg_on);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k1[j] * 0.5 * dt;
    f(temp, k2, system, reg_on);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k2[j] * 0.5 * dt;
    f(temp, k3, system, reg_on);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k3[j] * dt;
    f(temp, k4, system, reg_on);
    for (int j = 0; j < n; j++)
      X[j] = X[j] + dt / 6 * (k1[j] + 2 * k2[j] + 2 * k3[j] + k4[j]);
  }

  X[4] += dt * drawStCount;
  point res(X[0], X[1], X[2], X[3], X[4]);
  return res;
}

void SetModelParams(double _M, double _m, double _l, double _R, double _g, 
  double _h_fi, double _h_x, double _Beta, double _gamma, double _E,
  double _p1_re, double _p1_im, double _p2_re, double _p2_im,
  double _p3_re, double _p3_im, double _p4_re, double _p4_im)
{
  M = _M, m = _m, l = _l, R = _R, g = _g,
    h_fi = _h_fi, h_x = _h_x, Beta = _Beta, gamma = _gamma, E = _E;
  
  p[0] = (_p1_re, _p1_im);
  p[1] = (_p2_re, _p2_im);
  p[2] = (_p3_re, _p3_im);
  p[3] = (_p4_re, _p4_im);

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

//Âûäåëåíèå ïàìÿòè ïîä âíóòðåííèé ìàññèâ ñòðóêòóðû TAllDrawPoints
//Ðàçìåð ìàññèâà äîëæåí áûòü óæå çàëîæåí â ïîëå drawCount
void InitAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->AllocMem(allDrawData->drawCount);
}
//Îñâîáîæäåíèå ïàìÿòè îò âíóòðåííåãî ìàññèâà â ñòðóêòóðå TAllDrawPoints
void DeleteAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->FreeMem();
}
//Çàïîëíåíèå ìàññèâà âñåìè îòîáðàæàåìûìè òî÷êàìè (count øòóê)
void GetAllDrawPoints(TAllDrawPoints* allDrawData, bool system, bool reg_on)
{
  point drawPoint(fi, dfi_dt, x, dx_dt, t_start);
  TDinModel model(4, drawPoint);
  allDrawData->allDrawPoints[0] = drawPoint;
  std::ofstream fout;
  fout.open("values.txt", ios_base::trunc);
  for (int i = 1; i < allDrawData->drawCount; i++)
  {
    drawPoint = model.RK4(system, reg_on);
    allDrawData->allDrawPoints[i] = drawPoint;
    fout << drawPoint.fi << " " << drawPoint.dfi_dt << " " << drawPoint.x << " " << drawPoint.dx_dt << endl;
  }
}

void calc_coeffs(const vector<complex<double>>& p, vector<double>& g)
{
  // g0 + g1 * p + g2 * p ^ 2 + g3 * p ^ 3 + p ^ 4
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

// ïîèñê îáðàòíîé ìàòðèöà
vector<vector<double>> inv(const vector<vector<double>>& _A)
{
  size_t n = _A.size();
  std::ofstream fout;
  fout.open("inv.txt", ios_base::trunc);

  // âûâåäåì èñõîäíóþ ìàòðèöó
  fout << "Ishodnaya:" << endl << "size = " << n << endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << _A[i][j] << "\t";
    fout << endl;
  }
  fout << endl;

  // íàõîæäåíèå îïðåäåëèòåëÿ ïî Ãàóññó
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
          break;
        }
      }
      if (pivot == 0) // åñëè íå 0 íå íàéäåí, ==> îáðàòíîé ìàòðèöû íåò
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
  }

  // îáðàòíûé
  for (int i = static_cast<int>(n) - 1; i > 0; i--)
  {
    for (int j = i - 1; j >= 0; j--)
    {
      double pivot = A[j][i];
      for (int k = 0; k < n; k++)
      {
        A[j][k] -= A[i][k] * pivot;
        E[j][k] -= E[i][k] * pivot;
      }
    }
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
  fout.close();
  return E;
}

void transp(const vector<vector<double>>& P, vector<vector<double>>& P_T)
{
  int n = static_cast<int>(P.size());
  for (int i = 0; i < P.size(); i++)
    for (int j = 0; j < P.size(); j++)
      P_T[i][j] = P[j][i];

  std::ofstream fout;
  fout.open("transp.txt", ios_base::trunc);

  fout << "Ishodnaya matrica:" << endl;
  for (int i = 0; i < 4; i++)
  {
    for (int j = 0; j < 4; j++)
    {
      fout << P[i][j] << " ";
    }
    fout << endl;
  }

  fout << endl;

  fout << "Transponirovannaya matrica:" << endl;
  for (int i = 0; i < 4; i++)
  {
    for (int j = 0; j < 4; j++)
    {
      fout << P_T[i][j] << " ";
    }
    fout << endl;
  }
}

void calc_regulator()
{
  vector<double> coeff_g(4); // koefficienti har. polinoma pri zadannih kornyah p1-p4: g0 + g1*p + g2*p^2 + g3*p^3 + p^4 
  calc_coeffs(p, coeff_g); // ishem coeff_g po T. Vieta
  vector<double> a(4); // elementi matrici ~A

  double a21 = -(M + m) * g / (M * l), // koefficienti systemi v forme Koshi 
    a22 = -(M + m) * h_fi / (M * m * l * l),
    a24 = 1 / (M * l) * (gamma * E / (R * Beta) + h_x),
    a41 = m * g / M,
    a42 = h_fi / (M * l),
    a44 = -1 / M * (gamma * E / (R * Beta) + h_x);

  double b2 = -gamma / (M * R * l),
    b4 = gamma / (M * R);

  a[0] = 0;
  a[1] = a21 * a44 - a24 * a41;
  a[2] = a22 * a44 - a24 * a42 - a21;
  a[3] = -a22 - a44; // |A - pE|

  vector<vector<double>> A(4, vector<double>(4)), B(4, vector<double>(1));
  vector<vector<double>> delta_A(4, vector<double>(4)), delta_B(4, vector<double>(1)); // A è B, ïðèâåäåííûå ê êàíîí. âèäó

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

  delta_A[0][1] = 1;
  delta_A[1][2] = 1;
  delta_A[2][3] = 1;
  delta_A[3][0] = -a[0];
  delta_A[3][1] = -a[1];
  delta_A[3][2] = -a[2];
  delta_A[3][3] = -a[3];
  delta_B[3][0] = 1;

  // R = (B; AB; A^2B; A^3B)
  // ~R = (~B; ~A~B; ~A^2~B; ~A^3~B);
  vector<vector<double>> matrix_R(4, vector<double>(4)), delta_R(4, vector<double>(4)); // regulator
  vector<vector<double>> tempvec(4, vector<double>(1)), tempmatrix1(4, vector<double>(4)), tempmatrix2(4, vector<double>(4)),
    delta_tempvec(4, vector<double>(1)), delta_tempmatrix1(4, vector<double>(4)), delta_tempmatrix2(4, vector<double>(4));

  // B, ~B
  for (int i = 0; i < 4; i++)
  {
    matrix_R[i][0] = B[i][0];
    delta_R[i][0] = delta_B[i][0];
  }

  // AB, ~A~B
  mult(A, B, tempvec);
  mult(delta_A, delta_B, delta_tempvec);
  for (int i = 0; i < 4; i++)
  {
    matrix_R[i][1] = tempvec[i][0];
    delta_R[i][1] = delta_tempvec[i][0];
  }

  // A^2B, ~A^2~B
  mult(A, A, tempmatrix1);
  mult(delta_A, delta_A, delta_tempmatrix1);
  mult(tempmatrix1, B, tempvec);
  mult(delta_tempmatrix1, delta_B, delta_tempvec);
  for (int i = 0; i < 4; i++)
  {
    matrix_R[i][2] = tempvec[i][0];
    delta_R[i][2] = delta_tempvec[i][0];
  }

  // A^3B, ~A^3~B
  mult(tempmatrix1, A, tempmatrix2);
  mult(delta_tempmatrix1, delta_A, delta_tempmatrix2);
  mult(tempmatrix2, B, tempvec);
  mult(delta_tempmatrix2, delta_B, delta_tempvec);
  for (int i = 0; i < 4; i++)
  {
    matrix_R[i][3] = tempvec[i][0];
    delta_R[i][3] = delta_tempvec[i][0];
  }

  // P^-1: = ~R * R^-1
  vector<vector<double>> P(4, vector<double>(4)), matrix_R_inv(4, vector<double>(4));
  matrix_R_inv = inv(matrix_R);
  mult(delta_R, matrix_R_inv, P);
  vector<vector<double>> P_T(4, vector<double>(4));
  //transp(P, P_T);

  vector<vector<double>> a_g(4, vector<double>(1)), temp_reg(4, vector<double>(1));
  for (int i = 0; i < 4; i++)
    a_g[i][0] = a[i] - coeff_g[i];

  mult(P, a_g, temp_reg);
  for (int i = 0; i < 4; i++)
    reg[i] = temp_reg[i][0];

  std::ofstream fout;
  fout.open("reg.txt", ios_base::trunc);
  for (int i = 0; i < 4; i++)
    fout << reg[i] << " ";

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