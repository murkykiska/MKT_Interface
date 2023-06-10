using MKT_Interface.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MKT_Interface.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Config directTask;
        public Config DirectTask
        {
            get { return directTask; }
            set
            {
                directTask = value;
                OnPropertyChanged();
            }
        }
        private Config reverseTask;
        public Config ReverseTask
        {
            get { return reverseTask; }
            set
            {
                reverseTask = value;
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
        private void ReDrawPalette(double minValue, double maxValue, Color color1, Color color2)
        {
            Palette = new Palette(minValue, maxValue, color1, color2);
        }
        public ViewModel()
        {
            DirectTask = new Config();
            ReverseTask = new Config();
            Palette = new Palette(-100, 100, Colors.LightBlue, Colors.DarkViolet);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
