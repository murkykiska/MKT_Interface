#pragma once
#include "Area1D.h"

template <class T, class... Args1 > requires std::is_constructible<T, Args1...>::value
class DimenshionTemplate
{
public:
	T X, Y, Z;
	DimenshionTemplate(Args1... argX, Args1... argY, Args1... argZ) : X(argX...), Y(argY...), Z(argZ...) {}
	const T& getX() const
	{
		return X;
	}
	const T& getY() const
	{
		return Y;
	}
	const T& getZ() const
	{
		return Z;
	}
};