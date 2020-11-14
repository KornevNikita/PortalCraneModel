#pragma once

#include "quality_criteria.h"
#include "TDinModel.h"

#include <vector>
#include <algorithm>

void calc_quality_criteria(criteria& c);
void calc_T_criterion(double& T);
void calc_H_criterion(double& H);
void calc_h1_criterion(double& h1);
void calc_h2_criterion(double& h2);
void calc_Vmax_criterion(double& Vmax);
