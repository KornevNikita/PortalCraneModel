#include "pch.h"

#include "Matrix_operations.h"

// matrix multiplication:
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

// matrix transposition
void transp(const std::vector<std::vector<double>>& P,
  std::vector<std::vector<double>>& P_T)
{
  std::ofstream fout("transp.txt", std::ios_base::trunc);

  fout << "Ishodnaya matrica:" << std::endl;
  for (int i = 0; i < dim; i++)
  {
    for (int j = 0; j < dim; j++)
      fout << P[i][j] << " ";

    fout << std::endl;
  }
  fout << std::endl;

  int n = static_cast<int>(P.size());
  for (int i = 0; i < P.size(); i++)
    for (int j = 0; j < P.size(); j++)
      P_T[i][j] = P[j][i];

  fout << "Transponirovannaya matrica:" << std::endl;
  for (int i = 0; i < dim; i++)
  {
    for (int j = 0; j < dim; j++)
      fout << P_T[i][j] << " ";

    fout << std::endl;
  }
}

// Gauss method
void Gauss(std::vector<std::vector<double>>& _A, std::vector<double>& _b,
  std::vector<double>& res)
{
  std::vector<std::vector<double>> A(_A);
  std::vector<double>b(_b);
  std::ofstream fout("Gauss.txt", std::ios_base::trunc);

  int n = static_cast<int>(_A.size());

  // pryamoy hod:
  fout << "size = " << n << std::endl << "Ishodnaya:" << std::endl << std::endl;
  for (int i = 0; i < n; i++)
  {
    fout << "(";
    for (int j = 0; j < n - 1; j++)
      fout << A[i][j] << " ";
    fout << A[i][n - 1] << " | " << b[i] << ")" << std::endl;
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
          std::vector<double> temp = A[j];
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

    fout << std::endl;
    for (int i = 0; i < dim; ++i)
    {
      fout << "(";
      for (int j = 0; j < dim - 1; ++j)
        fout << A[i][j] << " ";
      fout << A[i][dim - 1] << " | " << b[i] << ")" << std::endl;
    }
  }

  // Obratniy hod:
  res[n - 1] = b[n - 1];
  for (int i = n - 2; i >= 0; --i)
  {
    double sum = 0;
    for (int j = i + 1; j < n; ++j)
      sum += A[i][j] * res[j];
    fout << "sum = " << sum << std::endl;
    res[i] = b[i] - sum;
  }

  fout << std::endl << "res:" << std::endl;
  fout << "(";
  for (int i = 0; i < dim - 1; ++i)
    fout << res[i] << ", ";
  fout << res[n - 1] << ")" << std::endl;
}

// matrix inversion
std::vector<std::vector<double>> inv(const std::vector<std::vector<double>>& _A)
{
  std::ofstream fout("inv.txt", std::ios_base::trunc);

  size_t n = _A.size();

  fout << "Ishodnaya:" << std::endl << "size = " << n << std::endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << _A[i][j] << "\t";
    fout << std::endl;
  }
  fout << std::endl;

  std::vector<std::vector<double>> A(_A);
  std::vector<std::vector<double>> E(n, std::vector<double>(n));
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
          std::vector<double> temp = A[j];
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

    fout << "i = " << i << std::endl;
    fout << "==================================================" << std::endl
      << "A:" << std::endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << A[i][j] << "\t";
      fout << std::endl;
    }
    fout << std::endl;

    fout << "==================================================" << std::endl
      << "E:" << std::endl;
    fout << "i = " << i << std::endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << E[i][j] << "\t";
      fout << std::endl;
    }
    fout << std::endl
      << "==================================================" << std::endl;
  }

  // obratniy hod
  fout << std::endl << "Obratniy hod:" << std::endl;
  for (int i = static_cast<int>(n) - 1; i > 0; i--)
  {
    for (int j = i - 1; j >= 0; j--)
    {
      double pivot = A[j][i];
      fout << "pivot = " << pivot << std::endl;
      for (int k = 0; k < n; k++)
      {
        fout << E[j][k] << " " << E[i][k] * pivot << " " << E[j][k] - E[i][k] * pivot << std::endl;
        A[j][k] -= A[i][k] * pivot;
        E[j][k] -= E[i][k] * pivot;
        fout << E[j][k] << std::endl;
      }
    }
    fout << "i = " << i << std::endl;
    fout << "==================================================" << std::endl
      << "A:" << std::endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << A[i][j] << "\t";
      fout << std::endl;
    }
    fout << std::endl;

    fout << "==================================================" << std::endl
      << "E:" << std::endl;
    fout << "i = " << i << std::endl;
    for (int i = 0; i < n; i++)
    {
      for (int j = 0; j < n; j++)
        fout << E[i][j] << "\t";
      fout << std::endl;
    }
    fout << std::endl
      << "==================================================" << std::endl;
  }

  fout << "Ishodnaya preobrazovannaya k edinichnoy:" << std::endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << A[i][j] << "\t";
    fout << std::endl;
  }
  fout << std::endl;

  fout << "Result:" << std::endl;
  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << E[i][j] << "\t";
    fout << std::endl;
  }

  fout << std::endl << "Proverka A * A^-1 = E:" << std::endl;
  std::vector <std::vector<double>> res(dim, std::vector<double>(dim)), proverka(_A);

  mult(proverka, E, res);

  for (int i = 0; i < n; i++)
  {
    for (int j = 0; j < n; j++)
      fout << res[i][j] << "\t";
    fout << std::endl;
  }

  fout.close();
  return E;
}