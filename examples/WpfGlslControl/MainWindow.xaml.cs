using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Example
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly DispatcherTimer timerSwitchShader;
		private readonly DispatcherTimer timer;
		private readonly Stopwatch time = new Stopwatch();
		private bool even = false;

		public MainWindow()
		{
			InitializeComponent();
			shaderControl.ShaderSourceCode = "uniform float time; " +
				"in vec2 uv;" +
				" void main() { gl_FragColor = abs(sin(time)) * vec4(uv, 0, 0); }";
			time.Start();

			timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1 / 60.0)
			};
			//shaderControl.SetUniform("iResolution", (float)shaderControl.ActualWidth, (float)shaderControl.ActualHeight);
			timer.Tick += (s, e) => shaderControl.SetUniform("time", (float) time.Elapsed.TotalSeconds);
			timer.Start();

			timerSwitchShader = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			timerSwitchShader.Tick += (s, e) => OnSwitchShader();
			timerSwitchShader.Start();
		}

		private void OnSwitchShader()
		{
			var shaderSource = "void main() { gl_FragColor = ";
			shaderSource += even ? "vec3(0.5); }" : "vec4(0.2, 0.0, 0.5, 1.0); }";
			shaderControl2.ShaderSourceCode = shaderSource;
			even = !even;
		}
	}
}
