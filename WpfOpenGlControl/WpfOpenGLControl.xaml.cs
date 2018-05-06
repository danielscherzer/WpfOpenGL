namespace WpfOpenGlControl
{
	using OpenTK;
	using OpenTK.Graphics;
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Forms.Integration;
	using System.Windows.Media;

	/// <summary>
	/// A WPF GL control. The render context is by default shared between the instances. It uses a OpenTk.GLControl internally
	/// </summary>
	public partial class WpfOpenGLControl : UserControl
	{
		/// <summary>
		/// Creates the internal glControl and attaches it to a WindowsFormsHost
		/// </summary>
		public WpfOpenGLControl()
		{
			InitializeComponent();
			// create and wrap WINFORM control
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				//TODO: performance with an INTEL integrated 620 on 4k is not good (see example on full screen)
				glControl = new GLControl();
				glControl.HandleCreated += GlControl_HandleCreated;
				host.Child = glControl;
				WindowsFormsHost.EnableWindowsFormsInterop();
				glControl.Paint += GlControl_Paint;
				glControl.Resize += GlControl_Invalidate;
				glControl.CreateControl(); //force creation of control, so dependent constructors have valid OpenGL
				glControl.VSync = true;
			}
		}

		/// <summary>
		/// Event is called each time the control has to be redrawn.
		/// </summary>
		public event EventHandler GlRender;

		/// <summary>
		/// Gets or sets a value indicating whether the render loop is activated.
		/// </summary>
		/// <value>
		///   <c>true</c> if the render loop is activated; otherwise, <c>false</c>.
		/// </value>
		public bool IsRenderLoopActivated
		{
			get { return (bool)GetValue(IsRenderLoopActivatedProperty); }
			set { SetValue(IsRenderLoopActivatedProperty, value); }
		}

		/// <summary>
		/// The render loop activated property
		/// </summary>
		public static readonly DependencyProperty IsRenderLoopActivatedProperty =
			DependencyProperty.Register(nameof(IsRenderLoopActivated), typeof(bool), typeof(WpfOpenGLControl)
			, new PropertyMetadata(false, OnRenderLoopActivatedPropertyChanged));

		/// <summary>
		/// Gets or sets a value indicating whether this instance has a shared rendering context.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance has shared context; otherwise, <c>false</c>.
		/// </value>
		public bool HasSharedContext
		{
			get { return (bool)GetValue(HasSharedContextProperty); }
			set { SetValue(HasSharedContextProperty, value); }
		}

		/// <summary>
		/// The has a shared context property
		/// </summary>
		public static readonly DependencyProperty HasSharedContextProperty =
			DependencyProperty.Register(nameof(HasSharedContext), typeof(bool), typeof(WpfOpenGLControl), new PropertyMetadata(false));

		/// <summary>
		/// Gets a value indicating whether the OpenGL context is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if the OpenGL context is valid; otherwise, <c>false</c>.
		/// </value>
		public bool ValidOpenGLContext => !(Context is null);

		/// <summary>
		/// Forces a redraw of this instance.
		/// </summary>
		public void Invalidate()
		{
			glControl.Invalidate();
		}

		private readonly GLControl glControl;
		private static IGraphicsContext singleContext;

		private IGraphicsContext Context
		{
			get
			{
				return HasSharedContext ? singleContext : glControl?.Context;
			}
		}

		private void GlControl_HandleCreated(object sender, EventArgs e)
		{
			if (glControl is null) return;
			if (singleContext is null) //first GLCONTROL sets single rendering context
			{
				singleContext = glControl.Context;
			}
		}

		private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (glControl is null) return;
			Context.MakeCurrent(glControl.WindowInfo);
			GL.Viewport(0, 0, glControl.Width, glControl.Height);
			GlRender?.Invoke(this, e);
			glControl.SwapBuffers();
			if(IsRenderLoopActivated) glControl.Invalidate(); //force redraw
		}

		private void GlControl_Invalidate(object sender, EventArgs e)
		{
			glControl.Invalidate();
		}

		private static void OnRenderLoopActivatedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			var control = source as WpfOpenGLControl;
			if (control is null) return;
			if (!control.ValidOpenGLContext) return;
			if ((bool)e.NewValue)
			{
				control.Invalidate();
				//CompositionTarget.Rendering += control.GlControl_Invalidate; // render every frame, but often creates jerky movement
			}
			else
			{
				//CompositionTarget.Rendering -= control.GlControl_Invalidate;
			}
		}
	}
}
