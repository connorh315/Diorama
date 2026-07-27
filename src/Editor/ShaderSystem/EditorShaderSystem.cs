using Avalonia.Remote.Protocol;
using BrickVault.Types;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components.RESH;
using Diorama.Editor.Attributes;
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

        public static Dictionary<int, string> IndexedFiles = Cache.FileTree.GetIndexedFiles();

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
            string datLocation = AppSettings.Settings.DatLocation;

            if (string.IsNullOrEmpty(datLocation))
            {
                progress?.Report(new FingerprintCacheProgress
                {
                    Current = 0,
                    Status = "Failed - DAT archive location not set!"
                });

                return;
            }

            ShaderFingerprintCache fCache = new ShaderFingerprintCache();

            fCache.ArchivesLocation = datLocation;

            Dictionary<string, List<(ShaderFingerprint, ShaderSetArray)>> fingerprintsByScene = new();

            EditorMaterial viewModel = new EditorMaterial();

            int processed = 0;

            using (RawFile file = new RawFile(new MemoryStream()))
            {
                foreach (var datPath in Directory.EnumerateFiles(datLocation, "*.DAT", SearchOption.AllDirectories))
                {
                    DATFile dat = DATFile.Open(datPath);

                    if (dat == null)
                    {
                        Console.WriteLine($"Skipping {datPath} - Invalid DAT archive!");
                        continue;
                    }

                    using (var ctx = dat.GetExtractionContext())
                    {
                        string datName = Path.GetFileNameWithoutExtension(datPath);

                        foreach (var sceneEntry in dat.GetFilesWithExtension("gsc"))
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            file.Seek(0, SeekOrigin.Begin);

                            dat.ExtractFile(sceneEntry, ctx, file.fileStream);

                            string path = $"{datName}\\{sceneEntry.Path}";
                        
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
                    }
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
