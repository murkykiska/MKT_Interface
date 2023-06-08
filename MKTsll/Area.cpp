#include "Area.h"

Area::Area(const std::vector<SplittedInterval>& intervalsX, const std::vector<SplittedInterval>& intervalsZ, const AreaInfo& areaInfo)
{
	if (areaInfo.getIntervalNumX() >= intervalsX.size())
		throw std::exception("Interval number for X cannot be bigger or equal to intervals count");

	if (areaInfo.getIntervalNumZ() >= intervalsZ.size())
		throw std::exception("Interval number for Z cannot be bigger or equal to intervals count");

	_intervalX = &intervalsX[areaInfo.getIntervalNumX()];
	_intervalZ = &intervalsZ[areaInfo.getIntervalNumZ()];

	set_pX(areaInfo.get_pX());
	set_pZ(areaInfo.get_pZ());
}

const SplittedInterval& Area::getIntervalX() const
{
	return *_intervalX;
}
const SplittedInterval& Area::getIntervalZ() const
{
	return *_intervalZ;
}

//std::vector<GeomElement>* getGeometryElements();
void Area::getElements(std::vector<MagnetElement>& geometryElements) const
{
	for (const IntervalRef& intervalZ : _intervalZ->getSubIntervals())
		for (const IntervalRef& intervalX : _intervalX->getSubIntervals())
			geometryElements.emplace_back(intervalX, intervalZ, get_pX(), get_pZ());
		
}

int Area::getElementsCount() const
{
	return _intervalX->getSubIntervals().size() * _intervalZ->getSubIntervals().size();
}