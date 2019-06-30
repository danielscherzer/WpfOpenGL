using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Threading;
using WpfGlslControl;

namespace Example
{
	class ShaderModel
	{
		private WpfShaderControl shaderControl;
		private IDisposable subscription;
		private DispatcherTimer timer = new DispatcherTimer();
		private Stopwatch time = new Stopwatch();

		public ShaderModel(WpfShaderControl shaderControl)
		{
			this.shaderControl = shaderControl;
		}

		private IObservable<string> CreateFileChangeSequence(string fileName)
		{
			var fullPath = Path.GetFullPath(fileName);
			return Observable.Return(fileName).Concat(
				Observable.Using(
				() => new FileSystemWatcher(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath))
				{
					NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.FileName,
					EnableRaisingEvents = true,
				},
				watcher =>
				{
					var fileChanged = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Changed += h, h => watcher.Changed -= h).Select(x => fullPath);
					var fileCreated = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Created += h, h => watcher.Created -= h).Select(x => fullPath);
					var fileRenamed = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => watcher.Renamed += h, h => watcher.Renamed -= h).Select(x => fullPath);
					return fileChanged.Merge(fileCreated).Merge(fileRenamed);
				})
				);
		}

		internal void LoadShaderFile(string fileName)
		{
			subscription?.Dispose();
			var seqFileChanged = CreateFileChangeSequence(fileName);
			subscription = seqFileChanged.Delay(TimeSpan.FromSeconds(0.1f)).Select((newFileName) => File.ReadAllText(newFileName))
				.Retry(3).ObserveOnDispatcher().Subscribe((shaderSource) => LoadShader(shaderSource));
		}

		internal void LoadShader(string shaderSource)
		{
			timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1 / 60.0)
			};
			shaderControl.ShaderSourceCode = shaderSource;

			var uniforms = ShaderParser.ParseUniforms(ShaderParser.RemoveComments(shaderSource));
			var timeUniform = uniforms.Where((uniform) => uniform.name.IndexOf("time", StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();
			if (!string.IsNullOrEmpty(timeUniform.name))
			{
				timer.Tick += (s, e) => shaderControl.SetUniform(timeUniform.name, (float)time.Elapsed.TotalSeconds);
				timer.Start();
				time.Restart();
			}
			var resolutionUniform = uniforms.Where((uniform) => uniform.name.IndexOf("resolution", StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();
			if (!string.IsNullOrEmpty(resolutionUniform.name))
			{
				timer.Tick += (s, e) => shaderControl.SetUniform(resolutionUniform.name, shaderControl.ResolutionX, shaderControl.ResolutionY);
			}
		}
	}
}
