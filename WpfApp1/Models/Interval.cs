using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MKT_Interface.Models
{
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
