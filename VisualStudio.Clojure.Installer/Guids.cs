// Guids.cs
// MUST match guids.h
using System;

namespace vsClojure.VisualStudio_Clojure_Installer
{
    static class GuidList
    {
        public const string guidVisualStudio_Clojure_InstallerPkgString = "ae6c2b24-8e4f-42f3-ac8f-c4d88d14fd87";
        public const string guidVisualStudio_Clojure_InstallerCmdSetString = "8a0db042-89b5-45d3-9c78-87c7c90231dc";

        public static readonly Guid guidVisualStudio_Clojure_InstallerCmdSet = new Guid(guidVisualStudio_Clojure_InstallerCmdSetString);
    };
}