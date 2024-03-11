using MKT_Interface.Models;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace MKT_Interface.Magnetism;

public class EMForwardCalculator
{
    public (double Bx, double Bz)[] B = null!;
    private readonly EMParameters parameters;

    public EMForwardCalculator(EMParameters parameters)
    {
        this.parameters = parameters;
        Init();
    }
    class BParams
    {
        public EMParameters parameters { get; set; } = null!;
        public Cell Cell { get; set; } = null!;
        public double RecX { get; set; }
    }

    public void Calculate(List<Cell> incells = null)
    {
        var recs = GetRecievers();
        B = new (double Bx, double Bz)[recs.Length];
        var cells = incells ?? parameters.Cells;

        for (int i = 0; i < B.Length; i++)
        {
            var b = Vector2d.Zero;

            foreach (var cell in cells)
            {
                var @params = new BParams { RecX = recs[i], parameters = parameters, Cell = cell};
                b += GetB(@params); //new Vector2d(Integrate1D(CalcBx, lx, rx, @params), Integrate1D(CalcBz, lz, rz, @params));
            }

            B[i] = (b.X, b.Y);
        }

    }

    static readonly double[] weights = [.55555555555555555, .8888888888888888, .55555555555555555];
    static readonly double[] points = [.7745966692414833, 0.0, -.7745966692414833];
    static double[] gweights = new double[9];
    static Vector2d[] gpoints = new Vector2d[9];
    private void Init()
    {       
        
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                gweights[i * 3 + j] = weights[i] * weights[j];
                gpoints[i * 3 + j] = new Vector2d(points[i], points[j]);
            }
    }

    private Vector2d GetB(BParams bParams)
    {
        double lx = bParams.Cell.X0, rx = bParams.Cell.X1, lz = bParams.Cell.Z0, rz = bParams.Cell.Z1;
        double cx = (lx + rx) / 2d, cz = (lz + rz) / 2d;

        var (Bx, Bz) = (0d, 0d);

        for (int i = 0; i < 9; i++)
        {
            double hx = bParams.Cell.X1 - bParams.Cell.X0, hz = bParams.Cell.Z1 - bParams.Cell.Z0;
            double px = bParams.Cell.PX, pz = bParams.Cell.PZ;

            double I = bParams.parameters.Current;
            (double x, double z) r = (bParams.RecX - (hx * gpoints[i].X + cx), - (hz * gpoints[i].Y + cz));

            double r2 = r.x * r.x + r.z * r.z;
            double r3 = Math.Sqrt(r2) * r2;
            double area = hx * hz;
            double w_mu = area * I / (4d * Math.PI * r3) * gweights[i] / 4d;

            Bx += w_mu * (px * (3d * r.x * r.x / r2 - 1d) + pz * (3d * r.x * r.z / r2));
            Bz += w_mu * (px * (3d * r.x * r.z / r2) + pz * (3d * r.z * r.z / r2 - 1d));
        }

        return new Vector2d(Bx, Bz);
    }


    public void Save(string path)
    {
      using var save = new StreamWriter(path);
      foreach (var (Bx, Bz) in B)
         save.WriteLine($"{Bx}\t{Bz}");
   }

    public double[] GetRecievers()
    {
        double[] recievers = new double[parameters.RecCount];
        double h = (parameters.RecX1 - parameters.RecX0) / (parameters.RecCount - 1);
        for (int i = 0; i < parameters.RecCount; i++)
            recievers[i] = parameters.RecX0 + i * h;
        return recievers;
    }
}


