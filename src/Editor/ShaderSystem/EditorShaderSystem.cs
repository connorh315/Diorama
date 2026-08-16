using Avalonia.Remote.Protocol;
using BrickVault.Types;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components.RESH;
using Diorama.Core.IO;
using Diorama.Editor.Attributes;
using Diorama.Editor.Material;
using Diorama.UI.Progress;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class EditorShaderSystem
    {
        public static ShaderFingerprintCache Cache = Load();

        const string FingerprintFilename = "materialfingerprint.cache";

        public static Dictionary<int, string> IndexedFiles = Cache?.FileTree?.GetIndexedFiles();

        public static List<ShaderSetArray> GetShaderSet()
        {
            using (RawFile file = new RawFile(FingerprintFilename))
            {
                Cache.OpenShaderSet(new SchemaSerializer(file, false));

                return Cache.SetArray;
            }
        }

        public static ShaderSetArray GetSet(List<ShaderSetArray> setArrays, int fileIndex, string materialName)
        {
            foreach (var set in setArrays)
            {
                if (set.FileIndex == fileIndex && set.MaterialName == materialName)
                {
                    return set;
                }
            }

            return null;
        }

        public static IEnumerable<(string, ShaderFingerprint)> Enumerate()
        {
            if (Cache == null) yield break;

            int fileIndex = -1;
            string path = "";
            foreach (var fingerprint in Cache.Cache)
            {
                if (fingerprint.FileIndex != fileIndex)
                {
                    path = IndexedFiles[fingerprint.FileIndex];
                    fileIndex = fingerprint.FileIndex;
                }

                yield return (path, fingerprint);
            }
        }

        public static Task CreateAsync(IProgress<FingerprintCacheProgress> progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                CreateInternal(progress, cancellationToken);
            }, cancellationToken);
        }

        private static void CreateInternal(IProgress<FingerprintCacheProgress> progress, CancellationToken cancellationToken)
        {
            string location = AppSettings.Settings.ProviderPath;

            if (string.IsNullOrEmpty(location))
            {
                progress?.Report(new FingerprintCacheProgress
                {
                    Current = 0,
                    Status = "Failed - Files location not set!"
                });

                return;
            }

            ShaderFingerprintCache fCache = new ShaderFingerprintCache();

            fCache.ProviderLocation = location;

            Dictionary<string, List<(ShaderFingerprint, ShaderSetArray)>> fingerprintsByScene = new();

            EditorMaterial viewModel = new EditorMaterial();

            int processed = 0;

            foreach (var file in FileProvider.EnumerateFiles("gsc", "ghg"))
            {
                cancellationToken.ThrowIfCancellationRequested();

                string path = file.FileLocation.ToString();

                FileLocation filePath = file.FileLocation;
                if (filePath is ArchiveFileLocation archiveFilePath)
                {
                    path = $"{archiveFilePath.ArchiveName}\\{archiveFilePath.ArchiveFilePath}";
                }

                try
                {
                    GScene scene = GScene.Parse(file);

                    progress.Report(new FingerprintCacheProgress()
                    {
                        Current = processed,
                        Status = $"Processing {path}"
                    });

                    if (scene.MaterialBlock.Materials.Length == 0) // likely empty file
                        continue;

                    fingerprintsByScene.Add(path, new List<(ShaderFingerprint, ShaderSetArray)>());

                    foreach (var mat in scene.MaterialBlock.Materials)
                    {
                        var matData = fCache.AddMaterial(mat);
                        fingerprintsByScene[path].Add(matData);
                    }

                    processed++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not parse file {path}: {ex.Message}");
                }
            }

            var paths = fingerprintsByScene.Keys.OrderBy(x => x).ToList();

            fCache.FileTree = NuFileTree.FromPaths(paths, 1);

            foreach ((string sceneName, var fingerprints) in fingerprintsByScene)
            {
                short fileIndex = fCache.FileTree.PathIndexes[sceneName];
                foreach ((ShaderFingerprint fingerprint, ShaderSetArray array) in fingerprints)
                {
                    fingerprint.FileIndex = fileIndex;
                    array.FileIndex = fileIndex;
                }
            }

            using (RawFile fingerprintFile = new RawFile(FingerprintFilename))
            {
                SchemaSerializer schema = new SchemaSerializer(fingerprintFile, true);

                fCache.Handle(schema, 0);
            }

            progress.Report(new FingerprintCacheProgress()
            {
                Current = processed,
                Status = $"Complete!"
            });

            Cache = fCache;
        }

        public static PropertyInfo[] GetProperties(EditorMaterial viewModel) => viewModel.GetType()
                                        .GetProperties()
                                        .Where(p => p.IsDefined(typeof(RequiresShaderChangeAttribute), false))
                                        .OrderBy(p => p.Name)
                                        .ToArray();

        public static ShaderFingerprintCache Load()
        {
            try
            {
                if (!Path.Exists(FingerprintFilename))
                {
                    Console.WriteLine("Could not find a material fingerprint cache");
                    return null;
                }

                using (RawFile fingerprintFile = new RawFile(FingerprintFilename))
                {
                    SchemaSerializer schema = new SchemaSerializer(fingerprintFile, false);

                    ShaderFingerprintCache cache = new ShaderFingerprintCache();

                    cache.Handle(schema, 0);

                    Console.WriteLine($"Successfully loaded material fingerprint cache");

                    return cache;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load material fingerprint cache. Rebuild it to fix this error.");

                return null;
            }
        }
    }
}
