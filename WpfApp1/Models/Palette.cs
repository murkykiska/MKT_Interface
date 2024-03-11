﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MKT_Interface.Models;

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

        Value1 = minValue;
        Value2 = minValue + h;
        Value3 = minValue + 2 * h;
        Value4 = minValue + 3 * h;
        Value5 = minValue + 4 * h;
        Value6 = minValue + 5 * h;
        Value7 = maxValue;
        Color1 = color1;
        Color2 = color2;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
