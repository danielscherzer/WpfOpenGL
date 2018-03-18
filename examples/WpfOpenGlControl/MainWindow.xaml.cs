using OpenTK.Graphics.OpenGL;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Example
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			glControl.GlRender += (s, e) => Draw(1f, 0f, 0f);
			glControl1.GlRender += (s, e) => Draw(0f, 1f, 0f);
			glControl2.GlRender += (s, e) => Draw(0f, 0f, 1f);
			glControl3.GlRender += (s, e) => Draw(1f, 0f, 1f);
			timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.0 / 60.0)
			};
			timer.Tick += (s, e) => Redraw();
			timer.Start();
		}

		private float angle = 0f;
		private readonly DispatcherTimer timer;

		private void Draw(float r, float g, float b)
		{
			GL.ClearColor(r, g, b, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Color3(1f, 1f, 1f);
			GL.LoadIdentity();
			GL.Rotate(angle, 0f, 0f, 1f);
			GL.Begin(PrimitiveType.Triangles);
			GL.Vertex2(-.9, -.9);
			GL.Vertex2(.9, -.9);
			GL.Vertex2(.9, .9);
			GL.End();
		}

		private void Redraw()
		{
			angle += (float)timer.Interval.TotalSeconds * 100;
			glControl.Invalidate();
			glControl2.Invalidate();
			glControl3.Invalidate();
		}
	}
}
