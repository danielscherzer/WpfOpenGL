using System.Linq;
using System.Windows;

namespace Example
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ShaderModel shaderModel;

		public MainWindow()
		{
			InitializeComponent();
			shaderModel = new ShaderModel(shaderControl);
			//shaderModel.LoadShader("uniform float time; " + "in vec2 uv;" + " void main() { gl_FragColor = abs(sin(time)) * vec4(uv, 0, 0); }");
			shaderModel.LoadShaderFile(@"D:\Daten\git\SHADER\distanceFields2D\vector field.glsl");
		}

		private void ShaderControl_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}

		private void ShaderControl_Drop(object sender, DragEventArgs e)
		{
			var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
			shaderModel.LoadShaderFile(fileNames.FirstOrDefault());
		}
	}
}
