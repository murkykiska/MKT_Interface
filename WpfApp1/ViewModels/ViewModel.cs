using MKT_Interface.Models;
using MKT_Interface.NeuralNetwork;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MKT_Interface.ViewModels;

public class ViewModel : INotifyPropertyChanged
{
    private EMParameters emParams;
    public EMParameters EMParams
    {
        get { return emParams; }
        set
        {
            emParams = value;
            OnPropertyChanged();
        }
    }
    private NNParameters nnParams;
    public NNParameters NNParams
    {
        get { return nnParams; }
        set
        {
            nnParams = value;
            OnPropertyChanged();
        }
    }
    private Palette palette;
    public Palette Palette
    {
        get { return palette; }
        set
        {
            palette = value;
            OnPropertyChanged();
        }
    }
    public void ReDrawPalette(double minValue, double maxValue, Color color1, Color color2)
    {
        Palette = new Palette(minValue, maxValue, color1, color2);
    }
    public ViewModel()
    {
        EMParams = new EMParameters();
        NNParams = new NNParameters();
        Palette = new Palette(-100, 100, Colors.LightBlue, Colors.DarkViolet);
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
