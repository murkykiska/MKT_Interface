using System;
using Fluent;
using MKT_Interface.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows;
using WpfApp1.EM;
using OpenTK.Graphics.OpenGL4;
using System.Text;
using System.Windows.Media.Media3D;
using MeshVisualizator;
using OpenTK.Mathematics;
using OpenTK.Text;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using PlotTest.Viewport;

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

         gl.Start(new GLWpfControlSettings()
         {
            MinorVersion = 5,
            MajorVersion = 4,
            GraphicsProfile = ContextProfile.Core,
         });
         GL.Enable(EnableCap.LineSmooth);
         GL.GetFloat(GetPName.AliasedLineWidthRange, new float[] { 0, 10 });

         GL.LineWidth(1);
   }

     private void createDirectCFG_Click(object sender, RoutedEventArgs e)
     {
         ViewModel.DirectTask.PrintCFG("../../../DirectTask.cfg");

     }

     private void createReverseCFG_Click(object sender, RoutedEventArgs e)
     {
         ViewModel.ReverseTask.PrintCFG("../../../ReverseTask.cfg");
     }
     private void EnterRecievers_Click(object sender, RoutedEventArgs e)
     {
         double leftX = double.Parse(receiverBegX.Text);
         double rightX = double.Parse(receiverEndX.Text);
         int n = int.Parse(receiverCount.Text);

      MagnetismManager.MakeDirect("..\\DirectTask.cfg", leftX, rightX, n, "..\\Recs.txt");

   }
   public event PropertyChangedEventHandler? PropertyChanged;
     protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
     {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
     }

    private PlotView plotl, plotr;
    private TextRenderer tr;
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

      plotl = new PlotView { Margin = (10, (int)gl.ActualWidth / 2 - 10, 10, 10) };
      plotr = new PlotView { Margin = (10 + (int)gl.ActualWidth / 2, (int)gl.ActualWidth / 2 - 10, 10, 10) };
      tr = new TextRenderer("Huydsadasdasdasdasd",
         new TextParams()
         {
            Color = Color4.Chartreuse, FontSize = 14, TextFontFamily = System.Drawing.FontFamily.GenericMonospace,
            TextFontStyle = System.Drawing.FontStyle.Bold
         });
      tr.SetCoordinates((50, 50));

    }
   double secTicks = 0;
   string fps = "";
   private void gl_OnRender(TimeSpan delta)
   {
      GL.ClearColor(Color4.AntiqueWhite);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      if (gl.ActualWidth > 0 && gl.ActualHeight > 0)
      {
         plotl?.DrawPlotView(((int)gl.ActualWidth, (int)gl.ActualHeight), () =>
            {
               tr?.DrawText();
            });
         plotr?.DrawPlotView(((int)gl.ActualWidth, (int)gl.ActualHeight), () =>
         {
            GL.ClearColor(Color4.Crimson);
         });
      }
      tr?.DrawText();
      GL.Finish();
   }
   private void gl_SizeChanged(object sender, SizeChangedEventArgs e)
   {

      var s = ((int)e.NewSize.Width, (int)e.NewSize.Height);
      GL.Viewport(0,0, s.Item1, s.Item2);
      Camera2D.Instance.Size = s;
      plotl?.SetAxes(s);
      plotr?.SetAxes(s);

      if (plotl is not null) plotl.Margin = (10, (int)gl.ActualWidth / 2 - 10, 10, 10);
      if (plotr is not null) plotr.Margin = (10 + (int)gl.ActualWidth / 2, (int)gl.ActualWidth / 2 - 10, 10, 10);
   }
   private void gl_Unloaded(object sender, RoutedEventArgs e)
   {
      
   }
}
