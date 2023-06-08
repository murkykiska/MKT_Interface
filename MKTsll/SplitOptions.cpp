#include "SplitOptions.h"

SplitOptions::SplitOptions()
{
	_intervalsCount = 1;
	_sparseRatio = 1;
}

SplitOptions::SplitOptions(int intervalsCount, double sparseRatio)
{
	setSplitOptions(intervalsCount, sparseRatio);
}

SplitOptions::SplitOptions(std::istream& in)
{
	readData(in);
}

SplitOptions::SplitOptions(libconfig::Setting& setting)
{
	setSplitOptions(setting);
}

void SplitOptions::setSplitOptions(libconfig::Setting& setting)
{
	int intervalsCount;
	double sparseRatio;
	setVarFromSetting(setting, intervalsCount, "IntervalsCount");
	setVarFromSetting(setting, sparseRatio, "SparseRatio");
	setSplitOptions(intervalsCount, sparseRatio);
}

void SplitOptions::checkSparseRatio(double sparseRatio)
{
	if (sparseRatio == 0)
		throw std::exception("Sparse ratio cannot be equal to 0");
}
void SplitOptions::checkIntervalsCount(double intervalsCount)
{
	if (intervalsCount < 1)
		throw std::exception("Intervals count cannot be less than 1");
}
void SplitOptions::setIntervalsCount(int intervalsCount)
{
	checkIntervalsCount(intervalsCount);
	_intervalsCount = intervalsCount;
}

void SplitOptions::setSparseRatio(double sparseRatio)
{
	checkSparseRatio(sparseRatio);
	if (sparseRatio < 0)
		sparseRatio = -1 / (sparseRatio);

	_sparseRatio = sparseRatio;
}

int SplitOptions::getIntervalsCount() const
{
	return _intervalsCount;
}
double SplitOptions::getSparseRatio() const
{
	return _sparseRatio;
}

void SplitOptions::setSplitOptions(int intervalsCount, double sparseRatio)
{
	checkIntervalsCount(intervalsCount);
	checkSparseRatio(sparseRatio);
	setIntervalsCount(intervalsCount);
	setSparseRatio(sparseRatio);
}

std::istream& SplitOptions::readData(std::istream& in)
{
	int intervalsCount;
	double sparseRatio;
	in >> intervalsCount;

	if (in.fail())
	{
		std::string errMSG = "Cannot read count of intervals";
		throw new std::exception(errMSG.c_str());
	}

	in >> sparseRatio;

	if (in.fail())
	{
		std::string errMSG = "Cannot read Sparse Ratio";
		throw new std::exception(errMSG.c_str());
	}

	setSplitOptions(intervalsCount, sparseRatio);
	return in;
}

std::string&& SplitOptions::getTypeDataName()
{
	return "SplitOptions";
}
