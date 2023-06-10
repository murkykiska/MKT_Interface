#include "MagnetIndocFunctor.h"

const double PI = 3.14159265358979323846;

MagnetIndocFunctor::MagnetIndocFunctor(double x, double z, double I, const MagnetElement& elem)
{
	this->x = x;
	this->z = z;
	_I = I;
	_elem = &elem;
}
double MagnetIndocFunctor::calcMagneticIndoctionFromElemPoint(double xElemPoint, double yElemPoint) const
{
	double coefX, coefZ;
	double r2 = calcCoefs_X_Y(xElemPoint, yElemPoint, coefX, coefZ);
	double r3 = pow(sqrt(r2), 3);
	double buf = _elem->get_pX() * coefX + _elem->get_pZ() * coefZ;
	return  _I / 4 / PI / r3 * buf;
}

double MagnetIndocFunctor::operator()(double xElemPoint, double yElemPoint) const
{
	return calcMagneticIndoctionFromElemPoint(xElemPoint, yElemPoint);
}

void MagnetIndocFunctor::setMagnetElement(const MagnetElement& elem)
{
	_elem = &elem;
}

void MagnetIndocFunctor::setI(double I)
{
	if (I < 0)
		throw std::exception("I for MagnetIndocFunctor cannot be less than 0");
	_I = I;
}


double MagnetIndocFunctorX::calcCoefs_X_Y(double xElemPoint, double zElemPoint, double& coefX, double& coefZ) const
{
	double x_ = x - xElemPoint;
	double z_ = z - zElemPoint;
	double r2 = x_ * x_ + z_ * z_;
	coefX = 3 * x_ * x_ / r2 - 1;
	coefZ = 3 * x_ * z_ / r2;
	return  r2;
}

double MagnetIndocFunctorZ::calcCoefs_X_Y(double xElemPoint, double zElemPoint, double& coefX, double& coefZ) const
{
	double x_ = x - xElemPoint;
	double z_ = z - zElemPoint;
	double r2 = x_ * x_ + z_ * z_;
	coefX = 3 * x_ * z_ / r2;
	coefZ = 3 * z_ * z_ / r2 - 1;
	return  r2;
}