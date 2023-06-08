#pragma once
#include "IntervalRef.h"

class IntervalRefX: private IntervalRef
{
public:
	IntervalRefX(const double& x1, const double& x2);
	const double& getXLeftPoint() const;
	const double& getXRightPoint() const;
	using IntervalRef::isPointBelonsToInterval;
	using IntervalRef::getLength;
};

class IntervalRefY : private IntervalRef
{
public:
	IntervalRefY(const double& y1, const double& y2);
	const double& getYLeftPoint() const;
	const double& getYRightPoint() const;
	using IntervalRef::isPointBelonsToInterval;
	using IntervalRef::getLength;
};

class IntervalRefZ : private IntervalRef
{
public:
	IntervalRefZ(const double& z1, const double& z2);
	const double& getZLeftPoint() const;
	const double& getZRightPoint() const;
	using IntervalRef::isPointBelonsToInterval;
	using IntervalRef::getLength;
};