﻿using MeshVisualizator;
using OpenTK.Mathematics;
using Plot.Shader;
using System;

namespace Plot.Viewport;

public class PlotView
{
    private (int x0, int x1, int y0, int y1) _margin = (10, 10, 10, 10);

    public Vector2i Size => new Vector2i(x1, y1);
    private int x0, x1, y0, y1;

    private int _vao, _vbo;
    private ShaderProgram _shader;

    private Axis _yAxis, _xAxis;
    public (int x0, int x1, int y0, int y1) Margin
    {
        set => _margin = value;
        get => _margin;
    }
    public PlotView()
    {
        _shader = new ShaderProgram(new[] { @"Plot/Viewport/Shaders/plotbox.vert", @"Plot/Viewport/Shaders/plotbox.frag" },
           new[] { ShaderType.VertexShader, ShaderType.FragmentShader });
        _shader.LinkShaders();

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);


        GL.BufferData(BufferTarget.ArrayBuffer, 8 * sizeof(float), new[]
        {
         -1f, -1f,
         1f, -1f,
         1f, 1f,
         -1f, 1f
      }, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        GL.LineWidth(1);
    }
    public void DrawPlotView(Vector2i ViewportSize, Action drawFunc)
    {
        x0 = _margin.x0 + Axis.Margin + Axis.TickMaxSize;
        x1 = _margin.x1 - Axis.Margin - Axis.TickMaxSize;
        y0 = _margin.y0 + Axis.Margin + Axis.TickMaxSize;
        y1 = ViewportSize.Y - (_margin.y1 * 2 + Axis.Margin) - Axis.TickMaxSize;

        _shader.UseShaders();
        if (x0 >= x1 || y0 >= y1)
            return;    
        GL.Viewport(x0, y0, x1, y1);
        GL.Scissor(x0, y0, x1, y1);
        GL.Enable(EnableCap.ScissorTest);

        GL.LineWidth(2);
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);

        Camera2D.Instance.Size = (x1, y1);

        drawFunc();

        // Stop clipping
        GL.Viewport(0, 0, ViewportSize.X, ViewportSize.Y);
        GL.Disable(EnableCap.ScissorTest);

        Camera2D.Instance.Size = ViewportSize;
        _yAxis?.Draw();
        _xAxis?.Draw();
    }

    public void SetAxes(Vector2i ViewportSize)
    {
        int x0 = _margin.x0 + Axis.Margin + Axis.TickMaxSize - ViewportSize.X / 2,
            x1 = x0 + _margin.x1 - Axis.Margin - Axis.TickMaxSize,

            y0 = _margin.y0 + Axis.Margin + Axis.TickMaxSize - ViewportSize.Y / 2,
            y1 = y0 - (_margin.y1 * 2 + Axis.Margin) - Axis.TickMaxSize + ViewportSize.Y;

        if (_xAxis is null)
            _xAxis = new Axis(x0, x1, y0, true);
        else
            _xAxis.OnViewChange(x0, x1, y0);

        if (_yAxis is null)
            _yAxis = new Axis(y0, y1, x0, false);
        else
            _yAxis.OnViewChange(y0, y1, x0);


    }
}