#pragma once
class IntervalRef
{
	const double* _leftPoint;
	const double* _rightPoint;

public:
	IntervalRef(const double& p1, const double& p2);
	const double& getLeftPoint() const;
	const double& getRightPoint() const;
	bool setLeftPoint(const double& p1);
	bool setRigntPoint(const double& p1);
	bool setIntervalRef(const double& p1, const double& p2);
	double getLength() const;
	bool isPointBelonsToInterval(double point) const;
};

