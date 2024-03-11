using MKT_Interface.Magnetism;
using MKT_Interface.Models;
using Splines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MKT_Interface.NeuralNetwork;

public class DatasetGenerator
{
   public class EMModel()
   {
      public List<Cell> cells { get; set; } // модель для обучения
      public (double x, double z)[] B; // В нейросеть
   }
   public DatasetGenerator(int numOfData, int inputLayerNum,
                           (double X0, double X1, double Y0, double Y1) area,
                           int numOfXCells, int numOfYCells,
                           int recNum, double recx0, double recx1)
   {
      this.numOfData = numOfData;
      this.inputLayerSize = inputLayerNum;
      this.area = area;
      this.numOfXCells = numOfXCells;
      this.numOfYCells = numOfYCells;
      this.recNum = recNum;
      this.recx0 = recx0;
      this.recx1 = recx1;

      EMParameters emParams = new() { RecCount = recNum, RecX0 = recx0, RecX1 = recx1 };
      calculator = new(emParams);

   }

   public List<EMModel> Models = new();
   Random random = new Random(DateTime.Now.GetHashCode());
   private readonly int numOfData;
   private readonly int inputLayerSize;
   private readonly (double X0, double X1, double Y0, double Y1) area;
   private readonly int numOfXCells;
   private readonly int numOfYCells;
   private readonly int recNum;
   private readonly double recx0;
   private readonly double recx1;
   private EMForwardCalculator calculator;

   public EMModel GenerateSingle((double x, double y) earthP, double len)
   {
      var segs = GenerateSegmentation();
      double hx = (area.X1 - area.X0) / numOfXCells, hy = (area.Y1 - area.Y0) / numOfYCells;

      Random random = new Random();
      int r = random.Next(segs.Count);

      var seg = segs[r];
      List<Cell> cells = [];
      for (int i = 0; i < numOfXCells; i++)
         for (int j = 0; j < numOfYCells; j++)
         {

            double x0 = area.X0 + hx * i, x1 = area.X0 + hx * (i + 1);
            double y0 = area.Y0 + hy * j, y1 = area.Y0 + hy * (j + 1);

            double cx = (x0 + x1) / 2d, cz = (y0 + y1) / 2d;
            bool found = seg.x0 < cx && cx < seg.x1 && seg.y0 < cz && cz < seg.y1;

            cells.Add(new Cell(x0, x1, y0, y1, found ? len * earthP.x : earthP.x, found ? len * earthP.y : earthP.y));
         }

      calculator.Calculate(cells);
      var B = calculator.B;
      //var xs = new double[recNum];
      //double xh = (recx1 - recx0) / (recNum - 1);

      //for (int i = 0; i < recNum; i++)
      //   xs[i] = recx0 + i * xh;

      //var Bx = B.Select(x => x.Bx).ToArray();
      //var Bz = B.Select(x => x.Bz).ToArray();

      //HermiteSpline splineBx = new(inputLayerSize, xs, Bx, weight: (double)inputLayerSize / xs.Count());
      //HermiteSpline splineBz = new(inputLayerSize, xs, Bz, weight: (double)inputLayerSize / xs.Count());

      //var sBx = splineBx.Qs;
      //var sBz = splineBz.Qs;
      //(double Bx, double By)[] sB = new (double Bx, double By)[inputLayerSize]; 

      //for (int i = 0; i < inputLayerSize ; i++)
      //   sB[i] = (sBx[i], sBz[i]);

      EMModel model = new() { B = B, cells = cells };
      return model;

   }
   public void GenerateData((double x, double y) earthP, double len, string dataFolder = "./NeuralNetwork/Data", string dataName = "data")
   {
      ArgumentException.ThrowIfNullOrEmpty(dataName, nameof(dataName));
      ArgumentException.ThrowIfNullOrEmpty(dataFolder, nameof(dataFolder));

      for (int k = 0; k < numOfData; k++)
      {
         var segs = GenerateSegmentation();
         double hx = (area.X1 - area.X0) / numOfXCells, hy = (area.Y1 - area.Y0) / numOfYCells;
         foreach (var seg in segs)
         {
            List<Cell> cells = [];
            for (int i = 0; i < numOfXCells; i++)
               for (int j = 0; j < numOfYCells; j++)
               {

                  double x0 = area.X0 + hx * i, x1 = area.X0 + hx * (i + 1);
                  double y0 = area.Y0 + hy * j, y1 = area.Y0 + hy * (j + 1);

                  double cx = (x0 + x1) / 2d, cz = (y0 + y1) / 2d;
                  bool found = seg.x0 < cx && cx < seg.x1 && seg.y0 < cz && cz < seg.y1;

                  cells.Add(new Cell(x0, x1, y0, y1, found ? len * earthP.x : earthP.x, found ? len * earthP.y : earthP.y));
               }

            calculator.Calculate(cells);
            var B = calculator.B;
            var xs = new double[recNum];
            double xh = (recx1 - recx0) / (recNum - 1);

            for (int i = 0; i < recNum; i++)
               xs[i] = recx0 + i * xh;

            var Bx = B.Select(x => x.Bx).ToArray();
            var Bz = B.Select(x => x.Bz).ToArray();
            HermiteSpline splineBx = new(inputLayerSize - 1, xs, Bx, weight: (double)inputLayerSize / xs.Count());
            HermiteSpline splineBz = new(inputLayerSize - 1, xs, Bz, weight: (double)inputLayerSize / xs.Count());

            var sBx = splineBx.Qs;
            var sBz = splineBz.Qs;
            (double Bx, double By)[] sB = new (double Bx, double By)[inputLayerSize];

            for (int i = 0; i < inputLayerSize; i++)
               sB[i] = (sBx[i], sBz[i]);

            EMModel model = new() { B = sB, cells = cells };
            Models.Add(model);
         }
      }
   }

   private List<Section> GenerateSegmentation()
   {
      List<Section> sections = new();

      int xleft = 0;
      double hx = (area.X1 - area.X0) / numOfXCells,
             hy = (area.Y1 - area.Y0) / numOfYCells;
      while (xleft < numOfXCells)
      {
         int xright = random.Next(xleft + 1, numOfXCells + 1);

         double x0 = area.X0 + xleft * hx;
         double x1 = area.X0 + xright * hx;

         int yleft = 0;
         while (yleft < numOfYCells)
         {
            int yright = random.Next(yleft + 1, numOfYCells + 1);
            double y0 = area.Y0 + yleft * hy;
            double y1 = area.Y0 + yright * hy;

            yleft = yright;
            sections.Add(new Section { x0 = x0, y0 = y0, x1 = x1, y1 = y1 });
         }
         xleft = xright;
      }

      return sections;
   }
   public struct Section
   {
      public double x0;
      public double x1;
      public double y0;
      public double y1;
   }
}
