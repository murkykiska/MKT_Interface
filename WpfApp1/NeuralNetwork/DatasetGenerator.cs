using System;
using System.Collections.Generic;

namespace MKT_Interface.NeuralNetwork;

public class DatasetGenerator
{
    public DatasetGenerator(int numOfData,
                            int numOfSimiralyGeneratedData,
                            (double X0, double X1, double Y0, double Y1) area,
                            int numOfXCells, int numOfYCells,
                            float dataTrainPercent = 0.8f) 
    {
        this.numOfData = numOfData;
        this.numOfSimiralyGeneratedData = numOfSimiralyGeneratedData;
        this.area = area;
        this.numOfXCells = numOfXCells;
        this.numOfYCells = numOfYCells;
        this.dataTrainPercent = dataTrainPercent;
    }

    Random random = new Random(DateTime.Now.GetHashCode());
    private readonly int numOfData;
    private readonly int numOfSimiralyGeneratedData;
    private readonly (double X0, double X1, double Y0, double Y1) area;
    private readonly int numOfXCells;
    private readonly int numOfYCells;
    private readonly float dataTrainPercent;

    public void GenerateData((double x, double y) minMagnetism, (double x, double y) maxMagnetism, string dataFolder = "./Data", string dataName = "data")
    {
        ArgumentException.ThrowIfNullOrEmpty(dataName, nameof(dataName));
        ArgumentException.ThrowIfNullOrEmpty(dataFolder, nameof(dataFolder));

        for (int i = 0; i < numOfData; i++)
        {
            
        }
    }

    private List<Section> GenerateSegmentation()
    {
        List<Section> sections = new();

        int xleft = 0;
        do
        {
            int xright = random.Next(xleft, numOfXCells + 2);

            double x0 = area.X0 + xleft / (numOfXCells + 1) * area.X1;
            double x1 = area.X0 + xright / (numOfXCells + 1) * area.X1;

            int yleft = 0;
            do
            {
                int yright = random.Next(yleft, numOfYCells + 2);
                double y0 = area.Y0 + yleft / (numOfYCells + 1) * area.Y1;
                double y1 = area.Y0 + yright / (numOfYCells + 1) * area.Y1;

                yleft = yright;
                sections.Add(new Section { x0 = x0, y0 = y0, x1 = x1, y1 = y1 });
            }
            while (yleft <= numOfYCells);

            xleft = xright;
        }
        while (xleft <= numOfXCells);

        return sections;
    }
    private struct Section
    {
        public double x0;
        public double x1;
        public double y0;
        public double y1;
    }
}
