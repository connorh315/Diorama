using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components.RESH
{
    public class NuFileTree : ISchemaSerializable
    {
        public uint Version;
        private int FileCount;
        private int SegmentCount;

        private byte[] StringBuffer;

        private NuResourceSegment[] Segments;

        private int NoHash;

        private Dictionary<int, string> DebugFilepaths => GetIndexedFiles();

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Version);
            schema.HandleInt(ref FileCount);
            schema.HandleInt(ref SegmentCount);
            schema.HandleBuffer(ref StringBuffer);

            if (!schema.Writing)
            {
                Segments = new NuResourceSegment[SegmentCount];
            }

            for (int i = 0; i < SegmentCount; i++)
            {
                Segments[i].Handle(schema, Version);
            }

            schema.HandleInt(ref NoHash);
        }

        public string GetSegment(int index)
        {
            string combined = "";
            while (true)
            {
                byte currByte = StringBuffer[index++];
                if (currByte == 0) break;

                combined += (char)currByte;
            }

            return combined;
        }

        private static IEnumerable<short> GetChildren(List<NuResourceSegment> segments, NuResourceSegment parent)
        {
            short current = parent.FinalChild;
            while (current > 0)
            {
                yield return current;

                current = segments[current].PreviousSibling;
            }
        }

        public IEnumerable<short> GetChildren(NuResourceSegment parent)
        {
            short current = parent.FinalChild;
            while (current > 0)
            {
                yield return current;

                current = Segments[current].PreviousSibling;
            }
        }

        static Dictionary<int, string> BuildPaths(RawFile segmentBuffer, NuResourceSegment[] segments, int pathCount)
        {
            var cache = new string[segments.Length];

            string[] paths = new string[pathCount];

            string Build(int index)
            {
                if (cache[index] != null)
                    return cache[index];

                var seg = segments[index];
                string name = string.Empty;
                if (seg.SegmentIndex > -1)
                {
                    segmentBuffer.Seek(seg.SegmentIndex, SeekOrigin.Begin);
                    name = segmentBuffer.ReadNullString();
                }

                if (seg.ParentIndex < 1)
                    return cache[index] = name;

                var parentPath = Build(seg.ParentIndex);

                return cache[index] = string.IsNullOrEmpty(parentPath)
                    ? name
                    : parentPath + "\\" + name;
            }

            Dictionary<int, string> indexedPaths = new();

            for (int i = 0; i < segments.Length; i++)
            {
                string path = Build(i);
                if (segments[i].FinalChild < 1)
                {
                    indexedPaths.Add(i, path);
                }
            }

            return indexedPaths;
        }

        public Dictionary<int, string> GetIndexedFiles()
        {
            using (RawFile segmentBuffer = new RawFile(new MemoryStream(StringBuffer)))
            {
                return BuildPaths(segmentBuffer, Segments, FileCount);
            }
        }

        public static NuFileTree FromPaths(IEnumerable<string> paths, uint versionToUse)
        {
            NuFileTree filetree = new NuFileTree();
            filetree.Version = versionToUse;
            filetree.FileCount = paths.Count();

            List<NuResourceSegment> segments = new List<NuResourceSegment>();

            segments.Add(new NuResourceSegment
            {
                FinalChild = 0,
                PreviousSibling = 0,
                SegmentIndex = -1,
                ParentIndex = 0,
                FileIndex = 0
            });

            Dictionary<int, string> segmentDictionary = new Dictionary<int, string>();
            int currentSegmentOffset = 0;

            ushort fileId = 0;
            foreach (string rawPath in paths)
            {
                string sanitised = NuExtensions.StandardiseLower(rawPath);
                string[] split = sanitised.Split('\\');

                short parent = 0;
                for (int i = 0; i < split.Length; i++)
                {
                    bool found = false;
                    short lastChild = 0;
                    bool isChild = i == split.Length - 1;
                   
                    if (!isChild)
                    {
                        foreach (short child in NuFileTree.GetChildren(segments, segments[parent]))
                        {
                            if (segmentDictionary[segments[child].SegmentIndex] == split[i])
                            {
                                found = true;
                                parent = child;
                                break;
                            }

                            lastChild = child;
                        }
                    }

                    if (!found)
                    {
                        NuResourceSegment childSegment = new NuResourceSegment()
                        {
                            ParentIndex = parent,
                            FileIndex = (ushort)(isChild ? fileId++ : 0),
                            PreviousSibling = lastChild,
                            FinalChild = (short)(isChild ? -1 : 0),
                            SegmentIndex = currentSegmentOffset
                        };
                        segmentDictionary.Add(currentSegmentOffset, split[i]);
                        currentSegmentOffset += split[i].Length + 2;

                        short childIndex = (short)segments.Count;

                        var parentSeg = segments[parent];
                        parentSeg.FinalChild = childIndex;
                        segments[parent] = parentSeg;

                        segments.Add(childSegment);
                        parent = childIndex;
                    }
                }
            }

            filetree.Segments = segments.ToArray();
            filetree.SegmentCount = segments.Count;

            using (RawFile segmentStream = new RawFile(new MemoryStream()))
            {
                foreach (string segment in segmentDictionary.Values)
                {
                    segmentStream.WriteString(segment, 2);
                }
                segmentStream.WritePadding(2);
                filetree.StringBuffer = ((MemoryStream)segmentStream.fileStream).ToArray();
            }


            return filetree;
        }
    }

    public struct NuResourceSegment : ISchemaSerializable
    {
        public short FinalChild;
        public short PreviousSibling;
        public int SegmentIndex;
        public short ParentIndex;
        public ushort FileIndex;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleShort(ref FinalChild);
            schema.HandleShort(ref PreviousSibling);
            schema.HandleInt(ref SegmentIndex);
            schema.HandleShort(ref ParentIndex);
            if (parentVersion > 1)
            {
                schema.HandleUShort(ref FileIndex);
            }
        }
    }
}
