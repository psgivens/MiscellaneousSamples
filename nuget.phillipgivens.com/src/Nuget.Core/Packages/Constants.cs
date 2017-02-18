using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NuGet
{
    public static class Constants
    {
        public static readonly string PackageExtension = ".nupkg";
        public static readonly string ManifestExtension = ".nuspec";
        public static readonly string ContentDirectory = "content";
        public static readonly string LibDirectory = "lib";
        public static readonly string ToolsDirectory = "tools";
        public static readonly string BinDirectory = "bin";
        public static readonly string SettingsFileName = "NuGet.Config";
        public static readonly string PackageReferenceFile = "packages.config";

        internal const string PackageServiceEntitySetName = "Packages";
        internal const string PackageRelationshipNamespace = "http://schemas.microsoft.com/packaging/2010/07/";

        // Starting from nuget 2.0, we use a file with the special name '_._' to represent an empty folder.
        internal const string PackageEmptyFileName = "_._";

        // This is temporary until we fix the gallery to have proper first class support for this.
        // The magic unpublished date is 1900-01-01T00:00:00
        public static readonly DateTimeOffset Unpublished = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security", 
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", 
            Justification="The type is immutable.")]
        public static readonly ICollection<string> AssemblyReferencesExtensions 
            = new ReadOnlyCollection<string>(new string[] { ".dll", ".exe", ".winmd" });
    }
}