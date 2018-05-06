using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace Example
{
	using Line = Tuple<Vector2, Vector2>;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			time.Start();
			GL.ClearColor(1f, 1f, 1f, 1f);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.LineSmooth);
			GL.LineWidth(5.0f);
		}

		private Stopwatch time = new Stopwatch();
		private const float size = 0.7f;
		private Line stick = new Line(new Vector2(-size, -size), new Vector2(size, size));
		private float lastRenderTime = 0f;

		private void GlRender(object sender, EventArgs e)
		{
			var currentTime = (float)time.Elapsed.TotalSeconds;
			var deltaTime = currentTime - lastRenderTime;
			lastRenderTime = currentTime;
			float angle = -deltaTime * 0.6f;

			stick = RotateLine(stick, angle);
			var minX = Math.Min(stick.Item1.X, stick.Item2.X);
			var maxX = Math.Max(stick.Item1.X, stick.Item2.X);
			var minY = Math.Min(stick.Item1.Y, stick.Item2.Y);
			var maxY = Math.Max(stick.Item1.Y, stick.Item2.Y);

			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.Color3(Color.CornflowerBlue);
			DrawLine(stick);

			GL.Color3(Color.YellowGreen);
			DrawBox(minX, maxX, minY, maxY);
		}

		private static void DrawBox(float minX, float maxX, float minY, float maxY)
		{
			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex2(minX, minY);
			GL.Vertex2(maxX, minY);
			GL.Vertex2(maxX, maxY);
			GL.Vertex2(minX, maxY);
			GL.End();
		}

		private static Line RotateLine(Line stick, float rotationAngle)
		{
			var mtxRotation = Matrix2.CreateRotation(rotationAngle);
			
			Vector2 a;
			a.X = Vector2.Dot(mtxRotation.Column0, stick.Item1);
			a.Y = Vector2.Dot(mtxRotation.Column1, stick.Item1);
			Vector2 b;
			b.X = Vector2.Dot(mtxRotation.Column0, stick.Item2);
			b.Y = Vector2.Dot(mtxRotation.Column1, stick.Item2);
			return new Line(a, b);
		}

		private static void DrawLine(Line stick)
		{
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(stick.Item1);
			GL.Vertex2(stick.Item2);
			GL.End();
		}
	}
}
