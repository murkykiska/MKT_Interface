using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MKT_Interface.Models
{
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
        private double pX;
        public double PX
        {
            get { return pX; }
            set 
            { 
                pX = value; 
                OnPropertyChanged(); 
            }
        }
        private double pZ;
        public double PZ
        {
            get { return pZ; }
            set
            {
                pZ = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
