using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace WpfGlslControl
{
	/// <summary>
	/// Interaction logic for WpfShaderControl.xaml
	/// </summary>
	public partial class WpfShaderControl : UserControl
	{
		/// <summary>
		/// Gets the shader log string.
		/// </summary>
		/// <value>
		/// Contains the shader compile/link log if an error has occurred.
		/// </value>
		public string ShaderLog
		{
			get { return (string)GetValue(ShaderLogProperty); }
			set { SetValue(ShaderLogProperty, value); }
		}

		/// <summary>
		/// Gets or sets the shader source code string.
		/// Setting it will trigger a redraw and a shader compile, link
		/// </summary>
		/// <value>
		/// The shader source code string.
		/// </value>
		public string ShaderSourceCode
		{
			get { return (string)GetValue(ShaderSourceCodeProperty); }
			set { SetValue(ShaderSourceCodeProperty, value); }
		}

		/// <summary>
		/// The shader log dependency property
		/// </summary>
		public static readonly DependencyProperty ShaderLogProperty =
			DependencyProperty.Register(nameof(ShaderLog), typeof(string), typeof(WpfShaderControl)
			, new PropertyMetadata(string.Empty));

		/// <summary>
		/// The shader source code dependency property
		/// </summary>
		public static readonly DependencyProperty ShaderSourceCodeProperty =
			DependencyProperty.Register(nameof(ShaderSourceCode), typeof(string), typeof(WpfShaderControl)
			, new PropertyMetadata(string.Empty, OnShaderSourceCodePropertyChanged));

		/// <summary>
		/// Initializes a new instance of the <see cref="WpfShaderControl"/> class.
		/// </summary>
		public WpfShaderControl()
		{
			InitializeComponent();
			contentManager = ContentManagerGL.Create(Assembly.GetExecutingAssembly(), false);
		}

		/// <summary>
		/// Sets a shader uniform.
		/// </summary>
		/// <param name="name">The name of the uniform.</param>
		/// <param name="x">The value of the uniform</param>
		/// <returns><code>true</code> if uniform name exists in the current shader</returns>
		public bool SetUniform(string name, float x)
		{
			return SetUniform(name, (location) => GL.Uniform1(location, x));
		}

		/// <summary>
		/// Sets a shader uniform.
		/// </summary>
		/// <param name="name">The name of the uniform.</param>
		/// <param name="i">The value of the uniform</param>
		/// <returns><code>true</code> if uniform name exists in the current shader</returns>
		public bool SetUniform(string name, int i)
		{
			return SetUniform(name, (location) => GL.Uniform1(location, i));
		}
		
		/// <summary>
		/// Sets a shader uniform.
		/// </summary>
		/// <param name="name">The name of the uniform.</param>
		/// <param name="x">The x-value of the uniform</param>
		/// <param name="y">The y-value of the uniform</param>
		/// <returns><code>true</code> if uniform name exists in the current shader</returns>
		public bool SetUniform(string name, float x, float y)
		{
			return SetUniform(name, (location) => GL.Uniform2(location, x, y));
		}

		/// <summary>
		/// Sets a shader uniform.
		/// </summary>
		/// <param name="name">The name of the uniform.</param>
		/// <param name="x">The x-value of the uniform</param>
		/// <param name="y">The y-value of the uniform</param>
		/// <param name="z">The z-value of the uniform</param>
		/// <returns><code>true</code> if uniform name exists in the current shader</returns>
		public bool SetUniform(string name, float x, float y, float z)
		{
			return SetUniform(name, (location) => GL.Uniform3(location, x, y, z));
		}

		private bool SetUniform(string name, Action<int> action)
		{
			//TODO: could be a problem if not called from a dispatch timer -> on source code change parse shader uniforms and get locations
			var location = shader.GetResourceLocation(ShaderResourceType.Uniform, name);
			uniformSetters.Add(() => action(location));
			glcontrol.Invalidate();
			return -1 != location;
		}

		private readonly FileContentManager contentManager;
		private static IShaderProgram defaultShader;
		private IShaderProgram shader;
		private bool needShaderReload;
		private List<Action> uniformSetters = new List<Action>();

		private void WpfOpenGLControl_GlRender(object sender, EventArgs e)
		{
			if (!glcontrol.ValidOpenGLContext) return;
			CheckShaderReload();
			shader.Activate();
			//set all changed uniforms
			foreach (var setter in uniformSetters)
			{
				setter.Invoke();
			}
			uniformSetters.Clear();
			//draw quad
			GL.DrawArrays(PrimitiveType.Quads, 0, 4);
			shader.Deactivate();
		}

		private void CheckShaderReload()
		{
			if (!needShaderReload) return;
			needShaderReload = false;
			try
			{
				if (!ReferenceEquals(defaultShader, shader)) shader.Dispose();
				var sVertexShader = contentManager.Load<string>("ScreenQuad.vert");
				shader = ShaderLoader.CreateFromStrings(sVertexShader, ShaderSourceCode);
				ShaderLog = string.Empty;
			}
			catch (ShaderException e)
			{
				ShaderLog = e.Message;
				shader = defaultShader;
			}
			uniformSetters.Clear(); //uniform locations may have changed, so old setters are invalid
		}

		private static void OnShaderSourceCodePropertyChanged(DependencyObject source,
				DependencyPropertyChangedEventArgs e)
		{
			var control = source as WpfShaderControl;
			if (control is null) return;
			control.needShaderReload = true;
			control.glcontrol.Invalidate();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (!glcontrol.ValidOpenGLContext) return;
			if (defaultShader is null)
			{
				var sVertexShader = contentManager.Load<string>("ScreenQuad.vert");
				var sFragmentShader = contentManager.Load<string>("checker");
				defaultShader = ShaderLoader.CreateFromStrings(sVertexShader, sFragmentShader);
			}
			if (shader is null)
			{
				shader = defaultShader;
			}
		}
	}
}
