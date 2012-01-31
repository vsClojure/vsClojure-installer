using Microsoft.VisualStudio.ExtensionManager;

namespace vsClojure.VisualStudio_Clojure_Installer
{
	public class RepositoryEntry : IRepositoryEntry
	{
		public string Name { get; set; }
		public string DownloadUrl { get; set; }
		public string VsixReferences { get; set; }
	}
}