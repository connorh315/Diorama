
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.GSC.Components.RESH;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor.Metadata;
using Diorama.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public static class GSceneConverter
    {
        public static EditorScene FromGScene(string filePath)
        {
            GScene scene = GScene.Parse(filePath);

            EditorScene editorScene = new EditorScene();
            editorScene.OriginalScene = scene;
            editorScene.Name = Path.GetFileName(filePath);
            editorScene.SceneTransform = Matrix4.CreateScale(1f, 1f, -1f); // All meshes are flipped, so this unflips them

            editorScene.Metadata = GetMetadata(scene);            

            Dictionary<ushort[], RenderIndicesBuffer> convertedIBuffer = new();
            Dictionary<VertexList, RenderVertexBuffer> convertedVBuffer = new();

            RenderMesh[] meshes = new RenderMesh[scene.MeshSceneBlock.Meshes.Length];
            for (int i = 0; i < scene.MeshSceneBlock.Meshes.Length; i++)
            {
                NuRenderMesh nuMesh = scene.MeshSceneBlock.Meshes[i];


                RenderVertexBuffer[] vBuffers = new RenderVertexBuffer[nuMesh.VertexBuffers.Length];
                for (int j = 0; j < vBuffers.Length; j++)
                {
                    var buffer = nuMesh.VertexBuffers[j];

                    if (!convertedVBuffer.ContainsKey(buffer))
                    {
                        var vBuffer = RenderVertexBuffer.FromBuffer(buffer);
                        convertedVBuffer.Add(buffer, vBuffer);
                        editorScene.GetOrAdd(vBuffer);
                    }

                    vBuffers[j] = convertedVBuffer[buffer];
                }

                var indices = nuMesh.Indices;
                if (!convertedIBuffer.ContainsKey(indices))
                {
                    var ibu = RenderIndicesBuffer.FromBuffer(indices);
                    convertedIBuffer.Add(indices, ibu);
                    editorScene.GetOrAdd(ibu);
                }

                RenderIndicesBuffer iBuffer = convertedIBuffer[nuMesh.Indices];

                RenderMesh mesh = new RenderMesh(vBuffers, iBuffer);
                mesh.VerticesBase = (int)nuMesh.VerticesBase;
                mesh.VerticesCount = (int)nuMesh.VerticesCount;
                mesh.IndicesBase = (int)nuMesh.IndicesBase;
                mesh.IndicesCount = (int)nuMesh.IndicesCount;

                mesh.OriginalMesh = nuMesh;

                meshes[i] = mesh;
            }

            var textures = new List<RenderTexture>();

            try
            {
                var nxg_textures = NxgTextures.Read(Path.ChangeExtension(filePath, "nxg_textures"));
                if (nxg_textures != null)
                {
                    for (int i = 0; i < nxg_textures.TextureSet.Textures.Length; i++)
                    {
                        textures.Add(RenderTexture.FromNuTexture(nxg_textures.TextureSet.Textures[i]));
                    }
                    editorScene.OriginalTextures = nxg_textures;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No texture sheet found for scene, using blank textures");
            }

            // TODO: Just do the reference sorting here instead
            EditorMaterial[] materials = new EditorMaterial[scene.MaterialBlock.Materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                NuMaterialData nuMaterialData = scene.MaterialBlock.Materials[i];

                EditorMaterial material = new EditorMaterial();

                material.Original = nuMaterialData;
                material.OriginalIndex = i;

                materials[i] = material;

                editorScene.Materials.Add(material);
            }

            List<NuLightmapData> gsceneLightmaps = scene.LightmapDataBlock.Lightmaps;
            EditorLightmap[] lightmaps = new EditorLightmap[scene.LightmapDataBlock.Lightmaps.Count];
            for (int i = 0; i < lightmaps.Length; i++)
            {
                NuLightmapData nuLightmap = gsceneLightmaps[i];
                EditorLightmap lightmap = new EditorLightmap();
                lightmap.Original = nuLightmap;

                lightmap.AmbientOcclusion = ResolveTexture(textures, nuLightmap.AoTID);
                lightmap.Smooth = ResolveTexture(textures, nuLightmap.SmoothTID);
                lightmap.Directional0 = ResolveTexture(textures, nuLightmap.DirectionalTIDs0);
                lightmap.Directional1 = ResolveTexture(textures, nuLightmap.DirectionalTIDs1);
                lightmap.Directional2 = ResolveTexture(textures, nuLightmap.DirectionalTIDs2);
                lightmap.Offsets[0] = nuLightmap.TexCoordOffset0;
                lightmap.Offsets[1] = nuLightmap.TexCoordOffset1;
                lightmap.Scales[0] = nuLightmap.TexCoordScale0;
                lightmap.Scales[1] = nuLightmap.TexCoordScale1;

                lightmaps[i] = lightmap;
            }

            var display = scene.DisplayScene;
            int matrixId = -1;
            int materialId = -1;
            int lightmapId = -1;
            Dictionary<int, EditorGeometryObject> geometry = new();

            List<EditorClipObject> allClipObjects = new();

            if (display.DisplayItems != null)
            {
                for (int commandId = 0; commandId < display.DisplayItems.Count; commandId++)
                {
                    NuDefunctDisplayItem command = display.DisplayItems[commandId];
                    switch (command.Command)
                    {
                        case DisplayCommand.Material:
                            materialId = (int)command.Index;
                            break;
                        case DisplayCommand.LightMap:
                            lightmapId = (int)command.Index;
                            break;
                        case DisplayCommand.MaterialClip:
                            break;
                        case DisplayCommand.Matrix:
                            matrixId = (int)command.Index;
                            break;
                        case DisplayCommand.DynamicGeo:
                            break;
                        case DisplayCommand.Mesh:
                            NuTransformMtx local = display.TransformMtxs[matrixId];

                            Matrix4 mtx = local.AsMatrix();

                            RenderMesh mesh = meshes[command.Index];

                            EditorGeometryObject obj = new EditorGeometryObject();
                            obj.OriginalTransform = local;
                            obj.Mesh = mesh;
                            if (local.IsZero())
                            {
                                mtx = Matrix4.Identity;
                                obj.CanEditTransform = false;
                            }
                            obj.Transform = mtx;
                            if (materialId > -1)
                            {
                                obj.Material = materials[materialId];
                            }
                            if (lightmapId > 0)
                            {
                                obj.Lightmap = lightmaps[lightmapId];
                            }

                            //Meshes.Add(mesh);
                            geometry.Add(commandId, obj);
                            //editorScene.Objects.Add(obj);
                            break;
                    }
                }

                foreach (var displayClip in display.ClipObjects)
                {
                    EditorClipObject clip = new EditorClipObject();
                    foreach (var el in displayClip.Elements)
                    {
                        if (!geometry.ContainsKey(el.OldGeometryIndex)) continue;
                        var geo = geometry[el.OldGeometryIndex];
                        clip.Elements.Add(geo);
                        geo.Parent = clip; // TODO: Remove
                        geo.Material = materials[el.OldMaterialIndex];
                        geo.Original = el;
                    }

                    allClipObjects.Add(clip);
                    clip.SceneOwner = editorScene;
                }
            }
            else
            {
                foreach (var displayClip in display.ClipObjects)
                {
                    EditorClipObject clip = new EditorClipObject();
                    foreach (var el in displayClip.Elements)
                    {
                        NuTransformMtx local = display.TransformMtxs[el.TransformIndex];
                        Matrix4 mtx = local.AsMatrix();
                        RenderMesh mesh = meshes[el.MeshIndex];
                        EditorGeometryObject obj = new EditorGeometryObject();
                        obj.OriginalTransform = local;
                        obj.Transform = mtx;
                        if (local.IsZero())
                        {
                            mtx = Matrix4.Identity;
                            obj.CanEditTransform = false;
                        }
                        obj.Transform = mtx;
                        obj.Mesh = mesh;
                        if (el.MaterialIndex > -1)
                        {
                            obj.Material = materials[el.MaterialIndex];
                        }
                        if (el.LightmapIndex > -1 && lightmaps.Length > el.LightmapIndex)
                        {
                            obj.Lightmap = lightmaps[el.LightmapIndex];
                        }

                        clip.Elements.Add(obj);
                        obj.Parent = clip;
                        obj.Original = el;
                        //geometry.Add(i, obj);
                    }
                    allClipObjects.Add(clip);
                    clip.SceneOwner = editorScene;
                }
            }


            for (int i = 0; i < display.SceneInstances.Count; i++)
            {
                var instance = display.SceneInstances[i];
                EditorSceneObject sceneObject = new EditorSceneObject();
                editorScene.Objects.Add(sceneObject);
                sceneObject.Name = $"SceneInstance_{i}";
                sceneObject.FadeDistances = instance.FadeDistances; // TODO: probably dangerous?
                sceneObject.ApproxSize = instance.ApproxSize;

                var geoBounds = display.BoundsCenterAndDistSqrd[i];
                sceneObject.BoundsCenterAndDistSqrd = new Vector4(geoBounds.X, geoBounds.Y, geoBounds.Z, geoBounds.W);

                var extents = display.BoundsExtentsAndRadius[i];
                sceneObject.BoundsExtentsAndRadius = new Vector4(extents.X, extents.Y, extents.Z, extents.W);

                if (instance.ClipObjectIndex > -1)
                {
                    sceneObject.ClipObject = allClipObjects[instance.ClipObjectIndex];
                    sceneObject.ClipObject.Parent = sceneObject;
                }

                if (instance.HasLods && instance.ClipObjectIndex != -1)
                {
                    sceneObject.Lods = new EditorLodGroup[4];

                    for (int j = 0; j < instance.Lods.Length; j++)
                    {
                        var lod = instance.Lods[j];
                        sceneObject.Lods[j] = new(j);
                        sceneObject.Lods[j].FadeDistance = instance.FadeDistances[j];

                        if (lod.NumInstances == 0) continue;

                        var lodClip = allClipObjects[lod.FirstInstance];
                        sceneObject.Lods[j].ClipObject = lodClip;
                        lodClip.Parent = sceneObject;
                    }

                    sceneObject.UseLodGroups = true;
                }
            }

            for (int i = 0; i < display.SpecialObjects.Count; i++)
            {
                var specialObject = display.SpecialObjects[i];
                if (specialObject.InstanceIndex != -1)
                {
                    var sceneObject = editorScene.Objects[specialObject.InstanceIndex];
                    sceneObject.Name = specialObject.Name;
                    sceneObject.SpecialObject = specialObject;
                }
            }

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];

                mat.Diffuse0 = ResolveTexture(textures, mat.Original.Diffuse0Index);
                mat.Diffuse1 = ResolveTexture(textures, mat.Original.Diffuse1Index);

                mat.Normal0 = ResolveTexture(textures, mat.Original.Normal0Index);
                mat.Normal1 = ResolveTexture(textures, mat.Original.Normal1Index);

                mat.Specular0 = ResolveTexture(textures, mat.Original.Specular0Index);

                mat.EnvMap = ResolveTexture(textures, mat.Original.EnvMap);

                //mat.DiffuseLayerBlend = (EditorDiffuseBlendMode)(mat.Original.baseDiffuseUsage);
                //mat.Diffuse1LayerBlend = (EditorDiffuseBlendMode)mat.Original.layerBlendDiffuse;

                //mat.Diffuse0UVSet = mat.Original.uvBlocks[0].UVSet;
                //mat.Diffuse1UVSet = mat.Original.uvBlocks[1].UVSet;
                //mat.Normal0UVSet = mat.Original.uvBlocks[4].UVSet;
                //mat.Normal1UVSet = mat.Original.uvBlocks[5].UVSet;
                //mat.Specular0UVSet = mat.Original.uvBlocks[12].UVSet;
                //mat.EnvMapUVSet = mat.Original.uvBlocks[16].UVSet;

                //mat.PerLayerUVScale1 = mat.Original.PerLayerUVScale1;
                //mat.PerLayerUVScale2 = mat.Original.PerLayerUVScale2;

                mat.LightmapUVSet = mat.Original.LightmapUVSet;

                //mat.Name = mat.Original.MaterialName;

                //mat.Occlusion = mat.Original.occlusion;

                //mat.RefractiveIndex = mat.Original.KRefractiveIndex;

                //mat.BlendMode = mat.Original.blendMode;
                //mat.AlphaTest = mat.Original.alphaTest;
                //mat.AlphaRef = mat.Original.Aref / 255f;
                //mat.CanAlphaBlend = mat.Original.miscFlags_canAlphaBlend;
                mat.Opaque = mat.Original.miscFlags_defunctOpaque;
                mat.SortLast = mat.Original.SortLast;
                mat.VertexControlledTint = mat.Original.VertexFlags_VertexControlledTint;

                //mat.ShadowImpostor = ConvertToBool(mat.Original.ShadowImpostor);

                //mat.PerLayerScale = ConvertToBool(mat.Original.materialFlags_per_layer_uvscale);

                mat.Colour = ConvertToBool(mat.Original.Colour);

                uint abgr = (uint)mat.Original.Colour1;
                float a = ((abgr >> 24) & 0xFF) / 255f;
                float b = ((abgr >> 16) & 0xFF) / 255f;
                float g = ((abgr >> 8) & 0xFF) / 255f;
                float r = ((abgr >> 0) & 0xFF) / 255f;
                mat.Colour1 = new Vector4(r, g, b, a);
            }

            editorScene.Textures = new ObservableCollection<RenderTexture>(textures);

            return editorScene;
        }

        private static bool ConvertToBool(byte val)
        {
            if (val == 1)
                return true;
            if (val == 0)
                return false;
            throw new Exception("Invalid variable contents!");
        }

        static RenderTexture ResolveTexture(List<RenderTexture> textures, int index)
        {
            if (index < 0 || textures.Count <= (index))
                return RenderTexture.GetWhiteTexture();

            return textures[index];
        }

        static EditorMetadata GetMetadata(GScene scene)
        {
            Dictionary<int, string> files = scene.ResourceHeader.FileTree.GetIndexedFiles();

            EditorMetadata metadata = new EditorMetadata();
            foreach (var reference in scene.ResourceHeader.References)
            {
                EditorResourceReference editorRef = new EditorResourceReference();
                
                if (!files.TryGetValue((int)reference.Hash, out string path))
                {
                    throw new Exception("File not found in resource header!");
                }

                editorRef.FilePath = path;
                editorRef.Type = reference.Type;
                editorRef.PlatformsAndClasses = reference.PlatformsAndClasses;

                if (reference.Checksum != null)
                {
                    editorRef.Checksum = reference.Checksum.Checksum;
                }
                
                metadata.Resources.Add(editorRef);
            }

            return metadata;
        }

        public static void Write(EditorScene scene)
        {
            var nuScene = scene.OriginalScene;
            ConvertResourceHeader(scene);
            ConvertMaterials(scene);
            ConvertMetadata(scene);


            string path = nuScene.Path;

#if DEBUG
            path = path.Replace(".GSC", "_1.GSC").Replace(".GHG", "_1.GHG");
#endif

            using (RawFile file = RawFile.Create(path))
            {
                GSerializationContext ctx = new GSerializationContext();
                nuScene.Write(file, ctx);
            }
        }

        public static void ConvertMaterials(EditorScene scene)
        {
            foreach (var mat in scene.Materials)
            {
                mat.Original.Diffuse0Index = scene.Textures.IndexOf(mat.Diffuse0);
                mat.Original.Diffuse1Index = scene.Textures.IndexOf(mat.Diffuse1);
                mat.Original.Normal0Index = scene.Textures.IndexOf(mat.Normal0);
                mat.Original.Normal1Index = scene.Textures.IndexOf(mat.Normal1);

                mat.Original.Specular0Index = scene.Textures.IndexOf(mat.Specular0);

                mat.Original.OldTid = mat.Original.Diffuse0Index;
            }
        }

        public static void ConvertResourceHeader(EditorScene scene)
        {
            var nuScene = scene.OriginalScene;
            var rawResources = scene.Metadata.Resources;

            List<EditorResourceReference> resources = scene.Metadata.Resources.OrderBy(x => x.Type).ToList();

            int referenceCount = scene.Metadata.Resources.Count;

            List<NuResourceReference> references = new List<NuResourceReference>();

            string[] paths = new string[referenceCount];

            for (int i = 0; i < referenceCount; i++)
            {
                paths[referenceCount - 1 - i] = resources[i].FilePath = NuExtensions.StandardiseLower(resources[i].FilePath);
            }

            NuFileTree filetree = NuFileTree.FromPaths(paths, scene.OriginalScene.ResourceHeader.FileTree.Version);

            Dictionary<int, string> fileDictionary = filetree.GetIndexedFiles();

            foreach (var reference in resources)
            {
                int hash = -1;
                foreach (int key in fileDictionary.Keys)
                {
                    if (fileDictionary[key] == reference.FilePath)
                    {
                        hash = key;
                        break;
                    }
                }

                //if (hash == -1) throw new Exception("Error when serializing resources!");

                NuResourceReference nuReference = new NuResourceReference()
                {
                    Type = reference.Type,
                    Hash = (uint)hash,
                    PlatformsAndClasses = reference.PlatformsAndClasses,
                    Discipline = -1
                };

                if (reference.Checksum != null)
                {
                    nuReference.Checksum = new NuCheckSum()
                    {
                        Checksum = reference.Checksum
                    };
                }

                references.Add(nuReference);
            }

            nuScene.ResourceHeader.References = references;
            nuScene.ResourceHeader.FileTree = filetree;
        }

        public static void ConvertMetadata(EditorScene scene)
        {
            GScene_4F originalScene = (GScene_4F)scene.OriginalScene;

            List<NuDynamicString> textureStrings = new List<NuDynamicString>();
            foreach (var tex in scene.Textures)
            {
                textureStrings.Add(new NuDynamicString(tex.Name));
            }

            originalScene.Metadata.MetaStrings = textureStrings;
        }
    }
}
