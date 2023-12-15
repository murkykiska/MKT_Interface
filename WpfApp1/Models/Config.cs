using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MKT_Interface.Models;

public class Config : INotifyPropertyChanged
{
    private ObservableCollection<Interval> xIntervals;
    public ObservableCollection<Interval> XIntervals
    {
        get { return xIntervals; }
        set
        {
            xIntervals = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<Interval> zIntervals;
    public ObservableCollection<Interval> ZIntervals
    {
        get { return zIntervals; }
        set
        {
            zIntervals = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<Area> areas;
    public ObservableCollection<Area> Areas
    {
        get { return areas; }
        set
        {
            areas = value;
            OnPropertyChanged();
        }
    }
    private double minX;
    public double MinX
    {
        get { return minX; }
        set 
        {
            minX = value;
            OnPropertyChanged();
        }
    }
    private double maxX;
    public double MaxX
    {
        get { return minZ; }
        set
        {
            minZ = value;
            OnPropertyChanged();
        }
    }
    private double minZ;
    public double MinZ
    {
        get { return minZ; }
        set
        {
            minZ = value;
            OnPropertyChanged();
        }
    }
    private double maxZ;
    public double MaxZ
    {
        get { return maxZ; }
        set
        {
            maxZ = value;
            OnPropertyChanged();
        }
    }
    public void PrintCFG(string filename)
    {
        using StreamWriter writer = new(filename);

        writer.WriteLine("Mesh:");
        writer.WriteLine("{");
        writer.WriteLine("\tBasePoints:");
        writer.WriteLine("\t{");
        writer.WriteLine($"\t\tX = [-100, 100]");
        writer.WriteLine($"\t\tZ = [-200, 100]");
        writer.WriteLine("\t}");
        writer.WriteLine("\tIntervals:");
        writer.WriteLine("\t{");

        writer.WriteLine($"\t\tX = (");
        writer.WriteLine("\t\t\t{");
        writer.WriteLine("\t\t\t\tSplitOptions:");
        writer.WriteLine("\t\t\t\t{");
        writer.WriteLine($"\t\t\t\t\tIntervalsCount = {XIntervals[0].IntervalsCount}");
        writer.WriteLine($"\t\t\t\t\tSparseRatio = {XIntervals[0].SparseRatio}");
        writer.WriteLine("\t\t\t\t}");
        writer.WriteLine("\t\t\t\tPointNum1 = 0");
        writer.WriteLine("\t\t\t\tPointNum2 = 1");
        writer.WriteLine("\t\t\t}");
        writer.WriteLine($"\t\t)");


        writer.WriteLine($"\t\tZ = (");
        writer.WriteLine("\t\t\t{");
        writer.WriteLine("\t\t\t\tSplitOptions:");
        writer.WriteLine("\t\t\t\t{");
        writer.WriteLine($"\t\t\t\t\tIntervalsCount = {ZIntervals[0].IntervalsCount}");
        writer.WriteLine($"\t\t\t\t\tSparseRatio = {ZIntervals[0].SparseRatio}");
        writer.WriteLine("\t\t\t\t}");
        writer.WriteLine("\t\t\t\tPointNum1 = 0");
        writer.WriteLine("\t\t\t\tPointNum2 = 1");
        writer.WriteLine("\t\t\t}");
        writer.WriteLine($"\t\t)");

        writer.WriteLine("\t}");

        writer.WriteLine("\tAreas = (");
        writer.WriteLine("\t\t{");
        writer.WriteLine($"\t\t\tIntervalXNum = {Areas[0].IntervalXNum}");
        writer.WriteLine($"\t\t\tIntervalZNum = {Areas[0].IntervalXNum}");
        writer.WriteLine($"\t\t\tpX = {Areas[0].PX}");
        writer.WriteLine($"\t\t\tpZ = {Areas[0].PZ}");
        writer.WriteLine("\t\t}");
        writer.WriteLine("\t)");
        writer.WriteLine("\tI = 100");
        writer.WriteLine("}");

    }
    public Config()
    {
        XIntervals = new ObservableCollection<Interval>();
        ZIntervals = new ObservableCollection<Interval>();
        Areas = new ObservableCollection<Area>();
        MinX = -100;
        MaxX = -100;
        MinZ = -200;
        MaxZ = -100;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
