using System;
using System.Collections.Generic;
using System.Text;
using Diorama.Editor;
using Diorama.Rendering.Shaders;
using OpenTK.Mathematics;

namespace Diorama.Rendering
{
    public class RenderContext
    {
        public Vector3 CameraScenePosition;

        public List<EditorGeometryObject> Transparent = new();

        public bool IsOpaquePass = true;

        public EditorScene Scene;

        public Shader Shader;

        public Camera Camera;

        public void Use()
        {
            Shader.SetVector3("camera", CameraScenePosition);
            Shader.SetMatrix4("view", Scene.SceneTransform * Camera.GetViewMatrix());
        }

        public RenderContext(EditorScene scene, Camera camera, Shader blendShader)
        {
            Vector3 cameraScenePos = (scene.SceneTransform * new Vector4(camera.Position, 1)).Xyz;
            CameraScenePosition = cameraScenePos;
            Shader = blendShader;
            Camera = camera;
            Scene = scene;
        }
    }
}
