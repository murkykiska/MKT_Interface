using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MKT_Interface.Models
{
    public class Palette : INotifyPropertyChanged
    {
        private double value1;
        public double Value1
        {
            get { return value1; }
            set
            {
                value1 = value;
                OnPropertyChanged();
            }
        }
        private double value2;
        public double Value2
        {
            get { return value2; }
            set
            {
                value2 = value;
                OnPropertyChanged();
            }
        }
        private double value3;
        public double Value3
        {
            get { return value3; }
            set
            {
                value3 = value;
                OnPropertyChanged();
            }
        }
        private double value4;
        public double Value4
        {
            get { return value4; }
            set
            {
                value4 = value;
                OnPropertyChanged();
            }
        }
        private double value5;
        public double Value5
        {
            get { return value5; }
            set
            {
                value5 = value;
                OnPropertyChanged();
            }
        }
        private double value6;
        public double Value6
        {
            get { return value6; }
            set
            {
                value6 = value;
                OnPropertyChanged();
            }
        }
        private double value7;
        public double Value7
        {
            get { return value7; }
            set
            {
                value7 = value;
                OnPropertyChanged();
            }
        }
        private Color color1;
        public Color Color1
        {
            get { return color1; }
            set 
            { 
                color1 = value; 
                OnPropertyChanged(); 
            }
        }
        private Color color2;
        public Color Color2
        {
            get { return color2; }
            set
            {
                color2 = value;
                OnPropertyChanged();
            }
        }
        public Palette(double minValue, double maxValue, Color color1, Color color2)
        {
            double h = (maxValue - minValue) / 6;

            Value1 = Math.Round(minValue, 0);
            Value2 = Math.Round(minValue + h, 0);
            Value3 = Math.Round(minValue + 2 * h, 0);
            Value4 = Math.Round(minValue + 3 * h, 0);
            Value5 = Math.Round(minValue + 4 * h, 0);
            Value6 = Math.Round(minValue + 5 * h, 0);
            Value7 = Math.Round(maxValue, 0);

            Color1 = color1;
            Color2 = color2;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
