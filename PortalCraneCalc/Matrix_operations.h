#pragma once

#include <vector>
#include <fstream>

extern size_t dim;

// matrix multiplication:
void mult(std::vector<std::vector<double>>& op1,
  std::vector<std::vector<double>>& op2,
  std::vector<std::vector<double>>& res);

// matrix transposition
void transp(const std::vector<std::vector<double>>& P,
  std::vector<std::vector<double>>& P_T);

// Gauss method
void Gauss(std::vector<std::vector<double>>& _A, std::vector<double>& _b,
  std::vector<double>& res);

// matrix inversion
std::vector<std::vector<double>> inv(const std::vector<std::vector<double>>& A);
