using Fluent;
using MKT_Interface.Models;
using MKT_Interface.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
    {
        private ViewModel viewModel;
        public ViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new ViewModel();
            directxIntervals.ItemsSource = ViewModel.DirectTask.XIntervals;
            directzIntervals.ItemsSource = ViewModel.DirectTask.ZIntervals;
            directAreas.ItemsSource = ViewModel.DirectTask.Areas;

            reversexIntervals.ItemsSource = ViewModel.ReverseTask.XIntervals;
            reversezIntervals.ItemsSource = ViewModel.ReverseTask.ZIntervals;
            reverseAreas.ItemsSource = ViewModel.ReverseTask.Areas;
        }

        private void createDirectCFG_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DirectTask.PrintCFG("../../../DirectTask.cfg");
        }

        private void createReverseCFG_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReverseTask.PrintCFG("../../../ReverseTask.cfg");
        }
        private void EnterRecievers_Click(object sender, RoutedEventArgs e)
        {
            double leftX = double.Parse(receiverBegX.Text);
            double rightX = double.Parse(receiverEndX.Text);
            int n = int.Parse(receiverCount.Text);

            //...
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
