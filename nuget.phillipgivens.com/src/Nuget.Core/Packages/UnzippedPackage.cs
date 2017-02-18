﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NuGet.Resources;

namespace NuGet
{
    /// <summary>
    /// An unzipped package has its contents laid out as physical files on disk inside a directory, 
    /// instead of inside a .nupkg file.
    /// </summary>
    /// <remarks>
    /// An unzipped package is strictly required to have this directory structure: ([] denotes directory)
    /// 
    ///    jQuery.1.0.nupkg
    ///    [jQuery.1.0]
    ///         jQuery.1.0.nuspec
    ///         [content]
    ///              jQuery.js
    ///         [lib]
    ///              jQuery.dll
    ///         [tools]
    ///              install.ps1
    /// </remarks>
    internal class UnzippedPackage : LocalPackage
    {
        private readonly IFileSystem _repositoryFileSystem;
        private readonly string _packageFileName;
        private readonly string _packageName;

        /// <summary>
        /// Create an uninstance of UnzippedPackage class
        /// </summary>
        /// <param name="repositoryDirectory">The root directory which contains the .nupkg file and the corresponding unippws directory.</param>
        /// <param name="packageName">Contains the file name without the extension of the nupkg file.</param>
        public UnzippedPackage(string repositoryDirectory, string packageName)
            : this(new PhysicalFileSystem(repositoryDirectory), packageName)
        {
        }

        public UnzippedPackage(IFileSystem repositoryFileSystem, string packageName)
        {
            if (repositoryFileSystem == null)
            {
                throw new ArgumentNullException("repositoryFileSystem");
            }

            if (String.IsNullOrEmpty(packageName))
            {
                throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packageName");
            }

            _packageName = packageName;
            _packageFileName = packageName + Constants.PackageExtension;
            _repositoryFileSystem = repositoryFileSystem;

            EnsureManifest();
        }

        private void EnsureManifest()
        {
            // we look for the .nuspec file at jQuery.1.4\jQuery.1.4.nuspec
            string manifestFile = Path.Combine(_packageName, _packageName + Constants.ManifestExtension);
            if (!_repositoryFileSystem.FileExists(manifestFile))
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_NotFound, _repositoryFileSystem.GetFullPath(manifestFile)));
            }

            using (Stream manifestStream = _repositoryFileSystem.OpenFile(manifestFile))
            {
                ReadManifest(manifestStream);
            }
        }

        public override Stream GetStream()
        {
            return _repositoryFileSystem.OpenFile(_packageFileName);
        }

        protected override IEnumerable<IPackageFile> GetFilesBase()
        {
            return from p in _repositoryFileSystem.GetFiles(_packageName, "*.*", recursive: true)
                   where !PackageUtility.IsManifest(p)
                   select new PhysicalPackageFile 
                          {
                              SourcePath = _repositoryFileSystem.GetFullPath(p),
                              TargetPath = p.Substring(_packageName.Length + 1)     // only count from the package root
                          };
        }

        protected override IEnumerable<IPackageAssemblyReference> GetAssemblyReferencesBase()
        {
            string libDirectory = Path.Combine(_packageName, Constants.LibDirectory);

            return from p in _repositoryFileSystem.GetFiles(libDirectory, "*.*", recursive: true)
                   let targetPath = p.Substring(_packageName.Length + 1)        // only count from the package root
                   where IsAssemblyReference(targetPath)
                   let file = new PhysicalPackageFile(()=>_repositoryFileSystem.OpenFile(p))
                              {
                                  SourcePath = _repositoryFileSystem.GetFullPath(p),
                                  TargetPath = targetPath
                              }
                   select new ZipPackageAssemblyReference(file);
        }
    }
}