using MeshVisualizator;
using OpenTK.Mathematics;
using Plot.Shader;
using System.Linq;
using System;
using System.Windows.Xps;

namespace Plot.Function;

public class FunctionCell3D : IFunction
{
   private int _vao = 0, _vbo = 0;

   private Box2d[] _cells;
   private float[] _values;
   private Matrix4[] _mats;

   public float min => _values.Min();
   public float max => _values.Max();

   private ShaderProgram _shader, _shaderLine;
   private int _textureID;
   private int tex_resolution = 16;
   private int _vboV;
   private int _vboMat;

   public FunctionCell3D()
   {
     
   }
   public Box2 GetDomain()
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

      return new Box2 { Min = ((float)x[0].Min.X, (float)y[0].Min.Y), Max = ((float)x[^1].Max.X, (float)y[^1].Max.Y) };
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

      _shader.UseShaders();
      if (_vao == 0)
         _vao = GL.GenVertexArray();
      GL.BindVertexArray(_vao);

      if (_vbo == 0)
         _vbo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

      {
         GL.BufferData(BufferTarget.ArrayBuffer, 8 * sizeof(float),
            new[]
            {
               //x  y  us      
               -1, -1,
               1, -1,
               -1, 1,
               1, 1,
            },
            BufferUsageHint.StaticDraw);

         GL.EnableVertexAttribArray(0);
         GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); // x y
      }

      float scale = _values.Max() - _values.Min(), min = _values.Min();
      if (scale == 0)
         scale = 1f;
      float[] vals = new float[_values.Length];
      for (int i = 0; i < vals.Length; i++)
         vals[i] = (min + _values[i]) / scale;

      _mats = new Matrix4[_values.Length];
      for (int i = 0; i < vals.Length; i++)
      {
         (double scalex, double scaley) = _cells[i].Size;
         (double tx, double ty) = _cells[i].Center;
         _mats[i] = Matrix4.CreateScale((float)scalex / 2f, (float)scaley / 2f, 1) * Matrix4.CreateTranslation((float)tx, (float)ty, 0);
      }

      _vboV = GL.GenBuffer();
      _vboMat = GL.GenBuffer();
      {
         GL.BindBuffer(BufferTarget.ArrayBuffer, _vboV);
         GL.BufferData(BufferTarget.ArrayBuffer, vals.Length * sizeof(float), vals, BufferUsageHint.StaticDraw);

         GL.BindVertexArray(_vao);
         GL.EnableVertexAttribArray(1);
         GL.VertexAttribDivisor(1, 1);
         GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);


         GL.BindBuffer(BufferTarget.ArrayBuffer, _vboMat);
         GL.BufferData(BufferTarget.ArrayBuffer, _mats.Length * 16 * sizeof(float), _mats,
            BufferUsageHint.StaticDraw);

         int vec4Size = 4 * sizeof(float);

         GL.EnableVertexAttribArray(2);
         GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 4 * vec4Size, 0);
         GL.EnableVertexAttribArray(3);
         GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 4 * vec4Size, (1 * vec4Size));
         GL.EnableVertexAttribArray(4);
         GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 4 * vec4Size, (2 * vec4Size));
         GL.EnableVertexAttribArray(5);
         GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 4 * vec4Size, (3 * vec4Size));

         GL.VertexAttribDivisor(2, 1);
         GL.VertexAttribDivisor(3, 1);
         GL.VertexAttribDivisor(4, 1);
         GL.VertexAttribDivisor(5, 1);

         var _ebo = GL.GenBuffer();
         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
         GL.BufferData(BufferTarget.ElementArrayBuffer, 4 * sizeof(uint), new uint[] { 0, 1, 2, 3 },
            BufferUsageHint.StreamDraw);
      }

      float[] texPixels = new float[3 * tex_resolution];
      for (int i = 0; i < texPixels.Length / 3; i++)
      {
         float w = i / (texPixels.Length / 3f - 1f);

         Vector3 color = Vector3.Lerp(Color0, Color1, w);
         texPixels[3 * i + 0] = color.X;
         texPixels[3 * i + 1] = color.Y;
         texPixels[3 * i + 2] = color.Z;
      }

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
      GL.BindVertexArray(0);
   }

   public Vector3 Color1 { get; set; }
   public Vector3 Color0 { get; set; }

   public void Draw(Color4 color, Box2 DrawArea)
   {
      _shader.UseShaders();
      var ortho = Camera2D.Instance.GetOrthoMatrix();
      var model = Matrix4.CreateTranslation(DrawArea.Center.X, DrawArea.Center.Y, 0);
      _shader.SetMatrix4("projection", ref ortho);
      _shader.SetMatrix4("model", ref model);

      GL.LineWidth(3);
      GL.BindVertexArray(_vao);
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture1D, _textureID);
      GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

      //_shaderLine.UseShaders();
      //_shaderLine.SetMatrix4("projection", ref ortho);
      //_shaderLine.SetMatrix4("model", ref model);
      //_shaderLine.SetVec4("color", ref color);
      //GL.DrawElementsInstanced(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedInt, IntPtr.Zero, _cells.Length);
      GL.BindVertexArray(0);


   }
}