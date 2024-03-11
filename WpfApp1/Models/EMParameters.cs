using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Math;
namespace MKT_Interface.Models;

public class EMParameters : INotifyPropertyChanged
{
    public class DoubleHandler(double _value) { public double Value { get => _value; set => _value = value; } };

    private ObservableCollection<Interval> xIntervals = 
    [
        new Interval() { IntervalsCount = 10, SparseRatio = 1 },
        new Interval() { IntervalsCount = 15, SparseRatio = 1.1 },
        new Interval() { IntervalsCount = 10, SparseRatio = 1 }
    ];

    public ObservableCollection<Interval> XIntervals
    {
        get => xIntervals;
        set
        {
            xIntervals = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<Interval> zIntervals =
    [
        new Interval() { IntervalsCount = 10, SparseRatio = 1 },
        new Interval() { IntervalsCount = 15, SparseRatio = 0.9 },
        new Interval() { IntervalsCount = 10, SparseRatio = 1 }
    ];
    public ObservableCollection<Interval> ZIntervals
    {
        get => zIntervals;
        set
        {
            zIntervals = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<Area> areas = [new Area() { IntervalXNum = 1, IntervalZNum = 1, PLength = 4 }];
    public ObservableCollection<Area> Areas
    {
        get => areas;
        set
        {
            areas = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<DoubleHandler> internalPtsX = [new(- 500d), new(-100d), new(100d), new(500d)];

    public ObservableCollection<DoubleHandler> InternalPtsX
    {
        get => internalPtsX;
        set { internalPtsX = value; OnPropertyChanged(); }
    }

    private ObservableCollection<DoubleHandler> internalPtsZ = [new(-500d),new(-200d), new(-100d), new(0d)];

    public ObservableCollection<DoubleHandler> InternalPtsZ
    {
        get => internalPtsZ;
        set { internalPtsZ = value; OnPropertyChanged(); }
    }

    private double current = 100;
    public double Current
    {
        get => current;
        set
        {
            current = value;
            OnPropertyChanged();
        }
    }
    private double recX0 = -1000;
    public double RecX0
    {
        get => recX0;
        set { recX0 = value; OnPropertyChanged(); }
    }
    private double recX1 = 1000;
    public double RecX1
    {
        get { return recX1; }
        set { recX1 = value; OnPropertyChanged(); }
    }
    private int recCount = 100;
    public int RecCount
    {
        get { return recCount; }
        set { recCount = value; OnPropertyChanged(); }
    }
    private double pX = 1;
    public double PX
    {
        get { return pX; }
        set
        {
            pX = value;
            OnPropertyChanged();
        }
    }
    private double pZ = 1;
    public double PZ
    {
        get { return pZ; }
        set
        {
            pZ = value;
            OnPropertyChanged();
        }
    }
    private bool showPx;

    public bool ShowPx
    {
        get => showPx;
        set { showPx = value; OnPropertyChanged(); }
    }

    private double plength => Sqrt(Pow(pX, 2) + Pow(pZ, 2));

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    List<Cell> cells;
    public List<Cell> Cells
    {
      set => cells = value;
        get {
            cells = new List<Cell>();
            var xs = GetAllAxisPoints(XIntervals, InternalPtsX);
            var zs = GetAllAxisPoints(ZIntervals, InternalPtsZ);

            for (int ix = 0; ix < xs.Count - 1; ix++)
            {
                for (int iz = 0; iz < zs.Count - 1; iz++)
                {
                    double x0 = xs[ix], x1 = xs[ix + 1];
                    double z0 = zs[iz], z1 = zs[iz + 1];
                    double cx = (x0 + x1) / 2d, cz = (z0 + z1) / 2d;

                    double length = plength;
                    bool found = false;
                    Area foundArea = null;

                    foreach (var area in Areas)
                    {
                        double xl = internalPtsX[area.IntervalXNum].Value, 
                               xr = internalPtsX[area.IntervalXNum+1].Value, 
                               zl = internalPtsZ[area.IntervalZNum].Value, 
                               zr = internalPtsZ[area.IntervalZNum+1].Value;

                        if (found) break;

                        foundArea = area;
                        found = xl < cx && cx < xr && 
                                zl < cz && cz < zr;
                    }

                    if (found) length = foundArea?.PLength ?? 1d;
                    (double x, double z) p = (length * pX, length * pZ);


                    cells.Add(new Cell(x0, x1, z0, z1, p.x, p.z));
                }   
            }

            return cells;
        }
    }

    List<double> GetAllAxisPoints(ObservableCollection<Interval> intervals, ObservableCollection<DoubleHandler> points)
    {
        List<double> allPoints = [points[0].Value];

        for (int i = 0; i < intervals.Count; i++)
        {
            var intx = intervals[i];

            double xl = points[i].Value;
            double xr = points[i + 1].Value;

            double hx = (xr - xl) ;
            double x0 = xl;
            bool isRegular = Abs(intx.SparseRatio - 1d) < 1e-13;

            double bx = isRegular ?
                hx / intx.IntervalsCount : hx * (1d - intx.SparseRatio) / (1d - Pow(intx.SparseRatio, intx.IntervalsCount));

            for (int ix = 0; ix < intx.IntervalsCount; ix++)
            {
                double scale = isRegular ? 1d : Pow(intx.SparseRatio, ix);
                x0 += bx * scale;
                
                allPoints.Add(x0);
            }
        }

        return allPoints;
    }
}
