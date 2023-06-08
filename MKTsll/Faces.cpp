#include "Faces.h"

Rectangle::Rectangle(const IntervalRef& interval1, const IntervalRef& interval2) : _interval1(interval1), _interval2(interval2)
{}

double Rectangle::getSquare() const
{
	return _interval1.getLength() * _interval2.getLength();
}

FaceXY::FaceXY(const IntervalRef& X, const IntervalRef& Y) : Rectangle(X,Y)
{}

const IntervalRef& FaceXY::getIntervalX() const
{
	return _interval1;
}
const IntervalRef& FaceXY::getIntervalY() const
{
	return _interval2;
}

FaceXZ::FaceXZ(const IntervalRef& X, const IntervalRef& Z) : Rectangle(X,Z)
{}

const IntervalRef& FaceXZ::getIntervalX() const
{
	return _interval1;
}
const IntervalRef& FaceXZ::getIntervalZ() const
{
	return _interval2;
}

FaceYZ::FaceYZ(const IntervalRef& Y, const IntervalRef& Z) : Rectangle(Y,Z)
{}

const IntervalRef& FaceYZ::getIntervalY() const
{
	return _interval1;
}
const IntervalRef& FaceYZ::getIntervalZ() const
{
	return _interval2;
}
