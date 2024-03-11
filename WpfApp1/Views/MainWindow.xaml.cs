using Fluent;
using MKT_Interface.Plot;
using MKT_Interface.ViewModels;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using Plot.Function;
using Plot.Viewport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp1.EM;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
{
   private ViewModel viewModel = null!;
   public ViewModel ViewModel
   {
      get { return viewModel; }
      set
      {
         viewModel = value;
         OnPropertyChanged();
      }
   }

   private MagnetismManager manager = null!;
   private Function2D funcBz = null!;
   private Function2D funcBx = null!;
   private Function2D funcSmoothBz = null!;
   private Function2D funcSmoothBx = null!;
   private FunctionCell2D funcPtrue = null!;
   private FunctionCell2D funcPcalc = null!;
   private PlotManager plotManager = PlotManager.Instance;
   public MainWindow()
   {

      ViewModel = new ViewModel();
      manager = new MagnetismManager(ViewModel.EMParams, ViewModel.NNParams); //"NeuralNetwork\\Models\\model.model"
      InitializeComponent();

      directxIntervals.ItemsSource = ViewModel.EMParams.XIntervals;
      directzIntervals.ItemsSource = ViewModel.EMParams.ZIntervals;
      directAreas.ItemsSource = ViewModel.EMParams.Areas;

      //GL 
      {
         gl.Start(new GLWpfControlSettings()
         {
            MajorVersion = 4,
            MinorVersion = 5,
            GraphicsProfile = ContextProfile.Core,
         });

         GL.Enable(EnableCap.LineSmooth);
         GL.GetFloat(GetPName.AliasedLineWidthRange, [0, 10]);
         GL.Enable(EnableCap.DebugOutput);
         GL.Enable(EnableCap.DebugOutputSynchronous);
         GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
         GL.LineWidth(2);
      }
   }

   private void EnterRecievers_Click(object sender, RoutedEventArgs e)
   {
      var (boxCells, values) = manager.GetBox2Cells();
      funcPtrue.SetCells(boxCells, values);

      var (x, Bx, Bz) = manager.MakeDirect();
      funcBx.FillPoints(x, Bx);
      funcBx.Prepare();
      funcBz.FillPoints(x, Bz);
      funcBz.Prepare();

      manager.FillSmoothedFunc2D(funcBx, funcSmoothBx);
      manager.FillSmoothedFunc2D(funcBz, funcSmoothBz);

      plotManager.ResetPlots(plotManager.ViewportSize);
      viewModel.ReDrawTruePalette(funcPtrue.Min, funcPtrue.Max, viewModel.TruePalette.Color1, viewModel.TruePalette.Color2);

   }

   private void train_Click(object sender, RoutedEventArgs e)
   {
      manager.TrainModel();
   }

   private void fit_Click(object sender, RoutedEventArgs e)
   {

      try
      {
         // 1. generate model

         var (boxCells, values) = manager.GetBox2Cells(cells);
         manager.FillFunc2D(funcPtrue, true);

         funcPtrue.SetCells(boxCells, values);

         manager.FillSmoothedFunc2D(funcBx, funcSmoothBx);
         manager.FillSmoothedFunc2D(funcBz, funcSmoothBz);

         // 2. predict

         Bcalc = manager.Predict(funcSmoothBx, funcSmoothBz);
         funcPcalc.SetCells(boxCells, ViewModel.EMParams.ShowPx ? Bcalc.Bx : Bcalc.Bz);

         // 3. draw model
         // 4. draw fitted model

         viewModel.ReDrawTruePalette(funcPtrue.Min, funcPtrue.Max, viewModel.TruePalette.Color1, viewModel.TruePalette.Color2);
         viewModel.ReDrawCalcPalette(funcPcalc.Min, funcPcalc.Max, viewModel.CalcPalette.Color1, viewModel.CalcPalette.Color2);

      }
      catch (Exception ex)
      {
         Logger.FileLogger.Logger.ErrorLog(ex.StackTrace ?? "");
         Logger.FileLogger.Logger.DebugLog(ex.Message);
         manager.SetErrorStatus();
      }

   }

   List<MKT_Interface.Models.Cell> cells;
   (float[] Bx, float[] Bz) B;
   (float[] Bx, float[] Bz) Bcalc;
   float[] xs;

   private void generate_Click(object sender, RoutedEventArgs e)
   {
      (cells, B, xs) = manager.GenerateModel();

      var (boxCells, values) = manager.GetBox2Cells(cells);
      funcPtrue.SetCells(boxCells, values);

      //var (result.xs, Bx, Bz) = manager.MakeDirect();
      funcBx.FillPoints(xs, B.Bx);
      funcBx.Prepare();
      funcBz.FillPoints(xs, B.Bz);
      funcBz.Prepare();

      manager.FillSmoothedFunc2D(funcBx, funcSmoothBx);
      manager.FillSmoothedFunc2D(funcBz, funcSmoothBz);

      plotManager.ResetPlots(plotManager.ViewportSize);
      viewModel.ReDrawTruePalette(funcPtrue.Min, funcPtrue.Max, viewModel.TruePalette.Color1, viewModel.TruePalette.Color2);
      viewModel.ReDrawCalcPalette(funcPcalc.Min, funcPcalc.Max, viewModel.CalcPalette.Color1, viewModel.CalcPalette.Color2);

   }
   private void load_Click(object sender, RoutedEventArgs e)
   {
      manager.LoadModel();
   }

   private void CheckBox_Checked(object sender, RoutedEventArgs e)
   {
      var (boxCells, values) = manager.GetBox2Cells(cells);
      funcPtrue.SetCells(boxCells, values);
      funcPcalc.SetCells(boxCells, ViewModel.EMParams.ShowPx ? Bcalc.Bx : Bcalc.Bz);

      manager.FillFunc2D(funcPtrue, true);
      //manager.FillFunc2D(funcPcalc, true);

      manager.FillSmoothedFunc2D(funcBx, funcSmoothBx);
      manager.FillSmoothedFunc2D(funcBz, funcSmoothBz);

      viewModel.ReDrawTruePalette(funcPtrue.Min, funcPtrue.Max, viewModel.TruePalette.Color1, viewModel.TruePalette.Color2);
      viewModel.ReDrawCalcPalette(funcPcalc.Min, funcPcalc.Max, viewModel.CalcPalette.Color1, viewModel.CalcPalette.Color2);
   }

   private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
   {
      var (boxCells, values) = manager.GetBox2Cells(cells);
      funcPtrue.SetCells(boxCells, values);
      funcPcalc.SetCells(boxCells, ViewModel.EMParams.ShowPx ? Bcalc.Bx : Bcalc.Bz);

      manager.FillFunc2D(funcPtrue, false);
      //manager.FillFunc2D(funcPcalc, false);

      manager.FillSmoothedFunc2D(funcBx, funcSmoothBx);
      manager.FillSmoothedFunc2D(funcBz, funcSmoothBz);

      viewModel.ReDrawTruePalette(funcPtrue.Min, funcPtrue.Max, viewModel.TruePalette.Color1, viewModel.TruePalette.Color2);
      viewModel.ReDrawCalcPalette(funcPcalc.Min, funcPcalc.Max, viewModel.CalcPalette.Color1, viewModel.CalcPalette.Color2);
   }

   #region GL

   private void gl_Loaded(object sender, RoutedEventArgs e)
   {
      GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      GL.Enable(EnableCap.Blend);
      GL.Enable(EnableCap.LineSmooth);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      #region Init camera
      Camera2D.Instance.Position.X = (float)gl.ActualWidth / 2f;
      Camera2D.Instance.Position.Y = (float)gl.ActualHeight / 2f;
      Camera2D.Instance.Position = -Vector3.UnitZ;
      #endregion

      #region Init plots, add dummy functions
      #region Bx 
      funcBx = new Function2D(lineType: Function2D.LineTypes.Continious);
      List<Vector2> points = [];
      for (float i = -0; i <= 300; i += 1)
         points.Add(new Vector2(i / 20f, MathF.Cos(i / 20f) * 20));
      funcBx.FillPoints(points.ToArray());
      funcBx.Prepare();

      funcSmoothBx = new Function2D(lineType: Function2D.LineTypes.Dashed);
      points = [];
      for (float i = -0; i <= 300; i += 1)
         points.Add(new Vector2(i / 20f, MathF.Tanh(i / 20f) * 25f));
      funcSmoothBx.FillPoints(points.ToArray());
      funcSmoothBx.Prepare();

      #endregion
      #region Bz
      funcBz = new Function2D(lineType: Function2D.LineTypes.Continious);
      points = [];
      for (float i = -0; i <= 300; i += 1)
         points.Add(new Vector2(i / 20f, MathF.Sin(i / 20f) * 15));
      funcBz.FillPoints(points.ToArray());
      funcBz.Prepare();

      funcSmoothBz = new Function2D(lineType: Function2D.LineTypes.Dashed);
      points = [];
      for (float i = -0; i <= 300; i += 1)
         points.Add(new Vector2(i / 20f, 1f / MathF.Tanh(i / 20f + 0.1f) * 20));
      funcSmoothBz.FillPoints(points.ToArray());
      funcSmoothBz.Prepare();

      #endregion
      #region true
      funcPtrue = new FunctionCell2D();

      funcPtrue.SetCells([new Box2(0, 0, 2, 3),
         new Box2(2, 3, 3, 4),
         new Box2(2, 0, 3, 3),
         new Box2(0, 3, 2, 4)],
         [1f, 2f, 3f, 4f]);

      funcPtrue.Color0 = (viewModel.TruePalette.Color1.R / 255f, viewModel.TruePalette.Color1.G / 255f, viewModel.TruePalette.Color1.B / 255f);
      funcPtrue.Color1 = (viewModel.TruePalette.Color2.R / 255f, viewModel.TruePalette.Color2.G / 255f, viewModel.TruePalette.Color2.B / 255f);
      funcPtrue.Prepare();
      #endregion
      #region calc
      funcPcalc = new FunctionCell2D();
      funcPcalc.SetCells([new Box2(0, 0, 2, 3),
         new Box2(2, 3, 3, 4),
         new Box2(2, 0, 3, 3),
         new Box2(0, 3, 2, 4)],
         [1f, 2f, 3f, 4f]);

      funcPcalc.Color0 = (viewModel.CalcPalette.Color1.R / 255f, viewModel.CalcPalette.Color1.G / 255f, viewModel.CalcPalette.Color1.B / 255f);
      funcPcalc.Color1 = (viewModel.CalcPalette.Color2.R / 255f, viewModel.CalcPalette.Color2.G / 255f, viewModel.CalcPalette.Color2.B / 255f);
      funcPcalc.Prepare();
      #endregion

      #endregion

      #region Set draw params for plots
      var plotul = new PlotView("X", "Bx")
      {
         Margin = (25, 20, 15, 15),
         DrawFunction =
          (Box2 DrawArea) =>
          {
             GL.LineWidth(2);
             if (funcBx is null || funcSmoothBx is null) return default;
             Box2 full = Function2D.Draw2Funcs(DrawArea, funcBx, funcSmoothBx, Color4.Firebrick, Color4.Coral);
                                                                               
             return full;
          }
      };

      var plotdl = new PlotView("X", "Bz")
      {
         Margin = (25, 20, 15, 15),
         DrawFunction =
          (Box2 DrawArea) =>
          {
             GL.LineWidth(2);
             if (funcBz is null || funcSmoothBx is null) return default;
             Box2 full = Function2D.Draw2Funcs(DrawArea, funcSmoothBz, funcBz, Color4.MediumPurple, Color4.CornflowerBlue);

             return full;
          }
      };

      var plotur = new PlotView("X", "Z (True)")
      {
         Margin = (25, 20, 15, 15),
         DrawFunction =
          (Box2 DrawArea) =>
          {
             GL.LineWidth(1f);
             funcPtrue?.Draw(Color4.Black, DrawArea);
             return funcPtrue?.Domain ?? default;
          }
      };

      var plotdr = new PlotView("X", "Z (NN)")
      {
         Margin = (25, 20, 15, 15),
         DrawFunction =
         (Box2 DrawArea) =>
         {
            GL.LineWidth(1f);
            funcPcalc?.Draw(Color4.Black, DrawArea);
            return funcPcalc?.Domain ?? default;
         }
      };

      Axis.TickMaxSize = 15;

      var s = ((int)gl.ActualWidth, (int)gl.ActualHeight);
      plotManager.ViewportSize = s;
      plotManager.SetupPlots([plotdl, plotdr, plotul, plotur], 2, 2);

      #endregion
   }

   private void gl_OnRender(TimeSpan delta)
   {
      GL.ClearColor(Color4.White);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      if (gl.ActualWidth > 0 && gl.ActualHeight > 0)
      {
         plotManager?.DrawPlots();
      }
      GL.Finish();
   }
   private void gl_SizeChanged(object sender, SizeChangedEventArgs e)
   {

      var s = ((int)e.NewSize.Width, (int)e.NewSize.Height);
      GL.Viewport(0, 0, s.Item1, s.Item2);
      Camera2D.Instance.Size = s;
      if (plotManager is null) return;

      plotManager.ResetPlots(s);
   }
   private void gl_Unloaded(object sender, RoutedEventArgs e)
   {

   }

   private Vector2 mouseCoords;
   private void Gl_OnMouseMove(object sender, MouseEventArgs e)
   {
      var pos = e.GetPosition(sender as IInputElement);
      mouseCoords = ((int)(pos.X - gl.ActualWidth / 2), (int)(gl.ActualHeight / 2 - pos.Y));
   }

   #endregion

   public event PropertyChangedEventHandler? PropertyChanged = null!;
   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

}
