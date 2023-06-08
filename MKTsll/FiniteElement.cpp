#include "FiniteElement.h"
#include <algorithm>

FiniteElement::FiniteElement(const IntervalRef& intervalX, const IntervalRef& intervalY, const IntervalRef& intervalZ, 
							const std::vector<int>& degreesOfFreedomNum,
							const std::vector<std::function<double(double, double, double)>>& func) :
							_geomElement(intervalX, intervalY, intervalZ),
							_degreesOfFreedomNum(degreesOfFreedomNum),
							_func(func) 
{	
	if (degreesOfFreedomNum.size() < 1)
		throw std::exception("Degrees of freedom count must be grater than 0");

	if (degreesOfFreedomNum.size() != _func.size())
		throw std::exception("Degrees of freedom count must be equal to bsis functions count in finite element");

}
