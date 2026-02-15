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

            EditorMaterial[] materials = new EditorMaterial[scene.Materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                NuMaterialData nuMaterialData = scene.Materials[i];

                EditorMaterial material = new EditorMaterial();

                material.Original = nuMaterialData;

                materials[i] = material;

                editorScene.Materials.Add(material);
            }

            var display = scene.DisplayScene;
            int matrixId = -1;
            int materialId = -1;
            Dictionary<int, EditorSceneObject> geometry = new();

            for (int commandId = 0; commandId < display.DisplayItems.Count; commandId++)
            {
                NuDefunctDisplayItem command = display.DisplayItems[commandId];
                switch (command.Command)
                {
                    case DisplayCommand.Material:
                        materialId = (int)command.Index;
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
                        obj.Transform = mtx;
                        obj.Mesh = mesh;
                        obj.Material = materials[materialId];

                        //Meshes.Add(mesh);
                        geometry.Add(commandId, obj);
                        editorScene.Objects.Add(obj);
                        break;
                }
            }

            //for (int i = 0; i < display.SceneInstances.Count; i++)
            //{
            //    var instance = display.SceneInstances[i];
            //    if (instance.ClipObjectIndex > -1)
            //    {
            //        var clip = display.ClipObjects[instance.ClipObjectIndex];
            //        foreach (var el in clip.Elements)
            //        {
            //            var mesh = geometry[el.GeometryIndex];
            //            var geoBounds = display.BoundsCenterAndDistSqrd[i];
            //            mesh.BoundsCenterAndDistSqrd = new Vector4(geoBounds.X, geoBounds.Y, geoBounds.Z, geoBounds.W);
            //        }
            //    }
            //}


            var nxg_textures = NxgTextures.Read(Path.ChangeExtension(filePath, "nxg_textures"));
            editorScene.Textures = new List<RenderTexture>();
            if (nxg_textures != null)
            {
                for (int i = 0; i < nxg_textures.Textures.Length; i++)
                {
                    editorScene.Textures.Add(RenderTexture.FromNuTexture(nxg_textures.Textures[i]));
                }
            }

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];

                mat.Diffuse0 = ResolveTexture(editorScene.Textures, mat.Original.Diffuse0Index);
                mat.Diffuse1 = ResolveTexture(editorScene.Textures, mat.Original.Diffuse1Index);
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
