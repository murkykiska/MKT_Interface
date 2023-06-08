#pragma once
#include "MeshInfo.h"
class MagnetismDirectTask
{
private:
	const MeshInfo* _mesh;
public:
	MagnetismDirectTask(const MeshInfo& mesh);
	void setMesh(const MeshInfo& mesh);
	double calcMagneticIndoctionX(double x, double z) const;
	double calcMagneticIndoctionY(double x, double z) const;
};

