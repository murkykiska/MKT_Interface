using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using MeshVisualizator;
using OpenTK.Mathematics;
using PlotTest.Shader;

namespace PlotTest.Viewport;

public class Axis
{
    public static int Margin = 20;
    public static int TickMaxSize = 20;
    public static bool IsInside = false;

    private int _vbo, _vao;
    private ShaderProgram _shader;

    private float[] _ticks;
    private int _position;
    private int _p0, _p1;

    private int _ticksNum;
    private bool _isX;

    //private Matrix4 _transform; 

    public Axis(int point0, int point1, int position, bool isX, int ticksNum = 41)
    {
       _p0 = point0;
       _p1 = point1;
       _position = position;
       _ticksNum = ticksNum;  
       _isX = isX;

       _shader = new ShaderProgram(new[] { @"Plot/Viewport/Shaders/axis.frag", @"Plot/Viewport/Shaders/axis.vert" },
          new[] { ShaderType.FragmentShader, ShaderType.VertexShader });
       _shader.LinkShaders();

       CalcTicks();
       Prepare();
    }

    private void Prepare()
    {
       _shader.UseShaders();
       
        if (_vao == 0) 
           _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        if (_vbo == 0) 
           _vbo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _ticks.Length * sizeof(float), _ticks, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);
        
    }
    public void Draw()
    {
        _shader.UseShaders();
        var ortho = Camera2D.Instance.GetOrthoMatrix();
        _shader.SetMatrix4("projection", ref ortho);
        //_shader.SetMatrix4("transform", ref _transform);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Lines, 0, 2 * _ticksNum);
        GL.BindVertexArray(0);
    }

    public void OnViewChange(int point0, int point1, int position)
    {
        _p0 = point0;
        _p1 = point1;
        _position = position;
        CalcTicks();
        Prepare();
    }
    private void CalcTicks()
    {
      //_transform = _isX ? Matrix4.CreateTranslation(0, _position, 0) 
      //                  : Matrix4.CreateTranslation(_position, 0, 0);

       float v = (_p1 - _p0) / ((float)_ticksNum - 1);
       _ticks = new float[_ticksNum * 4];
       for (int i = 0; i < _ticksNum * 4; i += 4)
       {
          int k = i / 4;
          int mul = IsInside ? -1 : 1;

          float u = mul * TickMaxSize;
          if (k % 5 != 0)
             u /= 1.5f;
          if (k % 10 != 0)
             u /= 1.5f;
          _ticks[i] = _isX ? _p0 + k * v : _position;     // x // point on axis
          _ticks[i + 1] = _isX ? _position : _p0 + k * v; // y // point on axis
          _ticks[i + 2] = _isX ? _p0 + k * v : _position - u; // x
          _ticks[i + 3] = _isX ? _position - u : _p0 + k * v; // y
       }
    }
}