using MKT_Interface.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        public ViewModel()
        {
            DirectTask = new Config();
            ReverseTask = new Config();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
