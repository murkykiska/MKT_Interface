#include "Reciver.h"
#include <fstream>
Reciver::Reciver()
{
	x = z = 1;
	Bx = Bz = 0;
}

Reciver::Reciver(std::istream& in)
{
	read(in);
}

void Reciver::read(std::istream& in)
{
	in.read((char*)&x, sizeof(x));
	if (in.fail())
		throw std::exception("Cannot read x coordinate of reciver");

	in.read((char*)&z, sizeof(z));
	if (in.fail())
		throw std::exception("Cannot read y coordinate of reciver");

	in.read((char*)&Bx, sizeof(Bx));
	if (in.fail())
		throw std::exception("Cannot read x magnetic value in reciver");

	in.read((char*)&Bz, sizeof(Bz));
	if (in.fail())
		throw std::exception("Cannot read y magnetic value in reciver");
}

double Reciver::getX() const
{
	return x;
}
double Reciver::getZ() const
{
	return z;
}
double Reciver::getBx() const
{
	return Bx;
}
double Reciver::getBz() const
{
	return Bz;
}

void Reciver::readRecivers(std::vector<Reciver>& recivers, std::string fileName)
{
	std::ifstream in;
	in.open(fileName, std::ios::binary);
	int reciversCount;
	in.read((char*)&reciversCount, sizeof(reciversCount));
	if (in.fail())
		throw std::exception("Cannot read recivers count");
	if(reciversCount < 0)
		throw std::exception("Recivers count cannot be less than 0");
	recivers.reserve(reciversCount);
	for(int i = 0; i < reciversCount; i++)
		recivers.emplace_back(in);
}
