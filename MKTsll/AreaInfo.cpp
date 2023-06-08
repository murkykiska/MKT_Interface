#include "AreaInfo.h"

AreaInfo::AreaInfo(const int intervalNumX, const int intervalNumZ, const double pX, const double pZ)
{
	setIntervalNumX(intervalNumX);
	setIntervalNumZ(intervalNumZ);
	set_pX(pX);
	set_pZ(pZ);
}

AreaInfo::AreaInfo(std::istream& in)
{
	int intervalNumX, intervalNumZ;
	double pX, pZ;

	in >> intervalNumX;
	if (in.fail())
		throw std::exception("Cannot read interval number for X axes");
	in >> intervalNumZ;
	if (in.fail())
		throw std::exception("Cannot read interval number for Z axes");
	in >> pX;
	if (in.fail())
		throw std::exception("Cannot read pX value");
	in >> pZ;
	if (in.fail())
		throw std::exception("Cannot read pZ value");

	setIntervalNumX(intervalNumX);
	setIntervalNumZ(intervalNumZ);
	set_pX(pX);
	set_pZ(pZ);
}


AreaInfo::AreaInfo(libconfig::Setting& setting)
{
	int intervalNumX, intervalNumZ;
	double pX, pZ;

	setVarFromSetting(setting, intervalNumX, "IntervalXNum");
	setVarFromSetting(setting, intervalNumZ, "IntervalZNum");
	setVarFromSetting(setting, pX, "pX");
	setVarFromSetting(setting, pZ, "pZ");

	setIntervalNumX(intervalNumX);
	setIntervalNumZ(intervalNumZ);
	set_pX(pX);
	set_pZ(pZ);
}


void AreaInfo::setIntervalNumX(int intervalNumX)
{
	if (intervalNumX < 0)
		throw std::exception("Interval X number for Area cannot be less than 0");

	_intervalNumX = intervalNumX;

}

void AreaInfo::setIntervalNumZ(int intervalNumZ)
{
	if (intervalNumZ < 0)
		throw std::exception("Interval Y number for Area cannot be less than 0");

	_intervalNumZ = intervalNumZ;
}

int AreaInfo::getIntervalNumX() const
{
	return _intervalNumX;
}
int AreaInfo::getIntervalNumZ() const
{
	return _intervalNumZ;
}