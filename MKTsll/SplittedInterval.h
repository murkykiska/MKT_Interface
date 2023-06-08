#pragma once
#include <vector>
#include "IntervalForSplit.h"
#include "IntervalRef.h"

class SplittedInterval
{
private:
	std::vector<double> _points;
	std::vector<IntervalRef> _subIntervals;
	void splitInterval(double p1, double p2, const IntervalForSplit& intervalForSplit);
public:
	SplittedInterval(const std::vector<double>& basePoints, const IntervalForSplit& intervalForSplit);
	const std::vector<double>& getIntervalPoints() const;
	const std::vector<IntervalRef>& getSubIntervals() const;
};

