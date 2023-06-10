#include "GausseIntegr.h"

namespace GausseIntegr
{
	const double root2_1 = sqrt((3.0 - 2 * sqrt(6.0 / 5.0)) / 7.0);
	const double root2_2 = sqrt((3.0 + 2 * sqrt(6.0 / 5.0)) / 7.0);
	const double weight1 = (18.0 + sqrt(30.0)) / 36.0;
	const double weight2 = (18.0 - sqrt(30.0)) / 36.0;

	const std::array<double, 4> GaussePoints = { root2_1, -root2_1, root2_2, -root2_2 };
	const std::array<double, 4> GausseWights = { weight1, weight1, weight2, weight2 };

	double integrate(double xLeft, double xRight, double yLeft, double yRight, std::function<double(double, double)> funct)
	{
		if (xRight < xLeft)
			std::swap(xRight, xLeft);
		if (yRight < yLeft)
			std::swap(yRight, yLeft);
		double xMid = (xLeft + xRight) / 2, yMid = (yLeft + yRight) / 2;
		double hxHalf = (xRight - xLeft) / 2;
		double hyHalf = (yRight - yLeft) / 2;

		double itegrSum = 0;
		for (int iy = 0; iy < GaussePoints.size(); iy++)
		{
			double y = yMid + GaussePoints[iy] * hyHalf;
			double buf = 0;
			for (int ix = 0; ix < GaussePoints.size(); ix++)
			{
				buf += GausseWights[ix] * funct(xMid + GaussePoints[ix] * hxHalf, y);
			}
			itegrSum += GausseWights[iy] * buf;
		}

		return hxHalf * hyHalf * itegrSum;
	}

}
