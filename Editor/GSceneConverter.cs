using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Rendering;
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
            editorScene.Name = Path.GetFileName(filePath);
            editorScene.SceneTransform = Matrix4.CreateScale(1f, 1f, -1f); // All meshes are flipped, so this unflips them

            Dictionary<ushort[], RenderIndicesBuffer> convertedIBuffer = new();
            foreach (var indicesBuffer in scene.indicesLists)
            {
                var uploaded = RenderIndicesBuffer.FromBuffer(indicesBuffer.Value);
                convertedIBuffer.Add(indicesBuffer.Value, uploaded);
            }

            Dictionary<VertexList, RenderVertexBuffer> convertedVBuffer = new();
            foreach (var verticesBuffer in scene.vertexLists)
            {
                var uploaded = RenderVertexBuffer.FromBuffer(verticesBuffer.Value);
                convertedVBuffer.Add(verticesBuffer.Value, uploaded);
            }

            RenderMesh[] meshes = new RenderMesh[scene.RenderMeshes.Length];
            for (int i = 0; i < scene.RenderMeshes.Length; i++)
            {
                NuRenderMesh nuMesh = scene.RenderMeshes[i];

                RenderVertexBuffer[] vBuffers = new RenderVertexBuffer[nuMesh.VertexBuffers.Length];
                for (int j = 0; j < vBuffers.Length; j++)
                {
                    vBuffers[j] = convertedVBuffer[nuMesh.VertexBuffers[j]];
                }

                RenderIndicesBuffer iBuffer = convertedIBuffer[nuMesh.Indices];

                RenderMesh mesh = new RenderMesh(vBuffers, iBuffer);
                mesh.VerticesBase = (int)nuMesh.VerticesBase;
                mesh.VerticesCount = (int)nuMesh.VerticesCount;
                mesh.IndicesBase = (int)nuMesh.IndicesBase;
                mesh.IndicesCount = (int)nuMesh.IndicesCount;

                meshes[i] = mesh;
            }

            var nxg_textures = NxgTextures.Read(Path.ChangeExtension(filePath, "nxg_textures"));
            editorScene.Textures = new List<RenderTexture>();
            if (nxg_textures != null)
            {
                for (int i = 0; i < nxg_textures.Textures.Length; i++)
                {
                    editorScene.Textures.Add(RenderTexture.FromNuTexture(nxg_textures.Textures[i]));
                }
            }

            // TODO: Just do the reference sorting here instead
            EditorMaterial[] materials = new EditorMaterial[scene.Materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                NuMaterialData nuMaterialData = scene.Materials[i];

                EditorMaterial material = new EditorMaterial();

                material.Original = nuMaterialData;

                materials[i] = material;

                editorScene.Materials.Add(material);
            }

            List<NuLightmapData> gsceneLightmaps = scene.Lightmaps;
            EditorLightmap[] lightmaps = new EditorLightmap[scene.Lightmaps.Count];
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
            Dictionary<int, EditorSceneObject> geometry = new();

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
                        Console.WriteLine(lightmapId);
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

                        EditorSceneObject obj = new EditorSceneObject();
                        obj.SetTransform(mtx);
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
                        editorScene.Objects.Add(obj);
                        break;
                }
            }

            for (int i = 0; i < display.SceneInstances.Count; i++)
            {
                var instance = display.SceneInstances[i];
                if (instance.ClipObjectIndex > -1)
                {
                    var clip = display.ClipObjects[instance.ClipObjectIndex];
                    foreach (var el in clip.Elements)
                    {
                        var sceneObject = geometry[el.GeometryIndex];
                        sceneObject.Name = $"SceneInstance_{i}";
                        var geoBounds = display.BoundsCenterAndDistSqrd[i];
                        //mesh.BoundsCenterAndDistSqrd = new Vector4(geoBounds.X, geoBounds.Y, geoBounds.Z, geoBounds.W);
                    }
                }
            }

            for (int i = 0; i < display.SpecialObjects.Count; i++)
            {
                var specialObject = display.SpecialObjects[i];
                if (specialObject.InstanceIndex != -1)
                {
                    var clip = display.ClipObjects[(int)specialObject.ClipObjectIndex];
                    foreach (var el in clip.Elements)
                    {
                        var sceneObject = geometry[el.GeometryIndex];
                        sceneObject.Name = specialObject.Name;
                    }
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
            if (index < 0)
                return RenderTexture.GetWhiteTexture();

            return textures[index];
        }
    }
}
