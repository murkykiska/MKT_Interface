#include "MeshInfo.h"
#include "Area.h"
#include <algorithm>

libconfig::Config& prepareConfig(libconfig::Config& config)
{
	config.setAutoConvert(true);
	return config;
}

MeshInfo::MeshInfo(libconfig::Config& config) : MeshInfo(getSettingFromConfig(prepareConfig(config), "Mesh")) {}


//MeshInfo::MeshInfo(std::string configFileName) : MeshInfo(getConfigFromFile(configFileName)) {}


void MeshInfo::checkBasePointsArr(std::vector<double>& p, std::string_view pName)
{
	if (!std::is_sorted(p.begin(), p.end()))
	{
		std::string exeptionMSG = "Array for ";
		exeptionMSG.append(pName.data());
		exeptionMSG += " base point should be sorted";
		throw std::exception(exeptionMSG.c_str());
	}
		
	if (std::adjacent_find(p.begin(), p.end()) != p.end())
	{
		std::string exeptionMSG = "Array for ";
		exeptionMSG.append(pName.data());
		exeptionMSG += " base point should not contain dublicates";
		throw std::exception(exeptionMSG.c_str());
	}
}

MeshInfo::MeshInfo(libconfig::Setting& setting)
{
	std::vector<double> _x;
	std::vector<double> _z;
	std::vector<IntervalForSplit> _intervalsXForSplit;
	std::vector<IntervalForSplit> _intervalsZForSplit;
	std::vector<AreaInfo> areasInfo;

	libconfig::Setting& basePointsSetting = getSubSetting(setting, "BasePoints");
	setArrayFromConfigArray(basePointsSetting, _x, "X");
	checkBasePointsArr(_x, "X");
	setArrayFromConfigArray(basePointsSetting, _z, "Z");
	checkBasePointsArr(_z, "Z");
	libconfig::Setting& intervalSetting = getSubSetting(setting, "Intervals");
	setArrayFromConfigList(intervalSetting, _intervalsXForSplit, "X");
	setArrayFromConfigList(intervalSetting, _intervalsZForSplit, "Z");
	setArrayFromConfigList(setting, areasInfo, "Areas");
	
	double I;
	setVarFromSetting(setting, I, "I");
	setI(I);


	splitIntervals(_intervalsX, _intervalsXForSplit, _x);
	splitIntervals(_intervalsZ, _intervalsZForSplit, _z);

	countMagneticElements(areasInfo);
}

MeshInfo::MeshInfo(const std::vector<double> _x, const std::vector<double> _z, const std::vector<IntervalForSplit> _intervalsXForSplit, const std::vector<IntervalForSplit> _intervalsZForSplit, double I, const std::vector<AreaInfo> areasInfo)
{
	splitIntervals(_intervalsX, _intervalsXForSplit, _x);
	splitIntervals(_intervalsZ, _intervalsZForSplit, _z);
	countMagneticElements(areasInfo);
	setI(I);
}

void MeshInfo::setI(double I)
{
	if (I < 0)
		throw std::exception("I cannot be less than 0");
	_I = I;
}

double MeshInfo::getI() const
{
	return _I;
}
void MeshInfo::splitIntervals(std::vector<SplittedInterval>& splittedIntervals, const std::vector<IntervalForSplit>& intervalsForSplit, const std::vector<double>& points)
{
	splittedIntervals.reserve(intervalsForSplit.size());
	for (const IntervalForSplit& intervalForSplit : intervalsForSplit)
		splittedIntervals.emplace_back(points, intervalForSplit);
	
}
void MeshInfo::countMagneticElements(const std::vector<AreaInfo>& areasInfo)
{
	std::vector<Area> areas;
	areas.reserve(areasInfo.size());

	for (const AreaInfo& areaInfo : areasInfo)
		areas.emplace_back(_intervalsX, _intervalsZ, areaInfo);

	int elementsCount = 0;
	for (const Area& area : areas)
		elementsCount += area.getElementsCount();

	_magneticElements.clear();
	_magneticElements.reserve(elementsCount);

	for (const Area& area : areas)
		area.getElements(_magneticElements);
}

const std::vector<MagnetElement>& MeshInfo::getMagneticElements() const
{	
	return _magneticElements;
}

const std::vector<SplittedInterval>& MeshInfo::getIntervalsX() const
{
	return _intervalsX;
}
const std::vector<SplittedInterval>& MeshInfo::getIntervalsZ() const
{
	return _intervalsZ;
}

void MeshInfo::change_px(double px, int elementNum)
{
	if (elementNum < 0)
		throw std::exception("Magnetic element num for changing px cannot be less than 0");
	if (elementNum >= _magneticElements.size())
		throw std::exception("Magnetic element num for changing px cannot be larger than magnetic elements count");
	_magneticElements[elementNum].set_pX(px);
}
void MeshInfo::change_pz(double pz, int elementNum)
{
	if (elementNum < 0)
		throw std::exception("Magnetic element num for changing pz cannot be less than 0");
	if (elementNum >= _magneticElements.size())
		throw std::exception("Magnetic element num for changing pz cannot be larger than magnetic elements count");
	_magneticElements[elementNum].set_pZ(pz);
}

void MeshInfo::writeMagneticElementsInBinaryFile(std::string_view fileName) const
{
	std::ofstream out;
	out.open(fileName.data(), std::ios::binary);
	for (const MagnetElement& element : _magneticElements)
		element.writeInBinaryFile(out);
}