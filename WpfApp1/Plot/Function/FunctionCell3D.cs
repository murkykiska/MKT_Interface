using MeshVisualizator;
using OpenTK.Mathematics;
using Plot.Shader;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Plot.Function;

public class FunctionCell3D : IFunction
{
   private int _vao = 0, _vbo = 0;
   private int _vaoSide = 0, _vboSide = 0;

   private float[] x_points;
   private float[] y_points;

   private ShaderProgram _shader;
   public IEnumerable<float> XPoints => x_points.OrderBy(p => p).ToArray();
   public IEnumerable<float> YPoints => y_points.OrderBy(p => p).ToArray();


   public Box2 GetDomain()
   {
      if (y_points == null || x_points == null)
         throw new Exception("FunctionCell3D was not defined!");

      var ys = y_points.OrderBy(p => p).ToArray();
      var xs = x_points.OrderBy(p => p).ToArray();
      return new Box2 { Min = (xs[0], ys[0]), Max = (xs[^1], ys[^1]) };
   }

   public void FillPoints(float[] x, float[] y)
   {
      if (x == null || y == null)
         throw new Exception($"Point set {(x == null ? nameof(x) : nameof(y)).ToUpper()} was empty!");
      if (x.Length != y.Length)
         throw new Exception("Length of set X was not equal to length of set Y");

      x_points = x;
      y_points = y;
   }

   public void FillPoints(Vector2[] points)
   {
      
   }

   public void Prepare()
   {
      _shader = new ShaderProgram(new[] { @"Plot/Function/Shaders/func3d.vert", @"Plot/Function/Shaders/func3d.frag" },
         new[] { ShaderType.VertexShader, ShaderType.FragmentShader });
      _shader.LinkShaders();

      float[] pointsFloats = new float[2 * x_points.Length];
      for (int i = 0; i < x_points.Length * 2; i += 2)
      {
         pointsFloats[i] = x_points[i];
         pointsFloats[i + 1] = y_points[i];
      }

      if (_vaoSide == 0)
         _vaoSide = GL.GenVertexArray();
      GL.BindVertexArray(_vao);

      if (_vboSide == 0)
         _vboSide = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vboSide);

      GL.BufferData(BufferTarget.ArrayBuffer, pointsFloats.Length * sizeof(float), pointsFloats, BufferUsageHint.StreamDraw);

      GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);

      GL.BindVertexArray(0);

      GL.GetFloat(GetPName.AliasedLineWidthRange, new float[] { 2, 10 });

   }
   public void Draw(Color4 color, Box2 DrawArea)
   {
      _shader.UseShaders();
      var ortho = Camera2D.Instance.GetOrthoMatrix();
      var model = Matrix4.CreateScale(DrawArea.Size.X, DrawArea.Size.Y, 1) * Matrix4.CreateTranslation(DrawArea.Center.X, DrawArea.Center.Y, 0);
      _shader.SetMatrix4("projection", ref ortho);
      _shader.SetMatrix4("model", ref model);
      _shader.SetVec4("color", ref color);

      GL.LineWidth(8);
      GL.BindVertexArray(_vao);
      GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, 0);


   }
}