#include "GeomElement.h"

Cuboid::Cuboid(const IntervalRef& intervalX, const IntervalRef& intervalY, const IntervalRef& intervalZ): _intervalX(&intervalX), _intervalY(&intervalY), _intervalZ(&intervalX) {}

const IntervalRef* Cuboid::getIntervalX() const
{
	return _intervalX;
}
const IntervalRef* Cuboid::getIntervalY() const
{
	return _intervalY;
}
const IntervalRef* Cuboid::getIntervalZ() const
{
	return _intervalZ;
}

void Cuboid::setIntervalX(const IntervalRef& intervalX)
{
	_intervalX = &intervalX;
}
void Cuboid::setIntervalY(const IntervalRef& intervalY)
{
	_intervalY = &intervalY;
}
void Cuboid::setIntervalZ(const IntervalRef& intervalZ)
{
	_intervalZ = &intervalZ;
}

bool Cuboid::isPointInElement(double x, double y, double z) const
{
	return	_intervalX->isPointBelonsToInterval(x) && 
			_intervalY->isPointBelonsToInterval(y) && 
			_intervalZ->isPointBelonsToInterval(z);
}