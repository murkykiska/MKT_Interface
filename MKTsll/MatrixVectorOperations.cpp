#include "MatrixVectorOperations.h"
int MultiplyMatrixVector(std::vector<std::vector<double>>& Matrix, std::vector <double>& vect, std::vector<double>& ans)
{
	if (Matrix.size() != vect.size())
	{
		return -1;
	}

	if (Matrix.size() == 0)
		return -2;

	ans.resize(Matrix[0].size());
	for (int i = 0; i < Matrix[0].size(); i++)
	{
		ans[i] = 0;

		for (int j = 0; j < vect.size(); j++)
		{
			ans[i] += Matrix[j][i] * vect[j];
		}

	}
	return 0;
}

int Multiply_LLt(std::vector<std::vector<double>>& L, std::vector <std::vector<double>>& LLt)
{
	int LLt_size = L[0].size();

	LLt.resize(LLt_size);
	//for (int i = 0; i < L.size(); i++)
	for (int i = 0; i < LLt_size; i++)
	{
		LLt[i].resize(LLt_size);
		//for (int j = 0; j < L.size(); j++)
		for (int j = 0; j < LLt_size; j++)
		{
			LLt[i][j] = 0;
			//for (int k = 0; k < L[i].size(); k++)
			for (int k = 0; k < L.size(); k++)
			{
				//cout << i << " " << j << " " << L[k][i] << " " << L[k][j] << endl;
				LLt[i][j] += L[k][i] * L[k][j];
			}
		}

	}
	return 0;
}
