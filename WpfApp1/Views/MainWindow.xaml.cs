using System;
using Fluent;
using MKT_Interface.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using WpfApp1.EM;
using OpenTK.Graphics.OpenGL4;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using MeshVisualizator;
using OpenTK.Mathematics;
using OpenTK.Text;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using Plot.Function;
using Plot.Viewport;
using System.Windows.Media;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
{
   private ViewModel viewModel;
   public ViewModel ViewModel
   {
      get { return viewModel; }
      set
      {
         viewModel = value;
         OnPropertyChanged(nameof(ViewModel));
      }
   }
   public MainWindow()
   {
      InitializeComponent();

      ViewModel = new ViewModel();
      directxIntervals.ItemsSource = ViewModel.DirectTask.XIntervals;
      directzIntervals.ItemsSource = ViewModel.DirectTask.ZIntervals;
      directAreas.ItemsSource = ViewModel.DirectTask.Areas;

      reversexIntervals.ItemsSource = ViewModel.ReverseTask.XIntervals;
      reversezIntervals.ItemsSource = ViewModel.ReverseTask.ZIntervals;
      reverseAreas.ItemsSource = ViewModel.ReverseTask.Areas;

      manager = new MagnetismManager();

      gl.Start(new GLWpfControlSettings()
      {
         MajorVersion = 4,
         MinorVersion = 5,
         GraphicsProfile = ContextProfile.Core,
      });
      GL.Enable(EnableCap.LineSmooth);
      GL.GetFloat(GetPName.AliasedLineWidthRange, new float[] { 0, 10 });
      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.LineWidth(1);
   }

   private void createDirectCFG_Click(object sender, RoutedEventArgs e)
   {
      ViewModel.DirectTask.PrintCFG("../../../DirectTask.cfg");

   }

   private void createReverseCFG_Click(object sender, RoutedEventArgs e)
   {
      ViewModel.ReverseTask.PrintCFG("../../../ReverseTask.cfg");

      MagnetismManager.MakeReverse("..\\..\\..\\EM\\DirectTask.cfg", "..\\..\\..\\EM\\Recs.txt", "..\\..\\..\\Cells.txt", double.Parse(Alpha.Text));
      manager.GetRecieverData("..\\..\\..\\Recs.txt");
      num_func = manager.GetRecieversDataOnPlane(true, 0);
      num_func.Prepare();

      manager.ReadCells("..\\..\\..\\Cells.txt");
      cell_func = manager.GetMagnetismData(true);
      //cell_func.Prepare();

      (double min, double max) = (num_func.GetDomain().Min.X, num_func.GetDomain().Max.X);
      for (int i = 0; i < p1XVals.Length - 1; i++)
      {
         var ix = min + i * (max - min) / 8;
         p1XVals[i].SetText(ix.ToString("g3"));
      }
      p1XVals[^1].SetText(max.ToString("g3"));

      (min, max) = (num_func.GetDomain().Min.Y, num_func.GetDomain().Max.Y);
      for (int i = 0; i < p1YVals.Length - 1; i++)
      {
         var iy = min + i * (max - min) / 8;
         p1YVals[i].SetText(iy.ToString("g4"));
      }
      p1YVals[^1].SetText(max.ToString("g4"));

      //(min, max) = (cell_func.GetDomain().Min.X, cell_func.GetDomain().Max.X);
      //for (int i = 0; i < p2XVals.Length - 1; i++)
      //{
      //   var ix = min + i * (max - min) / 8;
      //   p2XVals[i].SetText(ix.ToString("g3"));
      //}
      //p2XVals[^1].SetText(max.ToString("g3"));
      //
      //(min, max) = (cell_func.GetDomain().Min.Y, cell_func.GetDomain().Max.Y);
      //for (int i = 0; i < p2YVals.Length - 1; i++)
      //{
      //   var iy = min + i * (max - min) / 8;
      //   p2YVals[i].SetText(iy.ToString("g4"));
      //}
      //p2YVals[^1].SetText(max.ToString("g4"));

      //viewModel.ReDrawPalette(cell_func.min, cell_func.max, Color.FromRgb(63, 63, 63), Color.FromRgb(195, 195, 195));

   }
   private void EnterRecievers_Click(object sender, RoutedEventArgs e)
   {
      double leftX = double.Parse(receiverBegX.Text);
      double rightX = double.Parse(receiverEndX.Text);
      int n = int.Parse(receiverCount.Text);

      MagnetismManager.MakeDirect("..\\DirectTask.cfg", leftX, rightX, n, "..\\Recs.txt");
      manager.GetTrueCells("..\\DirectTask.cfg", "..\\CellsTrue.txt");
      manager.ReadCells("..\\..\\..\\CellsTrue.txt");
      cell_func = manager.GetMagnetismData(true);
      cell_func.Color0 = (viewModel.Palette.Color1.R / 255f, viewModel.Palette.Color1.G / 255f, viewModel.Palette.Color1.B / 255f);
      cell_func.Color1 = (viewModel.Palette.Color2.R / 255f, viewModel.Palette.Color2.G / 255f, viewModel.Palette.Color2.B / 255f);

      cell_func.Prepare();

      (double min, double max) = (cell_func.GetDomain().Min.X, cell_func.GetDomain().Max.X);
      for (int i = 0; i< p2XVals.Length - 1; i++)
      {
         var ix = min + i * (max - min) / 8;
         p2XVals[i].SetText(ix.ToString("g3"));
      }
      p2XVals[^1].SetText(max.ToString("g3"));

      (min, max) = (cell_func.GetDomain().Min.Y, cell_func.GetDomain().Max.Y);
      for (int i = 0; i < p2YVals.Length - 1; i++)
      {
         var iy = min + i * (max - min) / 8;
         p2YVals[i].SetText(iy.ToString("g4"));
      }
      p2YVals[^1].SetText(max.ToString("g4"));



      viewModel.ReDrawPalette(cell_func.min, cell_func.max, Color.FromRgb(63, 63, 63), Color.FromRgb(195, 195, 195));

 
   }
   public event PropertyChangedEventHandler? PropertyChanged;
   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   #region GL

   private MagnetismManager manager;
   private Function2D num_func;
   private FunctionCell3D cell_func;
   private PlotView plotl, plotr;
   private TextRenderer[] axisNames, p1XVals, p1YVals, p2XVals, p2YVals;
   private void gl_Loaded(object sender, RoutedEventArgs e)
   {
      GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      GL.Enable(EnableCap.Blend);
      GL.Enable(EnableCap.LineSmooth);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      Camera2D.Instance.Position.X = (float)gl.ActualWidth / 2f;
      Camera2D.Instance.Position.Y = (float)gl.ActualHeight / 2f;
      Camera2D.Instance.Position = -Vector3.UnitZ;

      plotl = new PlotView { Margin = (20, (int)gl.ActualWidth / 2 - 30, 30, 30) };
      plotr = new PlotView { Margin = (20 + (int)gl.ActualWidth / 2, (int)gl.ActualWidth / 2 - 40, 30, 30) };
      Axis.TickMaxSize = 15;

      {
         int p1x0 = plotl.Margin.x0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualWidth / 2,
            p1x1 = p1x0 + plotl.Margin.x1 - Axis.Margin - Axis.TickMaxSize,

            p1y0 = plotl.Margin.y0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualHeight / 2,
            p1y1 = p1y0 - (plotl.Margin.y1 * 2 + Axis.Margin) - Axis.TickMaxSize + (int)gl.ActualHeight;

         int p2x0 = plotr.Margin.x0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualWidth / 2,
            p2x1 = p2x0 + plotr.Margin.x1 - Axis.Margin - Axis.TickMaxSize,

            p2y0 = plotr.Margin.y0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualHeight / 2,
            p2y1 = p2y0 - (plotr.Margin.y1 * 2 + Axis.Margin) - Axis.TickMaxSize + (int)gl.ActualHeight;

         var axistparams = new TextParams()
         {
            Color = Color4.Black,
            FontSize = 14,
            TextFontFamily = System.Drawing.FontFamily.GenericMonospace,
            CharXSpacing = 6
         };
         var tparams = new TextParams()
         {
            Color = Color4.Black,
            FontSize = 8,
            TextFontFamily = System.Drawing.FontFamily.GenericMonospace,
            CharXSpacing = 3,
         };
         axisNames = new[]
         {
            new TextRenderer("Bz", axistparams).SetCoordinates((p1x0 - Axis.TickMaxSize, p1y1 + 2)),
            new TextRenderer("X", axistparams).SetCoordinates((p1x1 + 2, p1y0 - (float)axistparams.FontSize / 2)),
            new TextRenderer("Z", axistparams).SetCoordinates((p2x0 - Axis.TickMaxSize, p2y1 + 2)),
            new TextRenderer("X", axistparams).SetCoordinates((p2x1 + 2, p2y0 - (float)axistparams.FontSize / 2)),
         };
         p1XVals = new TextRenderer[9];
         p1YVals = new TextRenderer[9];
         p2XVals = new TextRenderer[9];
         p2YVals = new TextRenderer[9];

         for (int i = 0; i < 9; i++)
         {
            float p1vx = (p1x1 - p1x0) / 8f;
            float p1vy = (p1y1 - p1y0) / 8f;

            float p2vx = (p2x1 - p2x0) / 8f;
            float p2vy = (p2y1 - p2y0) / 8f;

            p1XVals[i] = new TextRenderer("0.0E-0", tparams)
               .SetCoordinates((p1x0 + p1vx * i - 10,
                                   p1y0 - Axis.TickMaxSize - tparams.FontSize - 2));
            p1YVals[i] = new TextRenderer("0.0E-0", tparams)
               .SetCoordinates((p1x0 - Axis.TickMaxSize - (tparams.FontSize - tparams.CharXSpacing) * "0.0E-0".Length, p1y0 + p1vy * i));

            p2XVals[i] = new TextRenderer("0.0E-0", tparams)
               .SetCoordinates((p2x0 + p2vx * i - 10,
                                   p2y0 - Axis.TickMaxSize - tparams.FontSize - 2));
            p2YVals[i] = new TextRenderer("0.0E-0", tparams)
               .SetCoordinates((p1x1 + plotl.Margin.x0 + 2, p2y0 + p2vy * i));

         }
      }
      var s = ((int)gl.ActualWidth, (int)gl.ActualHeight);
      plotl?.SetAxes(s);
      plotr?.SetAxes(s);
   }

   private void LoadDirectTaskValues_Click(object sender, RoutedEventArgs e)
   {


   }

   private void gl_OnRender(TimeSpan delta)
   {
      GL.ClearColor(Color4.White);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      if (gl.ActualWidth > 0 && gl.ActualHeight > 0 && axisNames != null)
      {
         foreach (var axisName in axisNames)
            axisName.DrawText(false);

         foreach (var x in p1XVals)
            x.DrawText(false);
         foreach (var x in p1YVals)
            x.DrawText(false);
         foreach (var x in p2XVals)
            x.DrawText(false);
         foreach (var x in p2YVals)
            x.DrawText(false);


         plotl?.DrawPlotView(((int)gl.ActualWidth, (int)gl.ActualHeight), () =>
          {
             num_func?.Draw(Color4.HotPink, num_func.GetDomain());
          });
         plotr?.DrawPlotView(((int)gl.ActualWidth, (int)gl.ActualHeight), () =>
         {
            //cell_func?.Draw(Color4.Black, cell_func.GetDomain());
         });
            //cell_func?.Draw(Color4.Black, cell_func.GetDomain());
      }
      //tr?.SetCoordinates(mouseCoords).SetText(mouseCoords.ToString()).DrawText(false);
      GL.Finish();
   }
   private void gl_SizeChanged(object sender, SizeChangedEventArgs e)
   {

      var s = ((int)e.NewSize.Width, (int)e.NewSize.Height);
      GL.Viewport(0, 0, s.Item1, s.Item2);
      Camera2D.Instance.Size = s;
      plotl?.SetAxes(s);
      plotr?.SetAxes(s);

      if (plotl is null) return;
      plotl.Margin = (20, (int)gl.ActualWidth / 2 - 30, 30, 30);
      if (plotr is not null)
         plotr.Margin = (20 + (int)gl.ActualWidth / 2, (int)gl.ActualWidth / 2 - 40, 30, 30);

      int p1x0 = plotl.Margin.x0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualWidth / 2,
         p1x1 = p1x0 + plotl.Margin.x1 - Axis.Margin - Axis.TickMaxSize,

         p1y0 = plotl.Margin.y0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualHeight / 2,
         p1y1 = p1y0 - (plotl.Margin.y1 * 2 + Axis.Margin) - Axis.TickMaxSize + (int)gl.ActualHeight;

      int p2x0 = plotr.Margin.x0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualWidth / 2,
         p2x1 = p2x0 + plotr.Margin.x1 - Axis.Margin - Axis.TickMaxSize,

         p2y0 = plotr.Margin.y0 + Axis.Margin + Axis.TickMaxSize - (int)gl.ActualHeight / 2,
         p2y1 = p2y0 - (plotr.Margin.y1 * 2 + Axis.Margin) - Axis.TickMaxSize + (int)gl.ActualHeight;


      axisNames[0].SetCoordinates((p1x0 - Axis.TickMaxSize, p1y1 + 2));
      axisNames[1].SetCoordinates((p1x1 + 2, p1y0 - (float)axisNames[1].Params.FontSize / 2));
      axisNames[2].SetCoordinates((p2x0 - Axis.TickMaxSize, p2y1 + 2));
      axisNames[3].SetCoordinates((p2x1 + 2, p2y0 - (float)axisNames[3].Params.FontSize / 2));

      for (int i = 0; i < 9; i++)
      {
         float p1vx = (p1x1 - p1x0) / 8;
         float p1vy = (p1y1 - p1y0) / 8;

         float p2vx = (p2x1 - p2x0) / 8;
         float p2vy = (p2y1 - p2y0) / 8;


         p1XVals[i].SetCoordinates((p1x0 + p1vx * i - 10,
               p1y0 - Axis.TickMaxSize - p2XVals[i].Params.FontSize - 2));
         p1YVals[i]
            .SetCoordinates((p1x0 - Axis.TickMaxSize - (p1YVals[i].Params.FontSize - p1YVals[i].Params.CharXSpacing) * "0.0E-0".Length, p1y0 + p1vy * i));


         p2XVals[i].SetCoordinates((p2x0 + p2vx * i - 10,
                p2y0 - Axis.TickMaxSize - p2XVals[i].Params.FontSize - 2));
         p2YVals[i].SetCoordinates((p1x1 + plotl.Margin.x0 + 2, p2y0 + p2vy * i));

      }
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

}
