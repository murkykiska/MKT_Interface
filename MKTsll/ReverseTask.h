#pragma once
#include "MagnetismDirectTask.h"
#include "Reciver.h"
#include "DEFINES.h"
class ReverseTask
{
private:
	double _alpha;
	void calcL(std::vector<std::vector<double>>& L, const std::vector<Reciver>& _recivers, MeshInfo& mesh) const;
	void calcS(std::vector<double>& S, const std::vector<Reciver>& _recivers) const;
	void addAlpha(std::vector<std::vector<double>>& Matrix) const;
	void fillElementsWithAnswer(MeshInfo& mesh, const std::vector<double>& ans) const;
public:
	ReverseTask(double alpha);
	void countSolution(const std::vector<Reciver>& _recivers, MeshInfo& mesh) const;
	void setAlpha(double alpha);
	double getAlpha() const;
};

