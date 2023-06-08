#pragma once
#include "GeomElement.h"
#include <vector>
#include <functional>

struct FiniteElement
{
public:
	const Cuboid _geomElement;
	const std::vector<std::function<double(double, double, double)>>& _func;
	const std::vector<int> _degreesOfFreedomNum;
	//const std::vector<double&> _degreesOfFreedom;
	FiniteElement(const IntervalRef& _intervalX, 
		const IntervalRef& _intervalY, 
		const IntervalRef& _intervalZ, 
		const std::vector<int>& degreesOfFreedomNum,
		const std::vector<std::function<double(double, double, double)>>& func);

};

