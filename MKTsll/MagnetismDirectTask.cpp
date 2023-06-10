#include "MagnetismDirectTask.h"
#include "MagnetismDirectTask.h"
#include "GausseIntegr.h"

const double PI = 3.14159265358979323846;

MagnetismDirectTask::MagnetismDirectTask(const MeshInfo& mesh)
{
	setMesh(mesh);
}

void MagnetismDirectTask::setMesh(const MeshInfo& mesh)
{
	_mesh = &mesh;
}

template<class MagnIndocFonctType> requires std::is_base_of<MagnetIndocFunctor, MagnIndocFonctType>::value
double MagnetismDirectTask::calcMagneticIndoction(double x, double z) const
{	
	double b = 0;
	double xMidElem, zMidElem;
	const std::vector<MagnetElement>& _magneticElements = _mesh->getMagneticElements();
	for (const MagnetElement& elem : _magneticElements)
	{
		MagnIndocFonctType magIndocFunctor(x, z, _mesh->getI(), elem);
		xMidElem = (elem.getIntervalX().getLeftPoint() + elem.getIntervalX().getRightPoint()) / 2;
		zMidElem = (elem.getIntervalZ().getLeftPoint() + elem.getIntervalZ().getRightPoint()) / 2;
		double r2 = pow(x - xMidElem, 2) + pow(z - zMidElem, 2);
		double elemDiag2 = pow(elem.getIntervalX().getLength(), 2) * pow(elem.getIntervalZ().getLength(), 2);
#ifdef GAUSSE_COUNT
		if (elemDiag2 / r2 > 0.1)
			b += GausseIntegr::integrate(elem.getIntervalX().getLeftPoint(), elem.getIntervalX().getRightPoint(), elem.getIntervalZ().getLeftPoint(), elem.getIntervalZ().getRightPoint(), magIndocFunctor);		
		else	
			b += elem.getSquare() * magIndocFunctor(xMidElem, zMidElem);
#else
		b += elem.getSquare() * magIndocFunctor(xMidElem, zMidElem);
#endif

	}

	return b;
	
}
/*
double MagnetismDirectTask::calcMagneticIndoction(MagnetIndocFunctor& magnIndocFunc) const
{
	double b = 0;
	double xMidElem, zMidElem;
	const std::vector<MagnetElement>& _magneticElements = _mesh->getMagneticElements();
	for (const MagnetElement& elem : _magneticElements)
	{
		magnIndocFunc.setMagnetElement(elem);
		xMidElem = (elem.getIntervalX().getLeftPoint() + elem.getIntervalX().getRightPoint()) / 2;
		zMidElem = (elem.getIntervalZ().getLeftPoint() + elem.getIntervalZ().getRightPoint()) / 2;
		b += elem.getSquare() * magnIndocFunc(xMidElem, zMidElem);
	}

	return b;
}
*/
//template<class MagnIndocFunctType> requires std::is_base_of<MagnetIndocFunctor, MagnIndocFunctType>::value
//double MagnetismDirectTask::calcMagneticIndoction(double x, double z) const
//{
//	double bx = 0;
//	double xMidElem, zMidElem;
//	double coefX, coefZ;
//	double x_, z_;
//	double r2, r3;
//	double buf;
//	const std::vector<MagnetElement>& _magneticElements = _mesh->getMagneticElements();
//	for (const MagnetElement& elem : _magneticElements)
//	{
//		MagnIndocFonctType magIndocFunctor(x, z, _mesh->getI(), elem);
//		xMidElem = (elem.getIntervalX().getLeftPoint() + elem.getIntervalX().getRightPoint()) / 2;
//		zMidElem = (elem.getIntervalZ().getLeftPoint() + elem.getIntervalZ().getRightPoint()) / 2;
//		bx += elem.getSquare() * magIndocFunctor(xMidElem, zMidElem);
//	}
//
//	return bx;
//}
double MagnetismDirectTask::calcMagneticIndoctionX(double x, double z) const
{
	/*
	double bx = 0;
	double xMidElem, zMidElem;
	double coefX, coefZ;
	double x_, z_;
	double r2, r3;
	double buf;
	const std::vector<MagnetElement>& _magneticElements = _mesh->getMagneticElements();
	for (const MagnetElement& elem : _magneticElements)
	{
		xMidElem = (elem.getIntervalX().getLeftPoint() + elem.getIntervalX().getRightPoint()) / 2;
		zMidElem = (elem.getIntervalZ().getLeftPoint() + elem.getIntervalZ().getRightPoint()) / 2;
		MagnetIndocFunctorX magIndocFunctor(x, z, _mesh->getI(), elem);
		//x_ = x - xMidElem;
		//z_ = z - zMidElem;
		//r2 = x_ * x_ + z_ * z_;
		//if (r2 / (elem.getIntervalX().getLength() * elem.getIntervalZ().getLength()) > 0.1)
		//{
		//	auto magFunc = [](double x1, double y1) 
		//	{
		//		return calcMagneticIndoctionXFromElemPoint(x, z, x1, y1, elem, _mesh->getI());
		//	}
		//}
		//r3 = pow(sqrt(r2), 3);
		//coefX = 3 * x_ * x_ / r2 - 1;
		//coefZ = 3 * x_ * z_ / r2;
		//buf = elem.get_pX() * coefX  + elem.get_pZ() * coefZ;
		bx += elem.getSquare() * magIndocFunctor(xMidElem, zMidElem);
	}
	*/
	//MagnetIndocFunctorX magnIndocFunc(x, z, _mesh->getI(), _mesh->getMagneticElements()[0]);
	return calcMagneticIndoction<MagnetIndocFunctorX>(x, z);
}

double MagnetismDirectTask::calcMagneticIndoctionY(double x, double z) const
{
	//double bx = 0;
	//double xMidElem, zMidElem;
	//double coefX, coefZ;
	//double x_, z_;
	//double r2, r3;
	//double buf;
	//const std::vector<MagnetElement>& _magneticElements = _mesh->getMagneticElements();
	//for (const MagnetElement& elem : _magneticElements)
	//{
	//	xMidElem = (elem.getIntervalX().getLeftPoint() + elem.getIntervalX().getRightPoint()) / 2;
	//	zMidElem = (elem.getIntervalZ().getLeftPoint() + elem.getIntervalZ().getRightPoint()) / 2;
	//	//x_ = x - xMidElem;
	//	//z_ = z - zMidElem;
	//	//r2 = x_ * x_ + z_ * z_;
	//	//r3 = pow(sqrt(r2), 3);
	//	//coefX = 3 * x_ * z_ / r2;
	//	//coefZ = 3 * z_ * z_ / r2 - 1;
	//	//buf = elem.get_pX() * coefX + elem.get_pZ() * coefZ;
	//	bx += elem.getSquare() * _mesh->getI() / 4 / PI / r3 * buf;
	//}

	//return bx;
	MagnetIndocFunctorZ magnIndocFunc(x, z, _mesh->getI(), _mesh->getMagneticElements()[0]);
	return calcMagneticIndoction<MagnetIndocFunctorZ>(x, z);
}
