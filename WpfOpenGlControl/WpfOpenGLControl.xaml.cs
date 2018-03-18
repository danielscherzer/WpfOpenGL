using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace WpfOpenGlControl
{
	/// <summary>
	/// A WPF gl control. The render context is shared between the instances. It uses a OpenTk.GLControl internally
	/// </summary>
	public partial class WpfOpenGLControl : UserControl
	{
		/// <summary>
		/// Event is called each time the control has to be redrawn.
		/// </summary>
		public event EventHandler GlRender;

		/// <summary>
		/// Gets a value indicating whether the OpenGL context is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if the OpenGL context is valid; otherwise, <c>false</c>.
		/// </value>
		public bool ValidOpenGLContext => !(s_context is null);

		/// <summary>
		/// Creates the internal glControl and attaches it to a WindowsFormsHost
		/// </summary>
		public WpfOpenGLControl()
		{
			InitializeComponent();
			// create and wrap WINFORM control
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				glControl = new GLControl();
				glControl.HandleCreated += GlControl_HandleCreated;
				windowsFormsHost1.Child = glControl;
				WindowsFormsHost.EnableWindowsFormsInterop();
				glControl.Paint += GlControl_Paint;
			}
		}

		/// <summary>
		/// Forces a redraw of this instance.
		/// </summary>
		public void Invalidate()
		{
			glControl.Invalidate();
		}

		private readonly GLControl glControl;
		private static IGraphicsContext s_context;

		private void GlControl_HandleCreated(object sender, EventArgs e)
		{
			if (glControl is null) return;
			if (s_context is null) //first GLCONTROL sets rendering context
			{
				s_context = glControl.Context;
			}
		}

		private void GlControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (glControl is null) return;
			glControl.Invalidate();
		}

		private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (glControl is null) return;
			s_context.MakeCurrent(glControl.WindowInfo);
			GL.Viewport(0, 0, glControl.Width, glControl.Height);
			GlRender?.Invoke(this, e);
			glControl.SwapBuffers();
		}
	}
}
