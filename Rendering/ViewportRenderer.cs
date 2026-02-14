using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class ViewportRenderer : IDioramaRenderer
    {
        float[] vertices = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };

        public int VertexBufferObject;
        public int VertexArrayObject;
        public int ElementBufferObject;

        public Shader testShader;

        public Camera Camera { get; private set; }

        List<RenderMesh> meshes = new();

        public RenderMesh Convert(NuRenderMesh geo)
        {
            float[] vertices = new float[geo.VerticesCount * 6];
            foreach (var vertexList in geo.VertexBuffers)
            {
                if (vertexList.Vertices[0].Position != System.Numerics.Vector3.Zero)
                {
                    int counter = 0;
                    for (int i = (int)geo.VerticesBase; i < geo.VerticesBase + geo.VerticesCount; i++)
                    {
                        vertices[counter] = vertexList.Vertices[i].Position.X;
                        vertices[counter + 1] = vertexList.Vertices[i].Position.Y;
                        vertices[counter + 2] = vertexList.Vertices[i].Position.Z;
                        counter += 6;
                    }
                }
                if (vertexList.Vertices[0].Normal != System.Numerics.Vector3.Zero)
                {
                    int counter = 0;
                    for (int i = (int)geo.VerticesBase; i < geo.VerticesBase + geo.VerticesCount; i++)
                    {
                        vertices[counter + 3] = vertexList.Vertices[i].Normal.X;
                        vertices[counter + 4] = vertexList.Vertices[i].Normal.Y;
                        vertices[counter + 5] = vertexList.Vertices[i].Normal.Z;
                        counter += 6;
                    }
                }

            }
            uint[] indices = new uint[geo.IndicesCount];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = geo.Indices[geo.IndicesBase + i];
            }

            RenderMesh mesh = new RenderMesh(vertices.ToArray(), indices);
            return mesh;
            //var oMtx = obj.Mtx.mtx;
            //mesh.Transform = new Matrix4(oMtx[0], oMtx[1], oMtx[2], oMtx[3], oMtx[4], oMtx[5], oMtx[6], oMtx[7], oMtx[8], oMtx[9], oMtx[10], oMtx[11], oMtx[12], oMtx[13], oMtx[14], oMtx[15]);
            //meshes.Add(mesh);
        }

        public void DebugParseFile(string filePath)
        {
            GScene parse = GScene.Parse(filePath);

            int matrixId = -1;
            int materialId = -1;
            var display = parse.DisplayScene;
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

                        RenderMesh mesh = Convert(parse.RenderMeshes[command.Index]);
                        mesh.Transform = mtx;
                        meshes.Add(mesh);
                        //mesh.Transform 

                        //MeshX mesh = meshesKeyed[command.Index];
                        //Entity ent = new Entity(new Matrix4(new Vector4(local.Row0, 0), new Vector4(local.Row1, 0), new Vector4(local.Row2, 0), new Vector4(local.Row3, 1)));
                        //ent.Mesh = mesh;
                        //ent.Material = materials[materialId];
                        //if (ent.Material.MaterialName == "_TTShaderMaterial21")
                        //{
                        //    Console.WriteLine();
                        //}
                        //mesh.Setup();
                        //entities.Add(ent);
                        //entitiesKeyed[commandId] = ent;
                        //vertOffset += mesh.VertexCount;
                        break;

                }
            }

            foreach (var obj in parse.DisplayScene.SpecialObjects)
            {
                var elements = parse.DisplayScene.ClipObjects[(int)obj.ClipObjectIndex].Elements;
                foreach (var el in elements)
                {
                    
                }
            }
        }

        private readonly Queue<string> loadScenes = new();
        public void EnqueueScene(string scene)
        {
            lock(loadScenes)
            {
                loadScenes.Enqueue(scene);
            }
        }

        public void Initialize()
        {
            //DebugParseFile(@"A:\LEVELS\STORY\11SCOOBYDOO\11SCOOBYDOOA\11SCOOBYDOOA_DX11.GSC");

            GL.ClearColor(1f, 0f, 0f, 1f);
            GL.Enable(EnableCap.DepthTest);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            testShader = new Shader("shader.vert", "shader.frag");
            Camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            testShader.Use();

            float time = (float)stopwatch.Elapsed.TotalSeconds;

            lock (loadScenes)
            {
                while (loadScenes.Count > 0)
                {
                    var scene = loadScenes.Dequeue();

                    meshes.Clear();
                    DebugParseFile(scene);
                }
            }

            //Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(time * 10));

            Matrix4 view = Camera.GetViewMatrix();

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)Width / Height, 0.1f, 1000f);

            //testShader.SetMatrix4("model", model);
            testShader.SetMatrix4("view", view);
            testShader.SetMatrix4("projection", projection);
            testShader.SetVector3("camera", Camera.Position);

            foreach (var mesh in meshes)
            {
                mesh.Draw(testShader);
            }

            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public int Width, Height;
        public void SetFramebufferSize(int width, int height)
        {
            Width = width; 
            Height = height;
        }

        public void Deinitialize()
        {
            testShader.Dispose();
        }
    }
}
