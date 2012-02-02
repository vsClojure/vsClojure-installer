using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace vsClojure.VisualStudio_Clojure_Installer
{
	[Guid("40953a10-3425-499c-8162-a90059792d13")]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	public sealed class VisualStudio_Clojure_InstallerPackage : Package
	{
		protected override void Initialize()
		{
			base.Initialize();
			var dte = (DTE2) GetService(typeof (DTE));
			dte.Events.DTEEvents.OnStartupComplete += UninstallPrompt;
		}

		private static void UninstallPrompt()
		{
			var prompt = new InstallationPrompt();
			prompt.RemindMeLaterButton.Click += (sender, args) => prompt.Close();
			prompt.Remove.Click += (sender, args) => prompt.Close();
			prompt.Remove.Click += (sender, args) => UninstallOldVersion();
			prompt.ShowDialog();
		}

		private static void UninstallOldVersion()
		{
			try
			{
				var extensionManager = GetGlobalService(typeof(SVsExtensionManager)) as IVsExtensionManager;
				var installerExtension = extensionManager.GetInstalledExtension("40953a10-3425-499c-8162-a90059792d13");
				extensionManager.Disable(installerExtension);
				extensionManager.Uninstall(installerExtension);
			}
			catch (Exception e)
			{
				var message = new StringBuilder();
				message.Append("Failed to remove the deprecated version of vsClojure.  Please use the extension manager to manually uninstall.");
				MessageBox.Show(message.ToString(), "vsClojure Uninstall Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}