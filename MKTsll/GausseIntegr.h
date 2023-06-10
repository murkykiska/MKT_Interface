#pragma once
#include <array>
#include <functional>

namespace GausseIntegr
{
	double integrate(double xLeft, double xRight, double yLeft, double yRight, std::function<double(double, double)> funct);
}

