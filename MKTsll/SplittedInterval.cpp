#include "SplittedInterval.h"
#include <iostream>

SplittedInterval::SplittedInterval(const std::vector<double>& basePoints, const IntervalForSplit& intervalForSplit)
{
	if (intervalForSplit.getFirstPointNum() >= basePoints.size())
		throw std::exception("First point number in interval cannot be equal or greater than points count");

	if (intervalForSplit.getSecondPointNum() >= basePoints.size())
		throw std::exception("Second point number in interval cannot be equal or greater than points count");

	double p1, p2;
	p1 = basePoints[intervalForSplit.getFirstPointNum()];
	p2 = basePoints[intervalForSplit.getSecondPointNum()];

	if (p2 == p1)
		throw std::exception("Interval fro splitting referes to equal values");

	splitInterval(p1, p2, intervalForSplit);
}

const std::vector<double>& SplittedInterval::getIntervalPoints() const
{
	return _points;
}

void SplittedInterval::splitInterval(double p1, double p2, const IntervalForSplit& intervalForSplit)
{
	int n = intervalForSplit.getSplitOptions().getIntervalsCount();
	double q = intervalForSplit.getSplitOptions().getSparseRatio();

	if (p1 > p2)
	{
		std::swap(p1, p2);
		q = 1 / q;
		std::cout << "Interval has reverse order of points (from bigger sto smaller value) therefore it was reversed.";
	}

	_points.reserve(n + 1);
	_points.emplace_back(p1);

	double h;
	if (q == 1)
	{
		h = (p2 - p1) / n;
		
		for (int i = 1; i < n; i++)
		{
			_points.emplace_back(p1 + i * h);
		}		
	}
	else
	{
		h = (p2 - p1) / (1 - std::pow(q, n)) * (1 - q);
		double previousValue = p1;
		for (int i = 0; i < n; i++)
		{
			_points.emplace_back(previousValue + h);
			previousValue += h;
			h *= q;
		}
	}
	_points.emplace_back(p2);
	
	_subIntervals.reserve(n);
	for (int i = 0; i < n; i++)
		_subIntervals.emplace_back(_points[i], _points[i + 1]);
	
}

const std::vector<IntervalRef>& SplittedInterval::getSubIntervals() const
{
	return _subIntervals;
}