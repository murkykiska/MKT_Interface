#pragma once
#include "MagnetElement.h"
class MagnetIndocFunctor
{	
	const MagnetElement* _elem;
	double _I;
public:
	double x;
	double z;
	virtual double calcCoefs_X_Y(double xElemPoint, double zElemPoint, double& coeffX, double&coeffZ) const = 0;

	double calcMagneticIndoctionFromElemPoint(double xElemPoint, double yElemPoint) const;
	double operator()(double xElemPoint, double yElemPoint) const;
	void setMagnetElement(const MagnetElement& elem);
	void setI(double I);

	MagnetIndocFunctor(double x, double z, double I, const MagnetElement& elem);
};

class MagnetIndocFunctorX : public MagnetIndocFunctor
{
	using MagnetIndocFunctor::MagnetIndocFunctor;
	double calcCoefs_X_Y(double xElemPoint, double zElemPoint, double& coefX, double& coefZ) const override;
};

class MagnetIndocFunctorZ : public MagnetIndocFunctor
{
	using MagnetIndocFunctor::MagnetIndocFunctor;
	double calcCoefs_X_Y(double xElemPoint, double zElemPoint, double& coefX, double& coefZ) const override;
};
