using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    public class Utils
    {
        public static VRBIM_MeshId CreateTemporaryMeshId(Guid temporaryId)
        {
            return new VRBIM_MeshId()
            {
                id = Configuration.temporaryMeshIndex,
                temporaryId = temporaryId
            };
        }
    }
}
