#pragma once
#include "MeshInfo.h"
#include "MagnetIndocFunctor.h"
#include "DEFINES.h"
class MagnetismDirectTask
{
private:
	const MeshInfo* _mesh;
	template<class MagnIndocFonctType> requires std::is_base_of<MagnetIndocFunctor, MagnIndocFonctType>::value
	double calcMagneticIndoction(double x, double z) const;
	//{
	//	double b = 0;
	//	double xMidElem, zMidElem;
	//	const std::vector<MagnetElement>& _magneticElements = _mesh->getMagneticElements();
	//	for (const MagnetElement& elem : _magneticElements)
	//	{
	//		MagnIndocFonctType magIndocFunctor(x, z, _mesh->getI(), elem);
	//		xMidElem = (elem.getIntervalX().getLeftPoint() + elem.getIntervalX().getRightPoint()) / 2;
	//		zMidElem = (elem.getIntervalZ().getLeftPoint() + elem.getIntervalZ().getRightPoint()) / 2;
	//		b += elem.getSquare() * magIndocFunctor(xMidElem, zMidElem);
	//	}

	//	return b;
	//}
public:
	MagnetismDirectTask(const MeshInfo& mesh);
	void setMesh(const MeshInfo& mesh);
	double calcMagneticIndoctionX(double x, double z) const;
	double calcMagneticIndoctionY(double x, double z) const;
};

