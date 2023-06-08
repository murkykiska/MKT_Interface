#pragma once
#include <sstream>
#include <vector>
struct Reciver
{
	double x;
	double z;
	double Bx;
	double Bz;

	Reciver();
	Reciver(std::istream& in);
	void read(std::istream& in);

	double getX() const;
	double getZ() const;
	double getBx() const;
	double getBz() const;

	static void readRecivers(std::vector<Reciver>& recivers, std::string fileName);
};

