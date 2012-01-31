using System.Runtime.InteropServices;
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
		public RepositoryEntry ExtensionRepositoryEntry =
			new RepositoryEntry()
			{
				DownloadUrl = "http://visualstudiogallery.msdn.microsoft.com/8938e63c-7527-443b-956d-84597b9bfc6f/file/64178/2/ClojureExtension.Deployment.vsix",
				Name = "vsClojure",
				VsixReferences = ""
			};

		protected override void Initialize()
		{
			base.Initialize();
			var dte = (DTE2) GetService(typeof (DTE));
			dte.Events.DTEEvents.OnStartupComplete += Download;
		}

		private void Download()
		{
			var prompt = new InstallationPrompt();

			prompt.InstallNowButton.Click +=
				(sender, args) =>
				{
					prompt.Close();
					InstallExtension();
				};

			prompt.RemindMeLaterButton.Click += (sender, args) => prompt.Close();
			prompt.ShowDialog();
		}

		private void InstallExtension()
		{
			var progressWindow = new InstallProgress();
			var extensionManager = GetGlobalService(typeof(SVsExtensionManager)) as IVsExtensionManager;
			var extensionRepository = GetGlobalService(typeof(SVsExtensionRepository)) as IVsExtensionRepository;
			
			progressWindow.RestartNowButton.IsEnabled = false;

			progressWindow.RestartNowButton.Click +=
				(sender, args) =>
				{
					progressWindow.Close();
					RestartVisualStudio();
				};

			extensionRepository.DownloadProgressChanged +=
				(sender, args) =>
				{
					progressWindow.CurrentStepText.Text = "Downloading...";
					progressWindow.DownloadPercentComplete.Minimum = 0;
					progressWindow.DownloadPercentComplete.Value = args.BytesReceived;
					progressWindow.DownloadPercentComplete.Maximum = args.TotalBytesToReceive;
					progressWindow.DownloadedAmountText.Text = ToKilobytesDisplay(args.BytesReceived) + " / " + ToKilobytesDisplay(args.TotalBytesToReceive);
				};

			extensionRepository.DownloadCompleted +=
				(sender, args) =>
				{
					progressWindow.CurrentStepText.Text = "Installing...";
					var installerExtension = extensionManager.GetInstalledExtension("40953a10-3425-499c-8162-a90059792d13");
					extensionManager.Install(args.Payload, false);
					extensionManager.Enable(extensionManager.GetInstalledExtension("7712178c-977f-45ec-adf6-e38108cc7739"));
					extensionManager.Disable(installerExtension);
					extensionManager.Uninstall(installerExtension);
					progressWindow.RestartNowButton.IsEnabled = true;
					progressWindow.CurrentStepText.Text = "Installation complete.";
				};

			extensionRepository.DownloadAsync(ExtensionRepositoryEntry);
			progressWindow.ShowDialog();
		}

		private void RestartVisualStudio()
		{
			var shell = (IVsShell4)GetService(typeof(SVsShell));
			shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);
		}

		private static string ToKilobytesDisplay(long bytes)
		{
			return (bytes / 1024) + "k";
		}
	}
}