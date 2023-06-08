#pragma once
#include "IntervalRef.h"
class Rectangle
{
public:
	const IntervalRef& _interval1;
	const IntervalRef& _interval2;
	Rectangle(const IntervalRef& X, const IntervalRef& Y);
	double getSquare() const;
};

class FaceXY : public Rectangle
{
public:
	const IntervalRef& getIntervalX() const;
	const IntervalRef& getIntervalY() const;
	FaceXY(const IntervalRef& X, const IntervalRef& Y);
};

class FaceXZ: public Rectangle
{
public:
	const IntervalRef& getIntervalX() const;
	const IntervalRef& getIntervalZ() const;
	FaceXZ(const IntervalRef& X, const IntervalRef& Z);
};

class FaceYZ: public Rectangle
{
public:
	const IntervalRef& getIntervalY() const;
	const IntervalRef& getIntervalZ() const;
	FaceYZ(const IntervalRef& Y, const IntervalRef& Z);
};

