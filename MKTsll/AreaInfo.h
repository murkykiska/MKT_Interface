#pragma once
#include <array>
#include <sstream>
#include "LibconfigDeluxe.h"
#include "MagnetInfo.h"

typedef unsigned char BoundCount_t;

class AreaInfo : public MagnetInfo
{
private:
	int _intervalNumX;
	int _intervalNumZ;

public:
	AreaInfo() = default;

	AreaInfo(const int _intervalNumX, const int _intervalNumZ, const double pX, const double pZ);

	AreaInfo(std::istream& in);

	AreaInfo(libconfig::Setting& setting);

	void setIntervalNumX(int intervalNumX);
	void setIntervalNumZ(int intervalNumY);

	int getIntervalNumX() const;
	int getIntervalNumZ() const;
};

