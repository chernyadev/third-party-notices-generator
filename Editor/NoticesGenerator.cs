using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Editor
{
    public static class NoticesGenerator
    {
        public const string DefaultFileName = "THIRD-PARTY-NOTICES.md";

        public const string DefaultHeader =
            "This project contains third-party software components governed by the license(s) indicated below:";

        private const string Separator = "\n\n----------------------------------------\n\n";

        /// <summary>
        /// Request list of used packages and generate notices file. Could be used in build scripts for CI.
        /// </summary>
        /// <param name="offlineMode">Specifies whether or not the Package Manager requests the latest information about the project's packages from the remote Unity package registry. When offlineMode is true, the PackageManager.PackageInfo objects in the PackageCollection returned by the Package Manager contain information obtained from the local package cache, which could be out of date.</param>
        /// <param name="includeIndirectDependencies">Set to true to include indirect dependencies in the PackageCollection returned by the Package Manager. Indirect dependencies include packages referenced in the manifests of project packages or in the manifests of other indirect dependencies. Set to false to include only the packages listed directly in the project manifest.
        /// Note: The version reported might not match the version requested in the project manifest.</param>
        /// <param name="excludeBuiltInPackages">Skip built-in Unity packages.</param>
        /// <param name="writeToStreamingAssetsFolder">Write the file to Streaming Assets folder.</param>
        /// <param name="fileName">Custom filename for the notices file.</param>
        /// <param name="header">Custom header for the notices file.</param>
        public static async Task Generate(bool offlineMode = false, bool includeIndirectDependencies = true,
            bool excludeBuiltInPackages = true, bool writeToStreamingAssetsFolder = true,
            string fileName = DefaultFileName, string header = DefaultHeader)
        {
            var request = Client.List(offlineMode, includeIndirectDependencies);

            while (!request.IsCompleted)
            {
                await Task.Yield();
            }

            if (request.Status == StatusCode.Success)
            {
                Write(request.Result, excludeBuiltInPackages, writeToStreamingAssetsFolder, fileName, header);
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.Log(request.Error.message);
            }
        }

        private static void Write(PackageCollection packageCollection, bool excludeBuiltInPackages,
            bool writeToStreamingAssetsFolder, string fileName, string header)
        {
            var notices = new List<string> { header };

            foreach (var package in packageCollection)
            {
                if (excludeBuiltInPackages && package.source == PackageSource.BuiltIn)
                {
                    continue;
                }

                var licenseFile = Directory
                    .GetFiles(package.resolvedPath)
                    .FirstOrDefault(f => Path.GetFileNameWithoutExtension((string)f).ToLower() == "license");

                if (licenseFile == null || !File.Exists(licenseFile))
                {
                    Debug.Log($"No license file found, skipping: {package.name}");
                    continue;
                }

                var licenseText = File.ReadAllText(licenseFile);
                var packageData = $"### {package.displayName}\n" +
                                  $"#### Package: [{package.name}]({package})\n" +
                                  $"#### Version: {package.version}\n" +
                                  $"#### [Documentation]({package.documentationUrl})\n" +
                                  $"{licenseText}";

                notices.Add(packageData);
            }

            if (writeToStreamingAssetsFolder)
            {
                fileName = Path.Combine(Application.streamingAssetsPath, fileName);
            }

            var file = new FileInfo(fileName);
            file.Directory?.Create();
            File.WriteAllText(file.FullName, string.Join(Separator, notices));

            AssetDatabase.Refresh();
        }
    }
}
