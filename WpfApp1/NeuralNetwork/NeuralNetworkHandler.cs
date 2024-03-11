using MKT_Interface.Models;
using MKT_Interface.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#region using TF
//using Tensorflow;
//using Tensorflow.Keras;
//using Tensorflow.Keras.ArgsDefinition;
//using Tensorflow.Keras.Engine;
//using Tensorflow.Keras.Models;
//using Tensorflow.NumPy;
//using static Tensorflow.Binding;
//using static Tensorflow.KerasApi;
#endregion

#region using Keras

using Keras;
using Keras.Layers;
using Keras.Models;
using Keras.Optimizers;
using Numpy;
using System.Windows;
using Logger;

#endregion

namespace NeuralNetwork;

public class NeuralNetworkHandler
{
   Sequential model;
   private readonly NNParameters nnparams;
   private readonly EMParameters emparams;
   private readonly Dataset dataset;

   public int Steps => nnparams.Steps;
   public int Epochs => nnparams.Epochs;
   public float LearningRate => (float)nnparams.LearningRate;
   public int DisplayStep => nnparams.DisplaySteps;
   public int BatchSize => nnparams.BatchSize;
   public int InputSize => nnparams.InputSize;
   public int OutputSize => nnparams.XIntervalCount * nnparams.ZIntervalCount;

   public NeuralNetworkHandler(NNParameters nnparams, EMParameters emparams, TextWriter? output = null)
   {
      Keras.Keras.DisablePySysConsoleLog = true;
      this.nnparams = nnparams;
      this.emparams = emparams;
      //tf_output_redirect = output;
      //session = tf.Session();

      dataset = new Dataset();

      generator = new(Steps,
              nnparams.InputSize,
              (nnparams.MinX, nnparams.MaxX, nnparams.MinZ, nnparams.MaxZ),
              nnparams.XIntervalCount, nnparams.ZIntervalCount, emparams.RecCount, emparams.RecX0, emparams.RecX1);
   }
   DatasetGenerator generator;
   public void GenerateData()
   {
      generator.GenerateData((nnparams.Px, nnparams.Pz), nnparams.AnomalyLen);

      dataset.SetSize = generator.Models.Count;

      FileLogger.Logger.DebugLog($"Model generation started");

      foreach (var model in generator.Models)
      {

         var Bx = model.B.Select(x => x.x).ToArray();
         var Bz = model.B.Select(x => x.z).ToArray();

         var x = new double[model.B.Length * 2];

         for (int i = 0; i < model.B.Length; i++)
         {
            x[i] = Bx[i];
            x[i + 1] = Bz[i];
         }

         var y = model.cells.Select(x => Math.Sqrt(x.PX * x.PX + x.PZ * x.PZ)).ToArray();

         dataset.AddData(x, y);

      }

      FileLogger.Logger.DebugLog($"Model generation ended, generated {generator.Models.Count} models");
   }

   public float[] Predict(double[] x)
   {
      if (!modelPrepared)
         PrepareModel();

      NDarray npx = np.array(x, ndmin: 2);
      
      NDarray res = model.Predict(npx);
      return res.GetData<float>();
   }

   public void TrainModel()
   {

      //Task.Run(() =>
      {
         var train_data = dataset.GetData();
         FileLogger.Logger.DebugLog("Fitting");

         var param = model.Fit(train_data.X,
                               train_data.Y,
                               batch_size: BatchSize,
                               epochs: Epochs,
                               steps_per_epoch: Steps,
                               validation_split: 0.2f).HistoryLogs;
         string pars = string.Empty;

         foreach (var p in param)
            pars += $"{p.Key} : {string.Join(", ", p.Value)}\n";   
            
         FileLogger.Logger.DebugLog("History: \n" + pars);

         pars = "Final: \n";

         foreach (var p in param)
            pars += $"{p.Key} : {p.Value.Last()}\n";

         FileLogger.Logger.DebugLog(pars);

        }//);
   }

   bool modelPrepared = false;
   public void PrepareModel()
   {
      generator = new(Steps,
                 nnparams.InputSize,
                 (nnparams.MinX, nnparams.MaxX, nnparams.MinZ, nnparams.MaxZ),
                 nnparams.XIntervalCount, nnparams.ZIntervalCount, emparams.RecCount, emparams.RecX0, emparams.RecX1);

      FileLogger.Logger.DebugLog("NN Model composing.");
            
      model = new Sequential();
      model.Add(new Dense(InputSize * 2, activation: "relu", input_dim: InputSize * 2));
      model.Add(new Dense(OutputSize * 2, activation: "relu"));
      model.Add(new Dense(OutputSize, activation: "relu"));

      model.Compile(optimizer: new SGD(), loss: "MeanSquaredError", metrics: ["accuracy"]);

      modelPrepared = true;

   }
   public void SaveModel() => model?.SaveWeight("./Models/model");//model.save("./Models/model.model");

   internal void LoadModel()
   {
      model?.LoadWeight("./Models/model");
   }
}


public class Dataset
{
   class Data
   {
      public NDarray X, Y;

      public Data(double[] xs, double[] ys)
      {
         X = np.array(xs);
         Y = np.array(ys);
      }
   }

   private const string DataPath = "./NeuralNetwork/Data/data";
   List<Data> datas = [];
   public int SetSize;
   public int Size => datas.Count;
   public string Path = string.Empty;
   public void AddData(double[] x, double[] y) => datas.Add(new Data(x, y));
   public (NDarray X, NDarray Y) GetData()
   {
      NDarray X = np.array(datas.Select(x => x.X).ToArray());
      NDarray Y = np.array(datas.Select(x => x.Y).ToArray());

      return (X, Y);
   }

   public void Load()
   {
      using BinaryWriter file = new BinaryWriter(File.OpenWrite(DataPath));

      try
      {
         if (datas.Count == 0) return;

         file.Write(Size);
         file.Write(datas[0].X.size);
         file.Write(datas[0].Y.size);
         foreach (var data in datas)
         {
            var xs = data.X.GetData<double>();
            var ys = data.Y.GetData<double>();

            foreach (var x in xs)
               file.Write(x);

            foreach (var y in ys)
               file.Write(y);
         }

      }
      catch (Exception e)
      {
         MessageBox.Show(e.Message);
         Console.WriteLine(e.Message);
      }

   }

   public void Save()
   {
      using BinaryReader file = new BinaryReader(File.OpenWrite(DataPath));

      try
      {
         int N = file.ReadInt32();
         int Nx = file.ReadInt32();
         int Ny = file.ReadInt32();

         for (int i = 0; i < N; i++)
         {
            double[] x = new double[Nx], y = new double[Ny];
            for (int j = 0; j < Nx; j++)
               x[j] = file.ReadDouble();

            for (int j = 0; j < Ny; j++)
               y[j] = file.ReadDouble();

            var data = new Data(x, y);
         }

      }
      catch (Exception e)
      {
         MessageBox.Show(e.Message);
         Console.WriteLine(e.Message);
      }

   }

}