using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class TextureHeaders
    {
        public List<NuTextureHeader> Headers;

        public static TextureHeaders Read(RawFile file)
        {
            TextureHeaders headers = new TextureHeaders();
            
            Debug.Assert(file.ReadString(4) == "HGXT");
            Debug.Assert(file.ReadUInt(true) == 0xc);

            headers.Headers = NuSerializer.ReadVectorArray<NuTextureHeader>(file);

            return headers;
        }
    }
}
