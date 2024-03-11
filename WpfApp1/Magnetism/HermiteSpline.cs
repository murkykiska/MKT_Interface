using System.Collections.Generic;
using System;
using static System.Math;
using System.Linq;

namespace Splines;

public class HermiteSpline
{
   double Psi(double x, double hi, int i)
   {
      double x2 = x * x, x3 = x * x * x;
      switch (i)
      {
         case 0: return 1d - 3d * x2 + 2d * x3;
         case 1: return hi * (x - 2d * x2 + x3);
         case 2: return 3d * x2 - 2d * x3;
         case 3: return hi * (-x2 + x3);
         default:
            throw new Exception("wtf");
      }
   }

   double[,] G(double h)
   {
      var g = new double[4, 4]
      {
            {36d,     3d * h,     -36,     3d * h },
            {3d * h,  4d * h * h, -3d * h, -h * h },
            {-36d,    -3d * h,    36d,     -3d * h },
            { 3d * h, -h * h,     -3d * h, 4d * h * h }
      };

      for (int i = 0; i < 4; i++)
         for (int j = 0; j < 4; j++)
            g[i, j] /= 30d * h;
      return g;
   }

   private int elementsNum;
   private double[] xs;
   private double[] ys;
   private Segment[] segments;
   public double Alpha { get; set; } = 1e-7;
   public double[] Qs
   {
      get
      {
         double[] qs = new double[elementsNum + 1];
         for (int i = 0; i < elementsNum + 1; i++)
            qs[i] = Q[2 * i];
         return qs;
      }
   }

   public HermiteSpline(int elementsNum, double[] xs, double[] ys, double alpha = 1e-7, double weight = 1)
   {
      Alpha = alpha;
      Weight = weight;
      Fill(elementsNum, xs, ys);
      Build();
   }

   public HermiteSpline()
   {
   }

   public void Fill(int elementsNum, double[] xs, double[] ys)
   {
      this.elementsNum = elementsNum;
      this.xs = xs;
      this.ys = ys;
      A = new double[2 * elementsNum + 2, 2 * elementsNum + 2];
      B = new double[2 * elementsNum + 2];
      Q = new double[2 * elementsNum + 2];

      segments = new Segment[elementsNum];
      for (int i = 0; i < elementsNum; i++)
      {
         var h = (xs[^1] - xs[0]) / elementsNum;
         var seg = segments[i] = new Segment(xs[0] + h * i, xs[0] + h * (i + 1));

         for (int j = 0; j < xs.Length; j++)
            seg.TryAdd(xs[j], ys[j]);
      }
      segments[^1].Add(xs[^1], ys[^1]);
   }

   public void Build()
   {
      CreateSLAE();
      SolveSLAE();
   }

   public double GetValue(double x, bool prime = false)
   {
      double y = 0;
      double h = (xs[^1] - xs[0]) / elementsNum;

      for (int k = 0; k < segments.Length; k++)
      {
         var seg = segments[k];

         if (!seg.ContainsInclusive(x))
            continue;

         double ksi = seg.Ksi(x);
         if (prime)
            y += Q[2 * k + 1] * Psi(ksi, h, 1) + Q[2 * k + 3] * Psi(ksi, h, 3);
         else
            y += Q[2 * k] * Psi(ksi, h, 0) + Q[2 * k + 2] * Psi(ksi, h, 2);

         break;
      }
      return y;
   }

   double[] Q;
   double[,] A;
   double[] B;
   public double Weight { set; get; }

   void CreateSLAE()
   {
      for (int k = 0; k < segments.Length; k++)
      {
         var seg = segments[k];

         double w = Weight;

         var a = new double[4, 4];
         var b = new double[4];

         foreach (var (x, y) in seg.Points)
            for (int i = 0; i < 4; i++)
            {
               for (int j = 0; j < 4; j++)
                  a[i, j] += w * Psi(seg.Ksi(x), seg.H, i) * Psi(seg.Ksi(x), seg.H, j);

               b[i] += w * Psi(seg.Ksi(x), seg.H, i) * y;
            }

         var g = G(seg.H);
         for (int i = 0; i < 4; i++)
         {
            B[2 * k + i] += b[i];
            for (int j = 0; j < 4; j++)
               A[2 * k + i, 2 * k + j] += a[i, j] + Alpha * g[i, j];
         }

      }

      //fix ends

      //for (int i = 0; i < segments.Length; i++)
      //{
      //   A[0, i] = 0;
      //   A[1, i] = 0;

      //   B[0] = segments[0].Points[0].y;
      //   B[1] = 0;

      //   A[2 * segments.Length - 2, i] = 0;
      //   A[2 * segments.Length - 1, i] = 0;
      //}
      //A[0, 0] = A[1, 1] = 1;
      //A[2 * segments.Length - 2, 2 * segments.Length - 2] = 0;
      //A[2 * segments.Length - 1, 2 * segments.Length - 1] = 0;

      //B[0] = segments[0].Points[0].y;
      //B[1] = 0;

      //B[^2] = segments[^1].Points[^1].y;
      //B[^1] = 0;
   }

   void SolveSLAE()
   {
      for (int ii = 0; ii < Q.Length; ii++)
         Q[ii] = B[ii];

      int i, j, k, n, m;
      n = B.Length;

      double aa, bb;
      for (k = 0; k < n; k++)
      {
         aa = Abs(A[k, k]);
         i = k;
         for (m = k + 1; m < n; m++)
            if (Abs(A[m, k]) > aa)
            {
               i = m;
               aa = Abs(A[m, k]);
            }

         if (aa == 0)
         {
            throw new System.Exception("System doesn't have solutions");
         }

         if (i != k)
         {
            for (j = k; j < n; j++)
            {
               bb = A[k, j];
               A[k, j] = A[i, j];
               A[i, j] = bb;
            }
            bb = B[k];
            B[k] = B[i];
            B[i] = bb;
         }
         aa = A[k, k];
         A[k, k] = 1;
         for (j = k + 1; j < n; j++)
            A[k, j] = A[k, j] / aa;
         B[k] /= aa;

         for (i = k + 1; i < n; i++)
         {
            bb = A[i, k];
            A[i, k] = 0;
            if (bb != 0)
            {
               for (j = k + 1; j < n; j++)
                  A[i, j] = A[i, j] - bb * A[k, j];
               B[i] -= bb * B[k];
            }

         }
      }

      for (i = n - 1; i >= 0; i--)
      {
         for (j = n - 1; j > i; j--)
            B[i] -= A[i, j] * B[j];
      }


   }
}

class Segment(double Left, double Right)
{
   public double Left { get; } = Left;
   public double Right { get; } = Right;
   public List<(double x, double y)> Points { get; } = [];
   public IEnumerable<double> Ksis => Points.Select(x => Ksi(x.x));
   public double H { get; } = Right - Left;
   public bool Contains(double point) => Ksi(point) >= 0 && Ksi(point) < 1;
   public bool ContainsInclusive(double point) => Ksi(point) >= 0 && Ksi(point) <= 1;
   public bool TryAdd(double x, double y)
   {
      if (Contains(x))
      {
         Points.Add((x, y));
         return true;
      }
      return false;
   }
   public void Add(double x, double y) => Points.Add((x, y));

   public double Ksi(double x) => (x - Left) / H;

}