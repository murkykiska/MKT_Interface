#include "ReverseTask.h"

void writeInBinaryFileRecivers(double left, double right, int nRecivers, const MagnetismDirectTask& dirctTask, std::string outFileName)
{
	int nIntervals = nRecivers - 1;
	double h = (right - left) / nIntervals;
	std::ofstream out;
	out.open(outFileName, std::ios::binary);
	out.write((char*)&nRecivers, sizeof(nRecivers));
	double x;
	double z = 0;
	double bx, bz;
	for (int i = 0; i < nIntervals; i++)
	{
		x = left + i * h;
		bx = dirctTask.calcMagneticIndoctionX(x, z);
		bz = dirctTask.calcMagneticIndoctionY(x, z);
		out.write((char*)&x, sizeof(x));
		out.write((char*)&z, sizeof(z));
		out.write((char*)&bx, sizeof(bx));
		out.write((char*)&bz, sizeof(bz));
	}
	x = right;
	bx = dirctTask.calcMagneticIndoctionX(x, z);
	bz = dirctTask.calcMagneticIndoctionY(x, z);
	out.write((char*)&x, sizeof(x));
	out.write((char*)&z, sizeof(z));
	out.write((char*)&bx, sizeof(bx));
	out.write((char*)&bz, sizeof(bz));
	out.close();
}

//void writeBInConsole(double left, double right, int nRecivers, const MagnetismDirectTask& dirctTask)
//{
//	int nIntervals = nRecivers - 1;
//	double h = (right - left) / nIntervals;
//	std::cout.write((char*)&nRecivers, sizeof(nRecivers));
//	double x;
//	double z = 0;
//	for (int i = 0; i < nIntervals; i++)
//	{
//		x = left + i * h;
//		std::cout << x << " " << dirctTask.calcMagneticIndoctionX(x, z) << " " << dirctTask.calcMagneticIndoctionY(x, z) << std::endl;
//	}
//	x = right;
//	std::cout << x << " " << dirctTask.calcMagneticIndoctionX(x, z) << " " << dirctTask.calcMagneticIndoctionY(x, z) << std::endl;
//}


void makeDirectTask(const char* cfgFileName, double left, double right, int nRecivers, const char* reciversFileName)
{
		libconfig::Config cfg;
		getConfigFromFile(cfg, cfgFileName);
		MeshInfo mesh(cfg);
		MagnetismDirectTask directTask(mesh);
		if (left > right)
			std::swap(left, right);
		int nIntervals = nRecivers - 1;
		double h = (right - left) / nIntervals;
		std::ofstream out;
		out.open(reciversFileName, std::ios::binary);
		out.write((char*)&nRecivers, sizeof(nRecivers));
		double x;
		double z = 0;
		double bx, bz;
		for (int i = 0; i < nIntervals; i++)
		{
			x = left + i * h;
			bx = directTask.calcMagneticIndoctionX(x, z);
			bz = directTask.calcMagneticIndoctionY(x, z);
			out.write((char*)&x, sizeof(x));
			out.write((char*)&z, sizeof(z));
			out.write((char*)&bx, sizeof(bx));
			out.write((char*)&bz, sizeof(bz));
		}
		x = right;
		bx = directTask.calcMagneticIndoctionX(x, z);
		bz = directTask.calcMagneticIndoctionY(x, z);
		out.write((char*)&x, sizeof(x));
		out.write((char*)&z, sizeof(z));
		out.write((char*)&bx, sizeof(bx));
		out.write((char*)&bz, sizeof(bz));
		out.close();
}

void makeReverseTask(const char* cfgFileName, const char* reciversFileName, const char* ansFileName, double alpha)
{
		libconfig::Config cfgRev;
		getConfigFromFile(cfgRev, cfgFileName);
		MeshInfo meshRev(cfgRev);
		ReverseTask reverseTask(alpha);
		std::vector<Reciver> recivers;
		Reciver::readRecivers(recivers, reciversFileName);
		reverseTask.countSolution(recivers, meshRev);
		meshRev.writeMagneticElementsInBinaryFile(ansFileName);
}

// make direct
//int main(int argc, char* argv[])
//{
//	std::ifstream in;
//	in.open(argv[0]);
//
//	std::string cfg, recs;
//	double left, right;
//	int c;
//
//	in >> cfg >> left >> right >> c >> recs;
//
//	makeDirectTask(cfg.c_str(), left, right, c, recs.c_str());
//	return 0;
//}


int main(int argc, char* argv[])
{
	
	std::ifstream in;
	in.open(argv[0]);

	std::string cfg, recs, ans;
	double alpha;

	in >> cfg >> recs >> ans >> alpha;

	makeReverseTask(cfg.c_str(), recs.c_str(), ans.c_str(), alpha );
	return 0;
}