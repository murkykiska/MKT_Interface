using Fluent;
using MKT_Interface.Plot;
using MKT_Interface.ViewModels;
using OpenTK.Mathematics;
using OpenTK.Text;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using Plot.Function;
using Plot.Viewport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        //manager = new MagnetismManager();

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
        GL.LineWidth(2);
    }

    private void createDirectCFG_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.DirectTask.PrintCFG("../../../DirectTask.cfg");

    }

    private void createReverseCFG_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ReverseTask.PrintCFG("../../../ReverseTask.cfg");

        //MagnetismManager.MakeReverse("..\\..\\..\\EM\\DirectTask.cfg", "..\\..\\..\\EM\\Recs.txt", "..\\..\\..\\Cells.txt", double.Parse(Alpha.Text));
        //manager.GetRecieverData("..\\..\\..\\Recs.txt");
        //num_func = manager.GetRecieversDataOnPlane(true, 0);
        //num_func.Prepare();

        //manager.ReadCells("..\\..\\..\\Cells.txt");
        //cell_func = manager.GetMagnetismData(true);
        //cell_func.Prepare();


    }
    private void EnterRecievers_Click(object sender, RoutedEventArgs e)
    {
        double leftX = double.Parse(receiverBegX.Text);
        double rightX = double.Parse(receiverEndX.Text);
        int n = int.Parse(receiverCount.Text);

        //MagnetismManager.MakeDirect("..\\DirectTask.cfg", leftX, rightX, n, "..\\Recs.txt");
        //manager.GetTrueCells("..\\DirectTask.cfg", "..\\CellsTrue.txt");
        //manager.ReadCells("..\\..\\..\\CellsTrue.txt");
        //cell_func = manager.GetMagnetismData(true);


        viewModel.ReDrawPalette(cell_func.Min, cell_func.Max, Color.FromRgb(63, 63, 63), Color.FromRgb(195, 195, 195));



    }
    public event PropertyChangedEventHandler? PropertyChanged = null!;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region GL

    //private MagnetismManager manager;
    private Function2D num_func = null!;
    private FunctionCell2D cell_func = null!;
    private PlotManager plotManager = PlotManager.Instance;
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

        num_func = new Function2D();
        List<Vector2> points = new();
        for (float i = -0; i <= 300; i+=1)
            points.Add(new Vector2(i / 20f, MathF.Sin(i / 20f) * 20) );
        num_func.FillPoints(points.ToArray());
        num_func.Prepare();


        cell_func = new FunctionCell2D();
        //x0 y0 x1 y1
        cell_func.SetCells(new[] { new Box2(0, 0, 2, 3),
                                   new Box2(2, 3, 3, 4),
                                   new Box2(2, 0, 3, 3),
                                   new Box2(0, 3, 2, 4) },
                                   new[] { 1f, 2f, 3f, 4f });
        //cell_func.SetCells(new[] { new Box2(0, 0, 1, 2) }, new[] { 1f });

        cell_func.Color0 = (viewModel.Palette.Color1.R / 255f, viewModel.Palette.Color1.G / 255f, viewModel.Palette.Color1.B / 255f);
        cell_func.Color1 = (viewModel.Palette.Color2.R / 255f, viewModel.Palette.Color2.G / 255f, viewModel.Palette.Color2.B / 255f);

        cell_func.Prepare();

        PlotView plotl = new PlotView("X", "Bz") { Margin = (25, 35, 30, 30),
            DrawFunction =
            (Box2 DrawArea) =>
            {
                GL.LineWidth(3);             
                num_func?.Draw(Color4.HotPink, DrawArea);
                return num_func?.Domain ?? default;
            }
        };
        var plotr = new PlotView("X", "Z") { Margin = (25, 35, 30, 30),
            DrawFunction = 
            (Box2 DrawArea) =>
            {
                GL.LineWidth(4);
                cell_func?.Draw(Color4.Orange, DrawArea);
                return cell_func?.Domain ?? default;
            }
        };
        Axis.TickMaxSize = 15;

        var s = ((int)gl.ActualWidth, (int)gl.ActualHeight);
        plotManager.ViewportSize = s;
        plotManager.SetupPlots(new[] { plotl, plotr }, 2, 1);
    }

    private void LoadDirectTaskValues_Click(object sender, RoutedEventArgs e)
    {


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

}
