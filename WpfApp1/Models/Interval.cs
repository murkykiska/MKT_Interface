using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MKT_Interface.Models;

public class Interval : INotifyPropertyChanged
{
    private int intervalsCount;
    public int IntervalsCount
    {
        get { return intervalsCount; }
        set
        {
            intervalsCount = value;
            OnPropertyChanged();
        }
    }
    private double sparseRatio;
    public double SparseRatio
    {
        get { return sparseRatio; }
        set
        {
            sparseRatio = value; 
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
