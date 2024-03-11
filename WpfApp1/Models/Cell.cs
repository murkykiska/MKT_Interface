using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MKT_Interface.Models;

public class Cell : INotifyPropertyChanged
{
    private double x0;
    public double X0
    {
        get { return x0; }
        set
        {
            x0 = value;
            OnPropertyChanged();
        }
    }
    private double x1;
    public double X1
    {
        get { return x1; }
        set
        {
            x1 = value;
            OnPropertyChanged();
        }
    }
    private double z0;
    public double Z0
    {
        get { return z0; }
        set
        {
            z0 = value;
            OnPropertyChanged();
        }
    }
    private double z1;
    public double Z1
    {
        get { return z1; }
        set
        {
            z1 = value;
            OnPropertyChanged();
        }
    }
    private double px;
    public double PX
    {
        get { return px; }
        set
        {
            px = value;
            OnPropertyChanged();
        }
    }
    private double pz;
    public double PZ
    {
        get { return pz; }
        set
        {
            pz = value;
            OnPropertyChanged();
        }
    }
   public (double Cx, double Cz) Center => ((x0 + x1) / 2.0, (z0 + z1) / 2.0); 
    public Cell(double x0, double x1, double z0, double z1, double px, double pz)
    {
        X0 = x0;
        X1 = x1;
        Z0 = z0;
        Z1 = z1;            
        PX = px;
        PZ = pz;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
