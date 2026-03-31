using Diorama.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorClipObject
    {
        public EditorSceneObject Parent;

        public List<EditorGeometryObject> Elements { get; set; } = new();

        public void Draw(Shader shader) 
        {             
            foreach (var geo in Elements)
            {
                geo.Draw(shader);
            }
        }
    }
}
