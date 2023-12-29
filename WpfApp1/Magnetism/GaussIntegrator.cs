using System;

namespace MKT_Interface.Magnetism;

public static class GaussIntegrator
{
	//static private GaussIntegrator _instance = null;
	//static public GaussIntegrator Instance => _instance ??= new GaussIntegrator();
    //private GaussIntegrator(){}

	static readonly double[] weights = [.55555555555555555, .8888888888888888, .55555555555555555]; 
	static readonly double[] points = [.7745966692414833, 0.0, -.7745966692414833]; 

 	public static double Integrate1D(Func<double, IFuncParams, double> function, double leftx, double rightx, IFuncParams funcParams)
	{
		double result = 0;
		double h = (rightx - leftx) / 2d;
		double c = (leftx + rightx) / 2d;

		for (int i = 0; i < 3; i++)
			result += weights[i] * function(h * points[i] + c, funcParams);

		return result * h;
	}

    public static double Integrate2D(Func<double, double, IFuncParams, double> function, double leftx, double rightx, double lefty, double righty, IFuncParams funcParams)
    {
        double result = 0;
        double hx = (rightx - leftx) / 2d;
        double cx = (leftx + rightx) / 2d;
        double hy = (righty - lefty) / 2d;
        double cy = (lefty + righty) / 2d;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                result += weights[i] * weights[j] * function(hx * points[i] + cx, hy * points[i] + cy, funcParams);

        return result * hx * hy;
    }
}

public interface IFuncParams { }

