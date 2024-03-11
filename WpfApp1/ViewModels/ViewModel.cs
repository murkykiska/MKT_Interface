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
   private Palette truePalette;
   public Palette TruePalette
   {
      get { return truePalette; }
      set
      {
         truePalette = value;
         OnPropertyChanged();
      }
   }
   private Palette calcPalette;
   public Palette CalcPalette
   {
      get { return calcPalette; }
      set
      {
         calcPalette = value;
         OnPropertyChanged();
      }
   }
   public void ReDrawTruePalette(double minValue, double maxValue, Color color1, Color color2)
   {
      TruePalette = new Palette(minValue, maxValue, color1, color2);
   }
   public ViewModel()
   {
      EMParams = new EMParameters();
      NNParams = new NNParameters();
      TruePalette = new Palette(-100, 100, Colors.LightBlue, Colors.DarkViolet);
      CalcPalette = new Palette(-100, 100, Colors.Yellow, Colors.DarkOrange);
   }
   public event PropertyChangedEventHandler? PropertyChanged;
   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   internal void ReDrawCalcPalette(float minValue, float maxValue, Color color1, Color color2)
   {
      CalcPalette = new Palette(minValue, maxValue, color1, color2);
   }
}

