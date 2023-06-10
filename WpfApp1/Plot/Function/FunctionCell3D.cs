using MeshVisualizator;
using OpenTK.Mathematics;
using Plot.Shader;
using System.Linq;
using System;
namespace Plot.Function;

public class FunctionCell3D : IFunction
{
   private int _vao = 0, _vbo = 0, _ebo = 0;

   private Box2d[] _cells;
   private float[] _values;
   private Matrix4[] _mats;

   private ShaderProgram _shader, _shaderLine;
   private int _textureID;
   private int tex_resolution;

   public Box2d GetDomain()
   {
      if (_cells == null)
         throw new Exception("FunctionCell3D was not defined!");

      var ys =
         from c in _cells
         orderby c.Min.Y
         select c;
      var xs = 
         from c in _cells
         orderby c.Min.X
         select c;
      var y = ys.ToArray();
      var x = xs.ToArray();

      return new Box2d { Min = (x[0].Min.X, y[0].Min.Y), Max = (x[^1].Max.X, y[^1].Max.Y) };
   }

   public void FillCells(Box2d[] cells, float[] values)
   {
      if (cells == null || values == null)
         throw new Exception($"{(cells == null ? nameof(cells) : nameof(values)).ToUpper()} was empty!");
      if (cells.Length != values.Length)
         throw new Exception("Length cells array was not equal to length of values array");

      _cells = cells;
      _values = values;
   }

   public void Prepare()
   {
      _shader = new ShaderProgram(new[] { @"Plot/Function/Shaders/func3d.vert", @"Plot/Function/Shaders/func3d.frag" },
         new[] { ShaderType.VertexShader, ShaderType.FragmentShader });
      _shader.LinkShaders();

      _shaderLine = new ShaderProgram(new[] { @"Plot/Function/Shaders/border.vert", @"Plot/Function/Shaders/border.frag" },
                                       new[] { ShaderType.VertexShader, ShaderType.FragmentShader });
      _shaderLine.LinkShaders();

      double scale = _values.Max() - _values.Min(), min = _values.Min();

      if (_vao == 0)
         _vao = GL.GenVertexArray();
      GL.BindVertexArray(_vao);

      if (_vbo == 0)
         _vbo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

      for (int i = 0; i < _cells.Length * 2; i += 2)
      {

         if (_vao == 0 && _ebo == 0 && _vbo == 0)
         {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 12 * sizeof(float),
               new[]
               {//x  y  us      
                  _cells[i].Min.X, _cells[i].Min.Y, (min + _values[i]) / scale,
                  _cells[i].Max.X, _cells[i].Min.Y, (min + _values[i]) / scale,
                  _cells[i].Min.X, _cells[i].Max.X, (min + _values[i]) / scale,
                  _cells[i].Max.X, _cells[i].Max.X, (min + _values[i]) / scale, 
               },
               BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // x y
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 3 * sizeof(float), 2 * sizeof(float)); // x y
            GL.EnableVertexAttribArray(1);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 4 * sizeof(uint), new uint[] { 0, 1, 2, 3 },
               BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
         }
      }


      void MakeScaleTexture(ref float[] texPixels)
      {
         for (int i = 0; i < texPixels.Length / 3; i++)
         {
            float w = i / (texPixels.Length / 3f - 1f);

            Vector3 color = Vector3.Lerp((0f, 0f, 0f), (1f, 1f, 1f), w);
            texPixels[3 * i + 0] = color.X;
            texPixels[3 * i + 1] = color.Y;
            texPixels[3 * i + 2] = color.Z;
         }
      }

      float[] texPixels = new float[3 * tex_resolution];
      MakeScaleTexture(ref texPixels);

      _textureID = GL.GenTexture();
      //GL.ActiveTexture(TextureUnit.Texture0); // unnessessary
      GL.BindTexture(TextureTarget.Texture1D, _textureID);
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

      GL.TexImage1D(TextureTarget.Texture1D,
         0,
         PixelInternalFormat.Rgb,
         tex_resolution,
         0,
         PixelFormat.Rgb,
         PixelType.Float,
         texPixels);

      GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
      GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);
      GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.BindTexture(TextureTarget.Texture1D, 0);

      GL.GetFloat(GetPName.AliasedLineWidthRange, new float[] { 2, 10 });

   }
   public void Draw(Color4 color, Box2 DrawArea)
   {
      _shader.UseShaders();
      var ortho = Camera2D.Instance.GetOrthoMatrix();
      var model = Matrix4.CreateScale(DrawArea.Size.X, DrawArea.Size.Y, 1) * Matrix4.CreateTranslation(DrawArea.Center.X, DrawArea.Center.Y, 0);
      _shader.SetMatrix4("projection", ref ortho);
      _shader.SetMatrix4("model", ref model);

      GL.LineWidth(8);
      GL.BindVertexArray(_vao);
      GL.BindTexture(TextureTarget.Texture1D, 0);
      GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, _cells.Length);

      _shaderLine.UseShaders();
      _shaderLine.SetMatrix4("projection", ref ortho);
      _shaderLine.SetMatrix4("model", ref model);
      _shaderLine.SetVec4("color", ref color);
      GL.DrawArraysInstanced(PrimitiveType.LineLoop, 0, 4, _cells.Length);


   }
}