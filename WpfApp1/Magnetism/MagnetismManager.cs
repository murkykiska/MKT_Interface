using System.IO;
using System.Linq;
using MKT_Interface.Magnetism;
using MKT_Interface.Models;
using MKT_Interface.NeuralNetwork;
using NeuralNetwork;
using OpenTK.Mathematics;
using Plot.Function;
namespace WpfApp1.EM;

public class MagnetismManager
{
    Vector4d[] _data;
    Cell[] Cells;
    EMForwardCalculator calculator = null!;
    NeuralNetworkHandler nnHandler = null!;
    private readonly EMParameters emParams;
    private readonly NNParameters nnParams;

    public MagnetismManager(EMParameters emParams, NNParameters nnParams, string model_path = "")
    {
        calculator = new(emParams);

        if (string.IsNullOrEmpty(model_path))
            nnHandler = new(nnParams);
        else 
            nnHandler = new(nnParams, new StreamWriter(model_path));
        this.emParams = emParams;
        this.nnParams = nnParams;
    }
    public (float[] x, float[] Bx, float[] Bz) MakeDirect()
    {
        calculator.Calculate();
        var Bxs = calculator.B.Select(x => (float)x.Bx).ToArray();
        var Bzs = calculator.B.Select(x => (float)x.Bz).ToArray();
        var xs = calculator.GetRecievers().Select(x => (float)x).ToArray();
        return (xs, Bxs, Bzs);
    }
    public static void MakeReverse()
    {
        
    }


    public (Box2[] boxCells, float[] values) GetBox2Cells()
    {
        var cells = emParams.Cells;

        Box2[] boxCells = new Box2[cells.Count];
        float[] values = new float[cells.Count];

        for (int i = 0; i < boxCells.Length; i++)
        {
            boxCells[i].Min = ((float)cells[i].X0, (float)cells[i].Z0);
            boxCells[i].Max = ((float)cells[i].X1, (float)cells[i].Z1);

            values[i] = emParams.ShowPx ? (float)cells[i].PX : (float)cells[i].PZ;
        }

        return (boxCells, values);
    }
    public Function2D MagnetismDataAsFunc2D(bool isX, float coord)
    {
        var data =
           from d in _data
           where !isX ? d.X == coord : d.Y == coord
           orderby isX ? d.X : d.Y
           select d;

        var f = new Function2D(lineType: Function2D.LineTypes.Dashes);

        var d_ = data.ToArray();
        Vector2[] func = new Vector2[d_.Length];
        for (int i = 0; i < d_.Length; i++)
        {
            (float x, float y) xy = (isX ? (float)d_[i].X : (float)d_[i].Y, isX ? (float)d_[i].Z : (float)d_[i].W);
            func[i] = xy;
        }

        f.FillPoints(func);
        return f;
    }
    public FunctionCell2D MagnetismDataAsFuncCells2D(bool isX)
    {

        Box2[] area = new Box2[Cells.Length];
        float[] vals = new float[Cells.Length];

        for (int i = 0; i < Cells.Length; i++)
        {
            area[i] = new Box2(((float)Cells[i].X0, (float)Cells[i].Z0), ((float)Cells[i].X1, (float)Cells[i].Z1));
            vals[i] = (float)(isX ? Cells[i].PX : Cells[i].PZ);
        }

        var f = new FunctionCell2D(area, vals);
        return f;
    }

}
