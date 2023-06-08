#pragma once
class MagnetInfo
{
private:
	double _pX;
	double _pZ;
public:
	MagnetInfo(double pX, double pZ);
	MagnetInfo();
	void set_pX(double pX);
	void set_pZ(double pZ);

	double get_pX() const;
	double get_pZ() const;
};

