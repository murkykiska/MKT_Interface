using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MKT_Interface.NeuralNetwork;

public class NNParameters : INotifyPropertyChanged
{
    private double learningRate;
    public double LearningRate
    {
        get => learningRate;
        set
        {
            learningRate = value;
            OnPropertyChanged();
        }
    }

    private int batchSize;
    public int BatchSize
    {
        get => batchSize;
        set
        {
            batchSize = value;
            OnPropertyChanged();
        }
    }
    private int displaySteps;

    public int DisplaySteps
    {
        get { return displaySteps; }
        set { displaySteps = value; OnPropertyChanged(); }
    }

    private int steps;

    public int Steps
    {
        get { return steps; }
        set { steps = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
