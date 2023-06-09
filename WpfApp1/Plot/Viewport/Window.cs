using System;
using System.Drawing;
using System.Text;
using MeshVisualizator;
using OpenTK.Mathematics;
using OpenTK.Text;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PlotTest.Function;
using PlotTest.Shader;
using PlotTest.Viewport;

namespace Plot;

public partial class Window : GameWindow
{
   private StringBuilder _sb = new();
   private TextRenderer[] _textRenderer;
   private Camera2D camera = Camera2D.Instance;
   private int Width, Height;
   private ShaderProgram _mouseProgram;


   public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
   {
      (Width, Height) = nativeWindowSettings.Size;

   }

   protected override void OnLoad()
   {
      GL.ClearColor(0.7f, 0.7f, 0.7f, 1f);
      camera.Size = Size;
      _textRenderer = new[]
      {
         new TextRenderer()
            .SetTextAndParam("fps",
               new TextParams
               {
                  Alignment = TextAlignment.Right,
                  Color = Color4.Gray,
                  TextFontFamily = FontFamily.GenericSerif,
                  TextFontStyle = FontStyle.Bold,
                  FontSize = 18,
                  //CharXSpacing = 16
               })
            .SetCoordinates((-Size.X/2, Size.Y/2 - 28)),
         new TextRenderer()
            .SetTextAndParam("abc",
               new TextParams
               {
                  Alignment = TextAlignment.Right,
                  Color = Color4.Cyan,
                  TextFontFamily = FontFamily.GenericSansSerif,
                  TextFontStyle = FontStyle.Underline,
                  FontSize = 24,
                  //CharXSpacing = 16
               })
            .SetCoordinates((0, 0)),
         new TextRenderer()
            .SetTextAndParam("cba",
               new TextParams
               {
                  Alignment = TextAlignment.Right,
                  Color = Color4.Blue,
                  TextFontFamily = FontFamily.GenericSansSerif,
                  TextFontStyle = FontStyle.Regular,
                  FontSize = 16,
                  //CharXSpacing = 16
               })
            .SetCoordinates((80, 0)),
      };

      var func = new Function2D();
      float[] xs = new float[1000];
      float[] ys = new float[1000];

      for (int i = 0; i < 1000; i++)
      {
         xs[i] = (float)i - 200;
         ys[i] = MathF.Sin(xs[i] / 20) * 50;
      }
      func.FillPoints(xs, ys);
      func.Prepare();
      FunctionManager.Instance.AddNewFunction(func);

      func = new Function2D();
      xs = new float[1000];
      ys = new float[1000];

      xs[0] = -200;
      ys[0] = MathF.Atan(-10) * 50;
      for (int i = 1; i < 1000; i++)
      {
         xs[i] = xs[i-1] + 5;
         ys[i] = MathF.Atan(xs[i] / 20) * 50;
      }
      func.FillPoints(xs, ys);
      func.Prepare();
      FunctionManager.Instance.AddNewFunction(func);

      GL.Enable(EnableCap.Blend);
      GL.Enable(EnableCap.LineSmooth);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      _mouseProgram = new ShaderProgram(new[] { @"Viewport/Shaders/mouse.frag", @"Viewport/Shaders/mouse.vert" },
         new[] { ShaderType.FragmentShader, ShaderType.VertexShader });
      _mouseProgram.LinkShaders();
      base.OnLoad();
   }

   protected override void OnMouseMove(MouseMoveEventArgs e)
   {
      base.OnMouseMove(e);
      Title = (e.X - (Axis.Margin + Axis.TickMaxSize + PlotView.Instance.Margin.x0) - PlotView.Instance.Size.X / 2,
         Size.Y - e.Y - (Axis.Margin + Axis.TickMaxSize + PlotView.Instance.Margin.y0) - PlotView.Instance.Size.Y / 2)
         .ToString();
   }
   
   protected override void OnResize(ResizeEventArgs e)
   {
      (Width, Height) = e.Size;
      //GL.Enable(EnableCap.ScissorTest);
      GL.Viewport(0, 0, e.Width, e.Height);
      camera.Size = e.Size;
      PlotView.Instance.SetAxes(e.Size);
      base.OnResize(e);
   }

   private float ticks = 0;
   private float angle = 0;
   protected override void OnRenderFrame(FrameEventArgs args)
   {
      ticks += (float)args.Time;

      if (ticks > 1)
         ticks = 0;

      // Render plotview
      {
         PlotView.Instance
            .DrawPlotView(Size, () =>
            {
               FunctionManager.Instance.DrawFunctions();
               DrawMouse();
            });
      }

      //foreach (var textRenderer in _textRenderer)
      //   textRenderer.DrawText();

      SwapBuffers();
      GL.ClearColor(0.7f, 0.7f, 0.7f, 1f);
      GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

      GL.Finish();
      base.OnRenderFrame(args);
   }

   private void DrawMouse()
   {
      var vao = GL.GenVertexArray();
      GL.BindVertexArray(vao);

      var vbo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
      GL.BufferData(BufferTarget.ArrayBuffer, 8 * sizeof(float), new[]
      {
         -10000,
         Size.Y-MousePosition.Y - PlotView.Instance.Size.Y / 2 - PlotView.Instance.Margin.y0 - Axis.Margin - Axis.TickMaxSize,
         10000,
         Size.Y-MousePosition.Y - PlotView.Instance.Size.Y / 2 - PlotView.Instance.Margin.y0 - Axis.Margin - Axis.TickMaxSize,
         MousePosition.X - PlotView.Instance.Size.X / 2 - PlotView.Instance.Margin.x0 - Axis.Margin - Axis.TickMaxSize,
         -10000,
         MousePosition.X - PlotView.Instance.Size.X / 2 - PlotView.Instance.Margin.x0 - Axis.Margin - Axis.TickMaxSize,
         10000,
      }, BufferUsageHint.DynamicDraw);
      GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      _mouseProgram.UseShaders();

      var ortho = Camera2D.Instance.GetOrthoMatrix();
      _mouseProgram.SetMatrix4("projection", ref ortho);
      GL.DrawArrays(PrimitiveType.Lines, 0, 4);

      GL.DeleteBuffer(vbo);
      GL.DeleteVertexArray(vao);
   }

   protected override void OnUnload()
   {
      foreach (var textRenderer in _textRenderer)
      {
         textRenderer.Dispose();
      }
      base.OnUnload();
   }
   
   protected override void OnKeyDown(KeyboardKeyEventArgs e)
   {
      if (e.Key == Keys.Space)
      {
         Axis.IsInside = !Axis.IsInside;
         PlotView.Instance.SetAxes(Size);
      }
      base.OnKeyDown(e);
   }

   protected override void OnMouseEnter()
   {
      Cursor = MouseCursor.Crosshair;
      MakeCurrent();
      base.OnMouseEnter();
   }

   protected override void OnMouseLeave()
   {
      Cursor = MouseCursor.Default;
      base.OnMouseLeave();
   }
   
}