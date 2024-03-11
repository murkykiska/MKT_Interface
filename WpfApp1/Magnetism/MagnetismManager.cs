using MKT_Interface.Magnetism;
using MKT_Interface.Models;
using MKT_Interface.NeuralNetwork;
using NeuralNetwork;
using OpenTK.Mathematics;
using Plot.Function;
using Splines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace WpfApp1.EM;

public class MagnetismManager
{
   Vector4d[] _data;
   Cell[] Cells;
   EMForwardCalculator calculator = null!;
   NeuralNetworkHandler nnHandler = null!;
   private readonly EMParameters emParams;
   private readonly NNParameters nnParams;

   public MagnetismManager(EMParameters emParams, NNParameters nnParams, string model_path = "")
   {
      calculator = new(emParams);

      if (string.IsNullOrEmpty(model_path))
         nnHandler = new(nnParams, emParams);
      else
         nnHandler = new(nnParams, emParams, new StreamWriter(model_path));
      this.emParams = emParams;
      this.nnParams = nnParams;
   }
   public (float[] x, float[] Bx, float[] Bz) MakeDirect()
   {
      calculator.Calculate();
      var Bxs = calculator.B.Select(x => (float)x.Bx).ToArray();
      var Bzs = calculator.B.Select(x => (float)x.Bz).ToArray();
      var xs = calculator.GetRecievers().Select(x => (float)x).ToArray();
      return (xs, Bxs, Bzs);
   }
   public void MakeReverse()
   {
      //nnHandler.TrainModel();
   }

   public (List<Cell> cells, (float[] Bx, float[] Bz) B, float[] xs) GenerateModel()
   {
      var dg = new DatasetGenerator(1, nnParams.InputSize, (nnParams.MinX, nnParams.MaxX, nnParams.MinZ, nnParams.MaxZ),
              nnParams.XIntervalCount, nnParams.ZIntervalCount, emParams.RecCount, emParams.RecX0, emParams.RecX1);

      var model = dg.GenerateSingle((nnParams.Px, nnParams.Pz), nnParams.AnomalyLen);

      var xs = new float[emParams.RecCount];
      float xh = (float)(emParams.RecX1 - emParams.RecX0) / (emParams.RecCount - 1);

      for (int i = 0; i < emParams.RecCount; i++)
         xs[i] = (float)emParams.RecX0 + i * xh;

      var Bx = model.B.Select(x => (float) x.x).ToArray();
      var Bz = model.B.Select(x => (float) x.z).ToArray();

      return (model.cells, (Bx, Bz), xs);
   }

   public void TrainModel()
   {
      try
      {
         nnParams.NNstatus = NNParameters.NNStatus.GeneratingData;
         nnHandler.GenerateData();
         nnHandler.PrepareModel();

         nnParams.NNstatus = NNParameters.NNStatus.Training;
         nnHandler.TrainModel();
         nnHandler.SaveModel();

         nnParams.NNstatus = NNParameters.NNStatus.Ready;

      }
      catch (Exception ex)
      {
         Logger.FileLogger.Logger.ErrorLog(ex.StackTrace ?? "");
         Logger.FileLogger.Logger.DebugLog(ex.Message);
         nnParams.NNstatus = NNParameters.NNStatus.Error;
      }
   }

   public (Box2[] boxCells, float[] values) GetBox2Cells(List<Cell> cells = null)
   {
      cells ??= emParams.Cells;

      Box2[] boxCells = new Box2[cells.Count];
      float[] values = new float[cells.Count];

      for (int i = 0; i < boxCells.Length; i++)
      {
         boxCells[i].Min = ((float)cells[i].X0, (float)cells[i].Z0);
         boxCells[i].Max = ((float)cells[i].X1, (float)cells[i].Z1);

         values[i] = emParams.ShowPx ? (float)cells[i].PX : (float)cells[i].PZ;
      }

      return (boxCells, values);
   }
   public Function2D MagnetismDataAsFunc2D(bool isX, float coord)
   {
      var data =
         from d in _data
         where !isX ? d.X == coord : d.Y == coord
         orderby isX ? d.X : d.Y
         select d;

      var f = new Function2D(lineType: Function2D.LineTypes.Dashed);

      var d_ = data.ToArray();
      Vector2[] func = new Vector2[d_.Length];
      for (int i = 0; i < d_.Length; i++)
      {
         (float x, float y) xy = (isX ? (float)d_[i].X : (float)d_[i].Y, isX ? (float)d_[i].Z : (float)d_[i].W);
         func[i] = xy;
      }

      f.FillPoints(func);
      return f;
   }
   public FunctionCell2D MagnetismDataAsFuncCells2D(bool isX)
   {

      Box2[] area = new Box2[Cells.Length];
      float[] vals = new float[Cells.Length];

      for (int i = 0; i < Cells.Length; i++)
      {
         area[i] = new Box2(((float)Cells[i].X0, (float)Cells[i].Z0), ((float)Cells[i].X1, (float)Cells[i].Z1));
         vals[i] = (float)(isX ? Cells[i].PX : Cells[i].PZ);
      }

      var f = new FunctionCell2D(area, vals);
      return f;
   }

   public void FillFunc2D(FunctionCell2D func, bool isX)
   {
      if (Cells is null) return;
      Box2[] area = new Box2[Cells.Length];
      float[] vals = new float[Cells.Length];

      for (int i = 0; i < Cells.Length; i++)
      {
         area[i] = new Box2(((float)Cells[i].X0, (float)Cells[i].Z0), ((float)Cells[i].X1, (float)Cells[i].Z1));
         vals[i] = (float)(isX ? Cells[i].PX : Cells[i].PZ);
      }

      func.FillValues(vals);
      func.Prepare();
   }

   public void FillSmoothedFunc2D(Function2D func, Function2D smoothFunc, int accuracy = 10)
   {
      if (nnParams.InputSize <= 1) return;
      var ps = func.Points;

      var xs = ps.Select(x => (double)x.X).ToArray();
      var sB = ps.Select(x => (double)x.Y).ToArray();

      HermiteSpline spline = new(nnParams.InputSize - 1, xs, sB, 1e-10, (double)nnParams.InputSize / ps.Count());

      List<float> spline_xs = [];

      float minX = (float)xs.Min();
      float maxX = (float)xs.Max();
      float hX = (maxX - minX) / nnParams.InputSize;

      spline_xs.Add(minX);
      for (int i = 1; i < nnParams.InputSize; i++)
         spline_xs.Add(minX + i * hX);

      for (int i = 0; i < nnParams.InputSize; i++)
      {
         float hLX = hX / (accuracy + 1);

         for (int j = 0; j < accuracy; j++)
            spline_xs.Add(minX + i * hX + (j + 1) * hLX);
      }

      spline_xs.Add(maxX);
      spline_xs.Sort();

      Vector2[] points = spline_xs.Select(x => new Vector2(x, (float)spline.GetValue(x, false))).ToArray();

      smoothFunc.FillPoints(points);
      smoothFunc.Prepare();
   }

   internal void LoadModel()
   {
      nnHandler.LoadModel();
   }

   internal (float[] Bx, float[] Bz) Predict(Function2D Bx, Function2D Bz)
   {

      var Bxxs = Bx.Points.Select(x => (double)x.X).ToArray();
      var Bxs = Bx.Points.Select(x => (double)x.Y).ToArray();
      var Bzxs = Bz.Points.Select(x => (double)x.X).ToArray();
      var Bzs = Bz.Points.Select(x => (double)x.Y).ToArray();

      HermiteSpline xspline = new(nnParams.InputSize - 1, Bxxs, Bxs, 1e-10, (double)nnParams.InputSize / Bxxs.Count());
      HermiteSpline zspline = new(nnParams.InputSize - 1, Bzxs, Bzs, 1e-10, (double)nnParams.InputSize / Bzxs.Count());

      double[] values = new double[xspline.Qs.Length * 2];

      for (int i = 0; i < values.Length / 2; i++)
      {
         values[2 * i] = xspline.Qs[i];
         values[2 * i + 1] = zspline.Qs[i];
      }

      var predicted = nnHandler.Predict(values);
      float[] pBx = new float[predicted.Length];
      float[] pBz = new float[predicted.Length];

      for (int i = 0; i < predicted.Length; i++)
      {
         pBx[i] = (float)(predicted[i] * nnParams.Pz);
         pBz[i] = (float)(predicted[i] * nnParams.Pz);    
      }

      return (pBx, pBz);
   }

   internal void SetErrorStatus()
   {
      nnParams.NNstatus = NNParameters.NNStatus.Error;
   }
}
