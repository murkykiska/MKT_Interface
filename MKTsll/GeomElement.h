#pragma once
#include "IntervalRef.h"

class Cuboid
{
private:
	const IntervalRef* _intervalX;
	const IntervalRef* _intervalY;
	const IntervalRef* _intervalZ;
public:
	const IntervalRef* getIntervalX() const;
	const IntervalRef* getIntervalY() const;
	const IntervalRef* getIntervalZ() const;

	void setIntervalX(const IntervalRef& _intervalX);
	void setIntervalY(const IntervalRef& _intervalY);
	void setIntervalZ(const IntervalRef& _intervalZ);

	Cuboid(const IntervalRef& _intervalX, const IntervalRef& _intervalY, const IntervalRef& _intervalZ);
	bool isPointInElement(double x, double y, double z) const;
};

