#include "quality_criteria.h"
#include "TDinModel.h"

#include <vector>
#include <algorithm>

extern vector<point> all_points;
extern vector<double> V;
extern double delta; // delta okresntost 0
extern double t_stop;

void calc_quality_criteria(double& T, double& H, double& h1, double& h2,double& Vmax)
{
  calc_T_criterion(T);

  calc_H_criterion(H);

  calc_h1_criterion(h1);

  calc_h2_criterion(h2);
}

void calc_T_criterion(double& T)
{
  if (all_points.back.x > delta) // t.e. sistema tak i ne voshla v delta-okrestnost
    T = t_stop;
  else
  {
    int i = all_points.size() - 2;
    while (all_points[i].x <= delta && i > 0)
      i--;

    T = all_points[i].t;
  }
}

void calc_H_criterion(double& H)
{
  double min_delta_t = 0;

  for (const auto& i : all_points)
    if (i.x < min_delta_t)
      min_delta_t = i.x;

  H = -1. * min_delta_t; // esli vse x > 0 to H ostanetsya = 0
}

void calc_h1_criterion(double& h1)
{
  double max_loc_min = 0;
  double first, second, third; // berem 3 tochki, e. srednyaya < 1 & 3 => loc min

  for (int i = 0; i < all_points.size() - 2; ++i)
  {
    first = all_points[i].x;
    second = all_points[i + 1].x;
    third = all_points[i + 2].x;

    if (second > 0 && second < first && second < third && second > max_loc_min)
      max_loc_min = second;
  }

  h1 = max_loc_min;
}

void calc_h2_criterion(double& h2)
{
  double max_loc_max = 0;
  double first, second, third; // berem 3 tochki, e. srednyaya > 1 & 3 => loc max

  for (int i = 0; i < all_points.size() - 2; ++i)
  {
    first = all_points[i].x;
    second = all_points[i + 1].x;
    third = all_points[i + 2].x;

    if (second > 0 && second > first && second > third && second > max_loc_max)
      max_loc_max = second;
  }

  h2 = max_loc_max;
}

void calc_Vmax_criterion(double& Vmax)
{
  Vmax = std::max_element(V.begin, V.end);
}