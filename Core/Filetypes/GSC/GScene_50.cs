using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC
{
    public class GScene_50 : GScene_4F
    {
        protected override void ReadSplines()
        {
            List<NuSpline> splines = NuSerializer.ReadVectorArray<NuSpline_50>(file).Cast<NuSpline>().ToList();
        }
    }
}
