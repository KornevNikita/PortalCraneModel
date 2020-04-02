#include "pch.h"
#include "Calc.h"
#include <fstream>
#include <complex>
using namespace std;

double M, m, l, R, g, h_fi, h_x, Beta, gamma, E; // параметры модели
double fi, dfi_dt, x, dx_dt; // начальное состояние
double dt, t_start, t_stop; //
int drawStCount; //
bool inDinamic; // параметры расчета
double xMax, yMax; // параметры масштаба
vector<double> reg(4); // регулятор

void mult(std::vector<std::vector<double>>& op1,
  std::vector<std::vector<double>>& op2,
  std::vector<std::vector<double>>& res)
{
  std::ofstream fout;
  fout.open("file.txt");
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

void f(const std::vector<double>& _X, std::vector<double>& _k)
{
  static double v; // regulator
  v = reg[0] * _X[0] + reg[1] * _X[1] + reg[2] * _X[2] + reg[3] * _X[3];
  _k[0] = _X[1]; // fi

  _k[1] = -_X[1] * (M + m) * h_fi / (M * m * l * l)
    - _X[0] * (M + m) * g / (M * l)
    + _X[3] * (gamma * E / (R * Beta) + h_x) / (M * l)
    - gamma / (M * R * l) * v; // dfi_dt

  _k[2] = _X[3]; // x

  _k[3] = _X[1] * h_fi / (M * l)
    + _X[0] * m * g / M
    - _X[3] * (gamma * E / (R * Beta) + h_x) / M
    + gamma / (M * R) * v; // dx_dt
}

point TDinModel::RK4()
{
  static std::vector<double> k1(4), k2(4), k3(4), k4(4), temp(4);
  for (int i = 1; i < drawStCount; i++) {
    f(X, k1);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k1[j] * 0.5 * dt;
    f(temp, k2);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k2[j] * 0.5 * dt;
    f(temp, k3);
    for (int j = 0; j < n; j++)
      temp[j] = X[j] + k3[j] * dt;
    f(temp, k4);
    for (int j = 0; j < n; j++)
      X[j] = X[j] + dt / 6 * (k1[j] + 2 * k2[j] + 2 * k3[j] + k4[j]);
  }
  X[4] += dt * drawStCount;
  point res(X[0], X[1], X[2], X[3], X[4]);
  return res;
}

void SetModelParams(double _M, double _m, double _l, double _R, double _g, double _h_fi, double _h_x, double _Beta, double _gamma, double _E)
{
  M = _M, m = _m, l = _l, R = _R, g = _g, h_fi = _h_fi, h_x = _h_x, Beta = _Beta, gamma = _gamma, E = _E;

  //regulator
  vector<complex<double>> p(4); // заданные корни
  vector<double> coeff_g(4); // коэффициенты полинома g0 + g1*p + g2*p^2 + g3*p^3 + p^4, которые будем искать (при p^4 нет коэфф, т.к. он = 1)
  p[0] = (1, 1);
  p[1] = (1, -1);
  p[2] = (-1, 1);
  p[3] = (-1, -1);
  calc_coeffs(p, coeff_g); // находим коэффициенты желаемого хар. полинома по заданным корням c помощью т. Виетта
  vector<double> a(4); // коэффициенты исходного полинома
  double a21 = -(M + m) * g / (M * l), // коэффициенты исходной матрицы А и вектора В
    a22 = -(M + m) * h_fi / (M * m * l * l),
    a24 = 1 / (M * l) * (gamma * E / (R * Beta) + h_x),
    a41 = m * g / M,
    a42 = h_fi / (M * l),
    a44 = -1 / M * (gamma * E / (R * Beta) + h_x);
  double b2 = -gamma / (M * R * l),
    b4 = gamma * (M * R);
  a[0] = 0;
  a[1] = a21 * a44 - a24 * a41;
  a[2] = a22 * a44 - a24 * a42 - a21;
  a[3] = -a22 - a44; // коэффициенты исходного полинома, полученного из |A - pE|
  vector<vector<double>> A(4, vector<double>(4)), B(4, vector<double>(1));
  vector<vector<double>> delta_A(4, vector<double>(4)), delta_B(4, vector<double>(1)); // A и B, приведенные к канон. виду

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

  // теперь надо посчитать R и delta_R, начнем с R
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

  //ищем P: = ~R * R^-1
  vector<vector<double>> P(4, vector<double>(4)), matrix_R_inv(4, vector<double>(4));
  matrix_R_inv = inv(matrix_R);
  mult(delta_R, matrix_R_inv, P);
  vector<vector<double>> P_T(4, vector<double>(4));
  //transp(P, P_T);

  // k = P_T * (a - g)
  vector<vector<double>> a_g(4);
  for (int i = 0; i < 4; i++)
    a_g[i][0] = a[i] - coeff_g[i];

  vector<vector<double>> temp_k(4, vector<double>(1));
  mult(P, a_g, temp_k);
  for (int i = 0; i < 4; i++)
    reg[i] = temp_k[i][0];
}

void SetInitParams(double _fi, double _dfi_dt, double _x, double _dx_dt)
{
  fi = _fi, dfi_dt = _dfi_dt, x = _x, dx_dt = _dx_dt;
}

void SetCalcParams(double _dt, double _t_start, double _t_stop, int _drawStCount, bool _inDinamic)
{
  dt = _dt, t_start = _t_start, t_stop = _t_stop, drawStCount = _drawStCount, inDinamic = _inDinamic;
}

void SetScaleParams(double _xMax, double _yMax)
{
  xMax = _xMax;
  yMax = _yMax;
}

int GetAllDrawPointsCount()
{
  return static_cast<int>((t_stop - t_start) / (drawStCount * dt));
}

//Выделение памяти под внутренний массив структуры TAllDrawPoints
//Размер массива должен быть уже заложен в поле drawCount
void InitAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->AllocMem(allDrawData->drawCount);
}
//Освобождение памяти от внутреннего массива в структуре TAllDrawPoints
void DeleteAllPointsArray(TAllDrawPoints* allDrawData)
{
  allDrawData->FreeMem();
}
//Заполнение массива всеми отображаемыми точками (count штук)
void GetAllDrawPoints(TAllDrawPoints* allDrawData)
{
  point drawPoint(fi, dfi_dt, x, dx_dt, t_start);
  TDinModel model(4, drawPoint);
  allDrawData->allDrawPoints[0] = drawPoint;

  for (int i = 1; i < allDrawData->drawCount; i++)
  {
    allDrawData->allDrawPoints[i] = model.RK4();
  }
}

//регулятор
void calc_coeffs(const vector<complex<double>>& p, vector<double>& g)
{
  // хар. полином - a0 + a1*p + a2*p^2 + a3*p^3 + a4*p^4
  complex<double> temp;
  temp = p[0] * p[1] * p[2] * p[3];
  g[0] = temp.real(); // p0*p1*p2*p3 = a0/a4
  temp = (-1.0) *
    (p[0] * p[1] * p[2] +
      p[0] * p[1] * p[3] +
      p[0] * p[2] * p[3] +
      p[1] * p[2] * p[3]); // p0*p1*p2 + p0*p1*p3 + p0*p2*p3 + p1*p2*p3 = (-a1)/a4
  g[1] = temp.real(); 
  temp = p[0] * p[1] +
    p[0] * p[2] +
    p[0] * p[3] +
    p[1] * p[2] +
    p[1] * p[3] +
    p[2] * p[3]; // p0*p1 + p0*p2 + p0*p3 + p1*p2 * p1*p3 + p2*p3 = a2/a4
  g[2] = temp.real();
  temp = (-1.0) * (p[0] + p[1] + p[2] + p[3]); // p0 + p1 + p2 + p3 = (-a3) / a4
  g[3] = temp.real();
  //g[4] = 1; // a4
}

// поиск обратной матрица
vector<vector<double>> inv(const vector<vector<double>>& _A)
{
  size_t n = _A.size();
  std::ofstream fout;
  fout.open("inv.txt", ios_base::trunc);

  // выведем исходную матрицу
  fout << "Ishodnaya:" << endl << "size = " << n << endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << _A[i][j] << "\t";
    fout << endl;
  }
  fout << endl;

  // нахождение определителя по Гауссу
  vector<vector<double>> A(_A);
  vector<vector<double>> E(n, vector<double>(n));
  for (int i = 0; i < n; i++)
    E[i][i] = 1;

  // прямой ход
  for (size_t i = 0; i < n; i++)
  {
    double pivot;
    pivot = A[i][i];

    if (pivot == 0) // если элемент на диагонали = 0, ищем в этом столбце не равный 0
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
      if (pivot == 0) // если не 0 не найден, ==> обратной матрицы нет
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

  // обратный
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

  fout << "Ishodnaya preobrazovannaya:" << endl;
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
}