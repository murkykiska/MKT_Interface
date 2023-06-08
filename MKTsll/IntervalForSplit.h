#pragma once
#include "InputData.h"
#include "SplitOptions.h"

class IIntervalForSplit
{
public:
	virtual int getFirstPointNum() const = 0;
	virtual int getSecondPointNum() const = 0;
	virtual const SplitOptions& getSplitOptions() const = 0;
};

class IntervalForSplit : public IIntervalForSplit, public InputData
{
private:
	int _p1, _p2;
	SplitOptions _splitOptions;
public:
	IntervalForSplit(int p1, int p2, SplitOptions& splitOptions);
	IntervalForSplit(std::istream& in);
	IntervalForSplit(libconfig::Setting& setting);
	IntervalForSplit();


	int getFirstPointNum() const override;
	int getSecondPointNum() const  override;
	const SplitOptions& getSplitOptions() const override;

	void setFirstPointNum(int p1);
	void setSecondPointNum(int p2);
	void setSplitOptions(SplitOptions& splitOptions);

	void setPointsNum(int p1, int p2);
	void setInterval(int p1, int p2, SplitOptions& splitOptions);
	void setInterval(libconfig::Setting& setting);

	std::istream& readData(std::istream& in) override;
	std::string&& getTypeDataName() override;
};


