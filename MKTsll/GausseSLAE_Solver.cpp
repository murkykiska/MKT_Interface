#include "GausseSLAE_Solver.h"
int Gauss(std::vector<std::vector<double>>& A, std::vector<double>& b)
{
	int i, j, k, n, m;
	n = b.size();
	double aa, bb;
	for (k = 0; k < n; k++)
	{
		aa = abs(A[k][k]);
		i = k;
		for (m = k + 1; m < n; m++)
			if (abs(A[m][k]) > aa)
			{
				i = m;
				aa = abs(A[m][k]);
			}

		if (aa == 0)
		{
			throw ("System doesn't have solutions");
		}

		if (i != k)
		{
			for (j = k; j < n; j++)
			{
				bb = A[k][j];
				A[k][j] = A[i][j];
				A[i][j] = bb;
			}
			bb = b[k];
			b[k] = b[i];
			b[i] = bb;
		}
		aa = A[k][k];
		A[k][k] = 1;
		for (j = k + 1; j < n; j++)
			A[k][j] = A[k][j] / aa;
		b[k] /= aa;

		for (i = k + 1; i < n; i++)
		{
			bb = A[i][k];
			A[i][k] = 0;
			if (bb != 0)
			{
				for (j = k + 1; j < n; j++)
					A[i][j] = A[i][j] - bb * A[k][j];
				b[i] -= bb * b[k];
			}

		}
	}

	for (i = n - 1; i >= 0; i--)
	{
		for (j = n - 1; j > i; j--)
			b[i] -= A[i][j] * b[j];
	}

	return 0;
}
