using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MKT_Interface.NeuralNetwork;

public class NNParameters : INotifyPropertyChanged
{
   static Dictionary<NNStatus, string> StatusAsString = new()
   {
      [NNStatus.Idle] = "Ожидание",
      [NNStatus.GeneratingData] = "Генерация датасета",
      [NNStatus.Training] = "Обучение",
      [NNStatus.Ready] = "Обучилось",
      [NNStatus.Error] = "Ошибка",
   };
   public enum NNStatus
   {
      Idle = 0,
      GeneratingData,
      Training,
      Ready,
      Error
   }

   public NNStatus nnstatus = NNStatus.Idle;
   public NNStatus NNstatus 
   { 
      get => nnstatus; 
      set 
      { 
         nnstatus = value; 
         Status = StatusAsString[value]; 
      }
   }

   private string status = StatusAsString[NNStatus.Idle];
   public string Status
   {
      get { return status; }
      set { status = value; OnPropertyChanged(); }
   }

   private double learningRate = 1e-3;
   public double LearningRate
   {
      get => learningRate;
      set
      {
         if (value < 1e-12)
            learningRate = 1e-12;
         else
            learningRate = value;
         OnPropertyChanged();
      }
   }

   private int batchSize = 10;
   public int BatchSize
   {
      get => batchSize;
      set
      {
         batchSize = value;
         OnPropertyChanged();
      }
   }
   private int displaySteps = 10;

   public int DisplaySteps
   {
      get { return displaySteps; }
      set { displaySteps = value; OnPropertyChanged(); }
   }

   private int steps = 3000;
   public int Steps
   {
      get { return steps; }
      set { steps = value; OnPropertyChanged(); }
   }

   private double px = 1d;

   public double Px
   {
      get => px;
      set { px = value; OnPropertyChanged(); }
   }
   private double pz = 1d;
   public double Pz
   {
      get => pz;
      set { pz = value; OnPropertyChanged(); }
   }
   private double anomalyLen = 2d;
   public double AnomalyLen
   {
      get => anomalyLen;
      set { anomalyLen = value; OnPropertyChanged(); }
   }

   private int inputSize = 25;

   public int InputSize
   {
      get => inputSize;
      set { inputSize = value; OnPropertyChanged(); }
   }

   private double minX = -100;
   public double MinX
   {
      get => minX;
      set
      {
         minX = value;
         OnPropertyChanged();
      }
   }
   private double maxX = 100;
   public double MaxX
   {
      get => maxX;
      set
      {
         maxX = value;
         OnPropertyChanged();
      }
   }
   private double minZ = -200;
   public double MinZ
   {
      get => minZ;
      set
      {
         minZ = value;
         OnPropertyChanged();
      }
   }
   private double maxZ = -100;
   public double MaxZ
   {
      get => maxZ;
      set
      {
         maxZ = value;
         OnPropertyChanged();
      }
   }

   private int xIntervalCount = 20;
   public int XIntervalCount
   {
      get => xIntervalCount;
      set
      {
         xIntervalCount = value;
         OnPropertyChanged();
      }
   }
   private int zIntervalCount = 20;
   public int ZIntervalCount
   {
      get => zIntervalCount;
      set
      {
         zIntervalCount = value;
         OnPropertyChanged();
      }
   }
   private int epochs = 10;
   public int Epochs
   {
      get => epochs;
      set
      {
         epochs = value;
         OnPropertyChanged();
      }
   }

   public event PropertyChangedEventHandler? PropertyChanged;
   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
}
