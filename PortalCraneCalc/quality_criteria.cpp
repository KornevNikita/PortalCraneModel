#include "pch.h"
#include "quality_criteria.h"

#include <fstream>

extern vector<point> all_points;
extern vector<double> V;
extern double delta; // delta okresntost 0
extern double t_stop;

void calc_quality_criteria(criteria& c)
{
  calc_T_criterion(c.T);

  calc_H_criterion(c.H);

  calc_h1_criterion(c.h1);

  calc_h2_criterion(c.h2);

  calc_Vmax_criterion(c.Vmax);
}

void calc_T_criterion(double& T)
{
  point* back = &all_points.back();
  if (abs(back->x) > delta) // t.e. sistema tak i ne voshla v delta-okrestnost
    T = t_stop;
  else
  {
    size_t i = all_points.size() - 2;
    while (abs(all_points[i].x) <= delta && i > 0)
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

  fstream fout("h1_criterion.txt", ios::app);

  for (size_t i = 0; i < all_points.size() - 2; ++i)
  {
    first = all_points[i].x;
    second = all_points[i + 1].x;
    third = all_points[i + 2].x;

    if (second > 0 && second < first && second < third && second > max_loc_min)
      max_loc_min = second;

    if (second < first && second < third)
      fout << second << endl;
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
  Vmax = *std::max_element(V.begin(), V.end());
}