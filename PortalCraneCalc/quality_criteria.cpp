#include "quality_criteria.h"
#include "TDinModel.h"

#include <vector>

extern vector<point> all_points;
extern double delta; // delta okresntost 0
extern double t_stop;

void calc_quality_criteria(double& T, double& H, double& h1, double& h2, double& Vmax)
{
  // T
  if (all_points.back.x > delta) // t.e. sistema tak i ne voshla v delta-okrestnost
    T = t_stop;
  else
  {
    int i = all_points.size() - 2;
    while (all_points[i].x <= delta)
      i--;
    T = all_points[i].t;
  }

  // H
  double min_x = 0;
  for (const auto& i : all_points)
    if (i.x < 0 && i.x < min_x)
      min_x = i.x;
  H = min_x; // esli vse x > 0 to H ostanetsya = 0

  // h1
  // h2



}