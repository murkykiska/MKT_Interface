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
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        var cells = manager.GetBox2Cells();
        funcPtrue.SetCells(cells.boxCells, cells.values);

        var result = manager.MakeDirect();
        funcBx.FillPoints(result.x, result.Bx);
        funcBx.Prepare();
        funcBz.FillPoints(result.x, result.Bz);
        funcBz.Prepare();

        plotManager.ResetPlots(plotManager.ViewportSize);
        viewModel.ReDrawPalette(funcPtrue.Min, funcPtrue.Max, Color.FromRgb(63, 63, 63), Color.FromRgb(195, 195, 195));
    }


    private void train_Click(object sender, RoutedEventArgs e)
    {

    }

    private void fit_Click(object sender, RoutedEventArgs e)
    {

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

        Camera2D.Instance.Position.X = (float)gl.ActualWidth / 2f;
        Camera2D.Instance.Position.Y = (float)gl.ActualHeight / 2f;
        Camera2D.Instance.Position = -Vector3.UnitZ;

        funcBx = new Function2D(lineType: Function2D.LineTypes.Continious);
        List<Vector2> points = new();
        for (float i = -0; i <= 300; i += 1)
            points.Add(new Vector2(i / 20f, MathF.Cos(i / 20f) * 20));
        funcBx.FillPoints(points.ToArray());
        funcBx.Prepare();

        funcBz = new Function2D(lineType: Function2D.LineTypes.Continious);
        points = new();
        for (float i = -0; i <= 300; i+=1)
            points.Add(new Vector2(i / 20f, MathF.Sin(i / 20f) * 15) );
        funcBz.FillPoints(points.ToArray());
        funcBz.Prepare();

        funcPtrue = new FunctionCell2D();
        //x0 y0 x1 y1
        funcPtrue.SetCells([ new Box2(0, 0, 2, 3),
                             new Box2(2, 3, 3, 4),
                             new Box2(2, 0, 3, 3),
                             new Box2(0, 3, 2, 4) ],
                             [1f, 2f, 3f, 4f]);
        //cell_func.SetCells(new[] { new Box2(0, 0, 1, 2) }, new[] { 1f });

        funcPtrue.Color0 = (viewModel.Palette.Color1.R / 255f, viewModel.Palette.Color1.G / 255f, viewModel.Palette.Color1.B / 255f);
        funcPtrue.Color1 = (viewModel.Palette.Color2.R / 255f, viewModel.Palette.Color2.G / 255f, viewModel.Palette.Color2.B / 255f);

        funcPtrue.Prepare();

        var plotl = new PlotView("X", "T") { Margin = (25, 35, 30, 30),
            DrawFunction =
            (Box2 DrawArea) =>
            {
                GL.LineWidth(3);             
                if (funcBx is null || funcBz is null) return default;
                Box2 sharedDomain = new Box2(MathF.Min(funcBx.Domain.Min.X, funcBz.Domain.Min.X),
                                             MathF.Min(funcBx.Domain.Min.Y, funcBz.Domain.Min.Y),
                                             MathF.Max(funcBx.Domain.Max.X, funcBz.Domain.Max.X),
                                             MathF.Max(funcBx.Domain.Max.Y, funcBz.Domain.Max.Y));

                funcBx?.Draw(Color4.Firebrick, DrawArea);
                funcBz?.Draw(Color4.CornflowerBlue, DrawArea);

                return sharedDomain;
            }
        };

        var plotr = new PlotView("X", "Z") { Margin = (25, 35, 30, 30),
            DrawFunction = 
            (Box2 DrawArea) =>
            {
                GL.LineWidth(0.5f);
                funcPtrue?.Draw(Color4.Black, DrawArea);
                return funcPtrue?.Domain ?? default;
            }
        };
        Axis.TickMaxSize = 15;

        var s = ((int)gl.ActualWidth, (int)gl.ActualHeight);
        plotManager.ViewportSize = s;
        plotManager.SetupPlots([plotl, plotr], 2, 1);
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
