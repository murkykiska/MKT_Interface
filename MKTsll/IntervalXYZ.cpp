#include "IntervalsXYZ.h"

IntervalRefX::IntervalRefX(const double& x1, const double& x2): IntervalRef(x1, x2)
{}
const double& IntervalRefX::getXLeftPoint() const
{
	return getLeftPoint();
}
const double& IntervalRefX::getXRightPoint() const
{
	return getRightPoint();
}


IntervalRefY::IntervalRefY(const double& y1, const double& y2) : IntervalRef(y1, y2)
{}

const double& IntervalRefY::getYLeftPoint() const
{
	return getLeftPoint();
}
const double& IntervalRefY::getYRightPoint() const
{
	return getRightPoint();
}


IntervalRefZ::IntervalRefZ(const double& z1, const double& z2) : IntervalRef(z1, z2)
{}

const double& IntervalRefZ::getZLeftPoint() const
{
	return getLeftPoint();
}
const double& IntervalRefZ::getZRightPoint() const
{
	return getRightPoint();
}