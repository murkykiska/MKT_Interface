#include "MagnetElement.h"

MagnetElement::MagnetElement(const IntervalRef& intervalX, const IntervalRef& intervalZ, double pX, double pZ) : FaceXZ(intervalX, intervalZ), MagnetInfo(pX, pZ) {}

void MagnetElement::writeInBinaryFile(std::ofstream& out) const
{
	if (out.fail())
		throw std::exception("Cannot write magnetic element in binary file");
	out.write((char*)&getIntervalX().getLeftPoint(), sizeof(double));
	out.write((char*)&getIntervalX().getRightPoint(), sizeof(double));
	out.write((char*)&getIntervalZ().getLeftPoint(), sizeof(double));
	out.write((char*)&getIntervalZ().getRightPoint(), sizeof(double));
	out.write((char*)&getIntervalX().getLeftPoint(), sizeof(double));
	double px = get_pX();
	double pz = get_pZ();
	out.write((char*)&px, sizeof(double));
	out.write((char*)&pz, sizeof(double));
}
