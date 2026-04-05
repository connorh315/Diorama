
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
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
                        convertedVBuffer.Add(buffer, RenderVertexBuffer.FromBuffer(buffer));
                    }

                    vBuffers[j] = convertedVBuffer[buffer];
                }

                var indices = nuMesh.Indices;
                if (!convertedIBuffer.ContainsKey(indices))
                {
                    convertedIBuffer.Add(indices, RenderIndicesBuffer.FromBuffer(indices));
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

            editorScene.Textures = new List<RenderTexture>();

            try
            {
                var nxg_textures = NxgTextures.Read(Path.ChangeExtension(filePath, "nxg_textures"));
                if (nxg_textures != null)
                {
                    for (int i = 0; i < nxg_textures.Textures.Length; i++)
                    {
                        editorScene.Textures.Add(RenderTexture.FromNuTexture(nxg_textures.Textures[i]));
                    }
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

                lightmap.AmbientOcclusion = ResolveTexture(editorScene.Textures, nuLightmap.AoTID);
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

                            Matrix4 mtx = new Matrix4(local.Mtx[0], local.Mtx[1], local.Mtx[2], 0, local.Mtx[3], local.Mtx[4], local.Mtx[5], 0, local.Mtx[6], local.Mtx[7], local.Mtx[8], 0, local.Mtx[9], local.Mtx[10], local.Mtx[11], 1);

                            RenderMesh mesh = meshes[command.Index];

                            EditorGeometryObject obj = new EditorGeometryObject();
                            obj.Transform = mtx;
                            obj.Mesh = mesh;
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
                        var geo = geometry[el.OldGeometryIndex];
                        clip.Elements.Add(geo);
                        geo.Parent = clip; // TODO: Remove
                        geo.Material = materials[el.OldMaterialIndex];
                    }
                    allClipObjects.Add(clip);
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
                        Matrix4 mtx = new Matrix4(local.Mtx[0], local.Mtx[1], local.Mtx[2], 0, local.Mtx[3], local.Mtx[4], local.Mtx[5], 0, local.Mtx[6], local.Mtx[7], local.Mtx[8], 0, local.Mtx[9], local.Mtx[10], local.Mtx[11], 1);
                        RenderMesh mesh = meshes[el.MeshIndex];
                        EditorGeometryObject obj = new EditorGeometryObject();
                        obj.Transform = mtx;
                        obj.Mesh = mesh;
                        if (el.MaterialIndex > -1)
                        {
                            obj.Material = materials[el.MaterialIndex];
                        }
                        if (el.LightmapIndex > -1)
                        {
                            obj.Lightmap = lightmaps[el.LightmapIndex];
                        }

                        clip.Elements.Add(obj);
                        obj.Parent = clip;
                        //geometry.Add(i, obj);
                    }
                    allClipObjects.Add(clip);
                }
            }


            for (int i = 0; i < display.SceneInstances.Count; i++)
            {
                var instance = display.SceneInstances[i];
                EditorSceneObject sceneObject = new EditorSceneObject();
                editorScene.Objects.Add(sceneObject);
                sceneObject.Name = $"SceneInstance_{i}";
                sceneObject.FadeDistances = instance.FadeDistances; // TODO: probably dangerous?

                var geoBounds = display.BoundsCenterAndDistSqrd[i];
                sceneObject.BoundsCenterAndDistSqrd = new Vector4(geoBounds.X, geoBounds.Y, geoBounds.Z, geoBounds.W);

                if (instance.ClipObjectIndex > -1)
                {
                    sceneObject.ClipObject = allClipObjects[instance.ClipObjectIndex];
                    sceneObject.ClipObject.Parent = sceneObject;
                }

                if (instance.Lods != null && instance.ClipObjectIndex != -1)
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

                mat.Diffuse0 = ResolveTexture(editorScene.Textures, mat.Original.Diffuse0Index);
                mat.Diffuse1 = ResolveTexture(editorScene.Textures, mat.Original.Diffuse1Index);

                mat.Normal0 = ResolveTexture(editorScene.Textures, mat.Original.Normal0Index);

                mat.LightmapUVSet = mat.Original.LightmapUVSet;

                uint abgr = (uint)mat.Original.Colour1;
                float a = ((abgr >> 24) & 0xFF) / 255f;
                float b = ((abgr >> 16) & 0xFF) / 255f;
                float g = ((abgr >> 8) & 0xFF) / 255f;
                float r = ((abgr >> 0) & 0xFF) / 255f;
                mat.Colour1 = new Vector4(r, g, b, a);
            }

            return editorScene;
        }

        static RenderTexture ResolveTexture(List<RenderTexture> textures, int index)
        {
            if (index < 0 || textures.Count <= (index))
                return RenderTexture.GetWhiteTexture();

            return textures[index];
        }
    }
}
