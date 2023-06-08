using System;
using Fluent;
using MKT_Interface.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows;
using WpfApp1.EM;

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

         MagnetismManager.makeDirectTask(@"../../../DirectTask.cfg", leftX, rightX, n, "../../../Recs.txt");

      }
      public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
