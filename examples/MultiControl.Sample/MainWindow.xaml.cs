using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Windows;

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
			time.Start();
		}

		private Stopwatch time = new Stopwatch();

		private void Draw(float r, float g, float b)
		{
			var angle = (float)time.Elapsed.TotalSeconds * 20f;
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

		private void GlRenderRed(object sender, System.EventArgs e) => Draw(1f, 0f, 0f);

		private void GlRenderGreen(object sender, System.EventArgs e) => Draw(0f, 1f, 0f);
	}
}
