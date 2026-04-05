using BrickVault;
using BrickVault.Types;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using RawFile = Diorama.Core.RawFile;

namespace Diorama.Tests
{
    public abstract class GscTestBase
    {
        private const uint OffsetBasis32 = 2166136261;
        private const uint Prime32 = 16777619;

        public static uint HashNextBytes(RawFile file, int n)
        {
            uint hash = OffsetBasis32;

            for (int i = 0; i < n; i++)
            {
                hash ^= file.ReadByte();
                hash *= Prime32;
            }

            return hash;
        }

        private bool TryParseFile(RawFile file, out string? errorMessage)
        {
            try
            {
                var scene = GScene.Parse(file);
                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private bool TryParseFile(RawFile file, out GScene? scene, out string? errorMessage)
        {
            try
            {
                scene = GScene.Parse(file);
                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                scene = null;
                return false;
            }
        }

        public bool TrySaveFile(GScene scene, RawFile file, out string? errorMessage)
        {
            try
            {
                scene.Write(file, new GSerializationContext());
                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private void AddFailedFile(Dictionary<string, GscFileFailureInfo> failed, string path, string reason)
        {
            if (reason.StartsWith("Attempting to read string"))
            {
                reason = "Invalid string read attempt";
            }

            if (!failed.TryGetValue(reason, out var failureInfo))
            {
                failureInfo = new GscFileFailureInfo();
                failed[reason] = failureInfo;
            }

            failureInfo.AddFile(path);
        }

        protected void MergeFailures(
            Dictionary<string, GscFileFailureInfo> global,
            Dictionary<string, GscFileFailureInfo> local)
        {
            foreach (var kvp in local)
            {
                var reason = kvp.Key;
                var localInfo = kvp.Value;

                if (!global.TryGetValue(reason, out var globalInfo))
                {
                    // No existing entry → just take it
                    global[reason] = new GscFileFailureInfo
                    {
                        Count = localInfo.Count,
                        Paths = new List<string>(localInfo.Paths)
                    };
                }
                else
                {
                    // Merge counts
                    globalInfo.Count += localInfo.Count;

                    // Merge paths (optionally cap)
                    foreach (var path in localInfo.Paths)
                    {
                        if (globalInfo.Paths.Count < 50) // optional cap
                        {
                            globalInfo.Paths.Add(path);
                        }
                    }
                }
            }
        }

        [TestCategory("Deserialization")]
        protected void DeserializeAll(string gamePath)
        {
            Dictionary<string, GscFileFailureInfo> globalFailed = new();

            int totalSucceeded = 0;
            int totalCount = 0;

            MassThreadExtractionContext.ForEachGscFile(
                gamePath,
                maxDegree: 2,
                initialize: () => new LocalContext(),
                process: (ctx, entry, file) =>
                {
                    ctx.Count++;

                    if (TryParseFile(file, out var error))
                    {
                        ctx.Succeeded++;
                    }
                    else
                    {
                        AddFailedFile(ctx.Failed, entry.Path, error);
                    }
                },
                finalize: ctx =>
                {
                    Interlocked.Add(ref totalSucceeded, ctx.Succeeded);
                    Interlocked.Add(ref totalCount, ctx.Count);

                    lock (globalFailed)
                    {   
                        MergeFailures(globalFailed, ctx.Failed);
                    }
                }
            );

            foreach (string failedReason in globalFailed.Keys)
            {
                var failureInfo = globalFailed[failedReason];

                Console.WriteLine($"{failedReason} - {failureInfo.Count}");
                foreach (var path in failureInfo.Paths)
                {
                    Console.WriteLine($"\t{path}");
                }
            }

            Console.WriteLine("--- FINAL RESULT ---");
            Console.WriteLine($"{totalSucceeded}/{totalCount}");
            if (totalSucceeded != totalCount)
            {
                Assert.Inconclusive($"{totalSucceeded}/{totalCount} ({(float)totalSucceeded/totalCount * 100:F1}%)");
            }
        }

        protected void ReserializeAll(string gamePath)
        {
            Dictionary<string, GscFileFailureInfo> globalFailed = new();

            int totalSucceeded = 0;
            int totalCount = 0;

            MassThreadExtractionContext.ForEachGscFile(
                gamePath,
                maxDegree: 2,
                initialize: () => new LocalContext(),
                process: (ctx, entry, file) =>
                {
                    ctx.Count++;

                    file.fileStream.SetLength(entry.DecompressedSize);

                    if (TryParseFile(file, out var scene, out var error))
                    {
                        file.Seek(0, SeekOrigin.Begin);

                        uint originalHash = HashNextBytes(file, (int)entry.DecompressedSize);

                        file.Seek(0, SeekOrigin.Begin);

                        if (TrySaveFile(scene, file, out var error2))
                        {
                            if (file.Position != entry.DecompressedSize)
                            {
                                AddFailedFile(ctx.Failed, entry.Path, "File not same size!");
                                return;
                            }

                            file.Seek(0, SeekOrigin.Begin);
                            uint newHash = HashNextBytes(file, (int)entry.DecompressedSize);

                            if (originalHash != newHash)
                            {
                                AddFailedFile(ctx.Failed, entry.Path, "File serialized wrong");
                            }
                            else
                            {
                                ctx.Succeeded++;
                            }

                        }
                        else
                        {
                            AddFailedFile(ctx.Failed, entry.Path, error2);
                        }
                    }
                    else
                    {
                        AddFailedFile(ctx.Failed, entry.Path, "Could not deserialize");
                    }
                },
                finalize: ctx =>
                {
                    Interlocked.Add(ref totalSucceeded, ctx.Succeeded);
                    Interlocked.Add(ref totalCount, ctx.Count);

                    lock (globalFailed)
                    {
                        MergeFailures(globalFailed, ctx.Failed);
                    }
                }
            );

            foreach (string failedReason in globalFailed.Keys)
            {
                var failureInfo = globalFailed[failedReason];

                Console.WriteLine($"{failedReason} - {failureInfo.Count}");
                foreach (var path in failureInfo.Paths)
                {
                    Console.WriteLine($"\t{path}");
                }
            }

            Console.WriteLine("--- FINAL RESULT ---");
            Console.WriteLine($"{totalSucceeded}/{totalCount}");
            if (totalSucceeded != totalCount)
            {
                Assert.Inconclusive($"{totalSucceeded}/{totalCount} ({(float)totalSucceeded / totalCount * 100:F1}%)");
            }
        }
    }
}
