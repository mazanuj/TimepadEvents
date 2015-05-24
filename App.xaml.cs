using System.Windows;
using Awesomium.Core;

namespace TimepadEvents
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnExit(ExitEventArgs e)
		{
			// Make sure we shutdown the core last.
			if (WebCore.IsInitialized)
				WebCore.Shutdown();

			base.OnExit(e);
		}
	}
}