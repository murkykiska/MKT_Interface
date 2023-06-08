#pragma once
#include <fstream>
#include <vector>
#include <string>
#include <exception>
/// <summary>
/// Ñòðóêòóðà äëÿ âõîäíûõ äàííûõ
/// </summary>
class InputData
{
public:
	virtual std::istream& readData(std::istream& in) = 0;
	virtual std::string&& getTypeDataName() = 0;
	std::istream& operator << (std::istream& in) { readData(in); }
};

//std::istream& operator >> (std::istream& in, InData& data);

template<class DataType> requires std::is_base_of<InputData, DataType>::value
std::istream& operator >> (std::istream& in, DataType& data)
{
	return data.readData(in);
}

template<class DataType> requires std::is_constructible<DataType, std::istream&>::value
std::istream& readData(std::istream& in, std::vector<DataType>& data)
{
	//DataType buf;

	int nData;
	in >> nData;

	if (in.fail())
	{
		std::string errMSG = "Cannot read number of elements";
		//programlog::writeErr("Cannot read number of \"" + buf.getTypeDataName() + "\" elements");
		throw new std::exception(errMSG.c_str());
		//return in;
	}

	
	if (nData < 0)
	{
		std::string errMSG = "The number of elements cannot be < 0";
		//programlog::writeErr("The number of \"" + buf.getTypeDataName() + "\" elements cannot be < 0");
		throw new std::exception(errMSG.c_str());
		//return in;
	}

	data.reserve(nData);

	for (int i = 0; i < nData; i++)
	{
		//in >> buf;
		data.emplace_back(in);
		//data[i] = buf;
		if (in.fail())
		{
			std::string errMSG = "Cannot read element";
			//programlog::writeErr("Cannot read \"" + buf.getTypeDataName() + "\" element");
			throw new std::exception(errMSG.c_str());
			//return in;
		}
	}
	return in;
}

template<class DataType> //requires std::is_base_of<InputData, DataType>::value
std::istream& operator >> (std::istream& in, std::vector<DataType>& dataAr)
{
	return readData(in, dataAr);
}
