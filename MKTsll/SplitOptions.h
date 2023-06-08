#pragma once
#include "InputData.h"
#include "LibconfigDeluxe.h"

class ISplitOptions
{
public:
	virtual int getIntervalsCount() const = 0;
	virtual double getSparseRatio() const = 0;
};

class SplitOptions : public ISplitOptions, public InputData
{
private:
	double _sparseRatio;
	int _intervalsCount;

	void checkSparseRatio(double sparseRatio);
	void checkIntervalsCount(double intervalsCount);
public:
	SplitOptions();
	SplitOptions(int intervalCount, double sparseRatio);
	SplitOptions(std::istream& in);
	SplitOptions(libconfig::Setting& conf);

	void setIntervalsCount(int intervalsCount);
	void setSparseRatio(double sparseRatio);

	int getIntervalsCount() const  override;
	double getSparseRatio() const override;

	void setSplitOptions(int intervalsCount, double sparseRatio);
	void setSplitOptions(libconfig::Setting& setting);
	std::istream& readData(std::istream& in) override;
	std::string&& getTypeDataName() override;
};

