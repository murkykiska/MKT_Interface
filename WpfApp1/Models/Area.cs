using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MKT_Interface.Models;

public class Area : INotifyPropertyChanged
{
    private int intervalXNum;
    public int IntervalXNum
    {
        get { return intervalXNum; }
        set
        {
            intervalXNum = value;
            OnPropertyChanged();
        }
    }
    private int intervalZNum;
    public int IntervalZNum
    {
        get { return intervalZNum; }
        set
        {
            intervalZNum = value;
            OnPropertyChanged();
        }
    }
    private double pLength;
    public double PLength
    {
        get => pLength;
        set { pLength = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
