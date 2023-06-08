#pragma once
#include "MagnetInfo.h"
#include "IntervalRef.h"
#include "Faces.h"
#include <fstream>
class MagnetElement : public MagnetInfo, public FaceXZ
{
public:
	MagnetElement(const IntervalRef& intervalX, const IntervalRef& intervalZ, double pX, double pZ);
	void writeInBinaryFile(std::ofstream& out) const;
};

