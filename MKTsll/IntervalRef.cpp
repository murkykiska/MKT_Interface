#include "IntervalRef.h"
#include <exception>
const double& min(const double &p1, const double &p2)
{
	return p1 > p2 ? p2 : p1;
}

const double& max(const double& p1, const double& p2)
{
	return p1 < p2 ? p2 : p1;
}

IntervalRef::IntervalRef(const double& p1, const double& p2)
{
	if (!setIntervalRef(p1, p2))
		throw std::exception("Point cannot be equal in interval");
}

const double& IntervalRef::getLeftPoint() const
{
	return *_leftPoint;
}

const double& IntervalRef::getRightPoint() const
{
	return *_rightPoint;
}

bool IntervalRef::setLeftPoint(const double& leftPoint)
{
	if (leftPoint >= *_rightPoint)
		return false;
	_leftPoint = &leftPoint;
	return true;
}

bool IntervalRef::setRigntPoint(const double& rightPoint)
{
	if (rightPoint <= *_leftPoint)
		return false;
	_rightPoint = &rightPoint;
	return true;
}

bool IntervalRef::setIntervalRef(const double& p1, const double& p2)
{
	if (p1 == p2)
		return false;
	_leftPoint = &min(p1, p2);
	_rightPoint = &max(p1, p2);
	return true;
}

double IntervalRef::getLength() const
{
	return *_rightPoint - *_leftPoint;
}

bool IntervalRef::isPointBelonsToInterval(double point) const
{
	return !(point < *_leftPoint || point > *_rightPoint);
}
