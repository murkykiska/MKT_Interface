using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MKT_Interface.Models;
using MKT_Interface.ViewModels;
using OpenTK.Mathematics;
using Plot.Function;

namespace WpfApp1.EM;

public class MagnetismManager
{
    Vector4d[] _data;
    Cell[] Cells;
    public static void MakeDirect(string directtaskCfg, double leftX, double rightX, int i, string recsTxt)
    {
        string direct = "..\\..\\..\\EM\\direct.exe";
        string dinfo = "..\\..\\..\\EM\\direct_info.txt";

        using (var s = new StreamWriter(dinfo))
        {
            s.WriteLine(directtaskCfg);
            s.WriteLine(leftX);
            s.WriteLine(rightX);
            s.WriteLine(i);
            s.WriteLine(recsTxt);
        }

        ProcessStartInfo info = new(direct, "./direct_info.txt");
        info.CreateNoWindow = false;
        info.WorkingDirectory = "./";
        try
        {
            using Process? exeProcess = Process.Start(info);
            exeProcess?.WaitForExit();
        }
        catch
        {
            throw new Exception("Pepega");
        }
    }
    public static void MakeReverse(string directtaskCfg, string recsTxt, string ansTxt, double alpha)
    {
        
    }
    public void GetRecieverData(string recs_data)
    {
        using (BinaryReader r = new(File.Open(recs_data, FileMode.Open)))
        {
            int num = r.ReadInt32();
            _data = new Vector4d[num];
            for (int i = 0; i < num; i++)
            {
                _data[i] = (r.ReadDouble(), r.ReadDouble(), r.ReadDouble(), r.ReadDouble());
            }

            _data = _data.OrderBy(_ => _.X).ToArray();
        }
    }
    public Function2D GetRecieversDataOnPlane(bool isX, float coord)
    {
        var data =
           from d in _data
           where !isX ? d.X == coord : d.Y == coord
           orderby isX ? d.X : d.Y
           select d;

        var f = new Function2D();

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
    public FunctionCell2D GetMagnetismData(bool isX)
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
    public void ReadCells(string path)
    {
        //int count;

        //using (BinaryReader r = new BinaryReader(File.Open(path, FileMode.Open)))
        //{
        //    count = r.ReadInt32();
        //    Cells = new Cell[count];
        //    for (int i = 0; i < count; i++)
        //    {
        //        Cells[i] = new Cell(r.ReadDouble(), r.ReadDouble(), r.ReadDouble(), r.ReadDouble(), r.ReadDouble(), r.ReadDouble());
        //    }
        //}1
    }

}
