using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    // this is the configuration data that gets sent from the client to the server
    // when the connection gets initialized
    [System.Serializable]
    public class ClientConfiguration
    {
        // default parameters:
        public int triangulationlevelOfDetail = 1;
        public VRBIM_ViewDetailLevel viewDetailLevel = VRBIM_ViewDetailLevel.Medium;
        public bool flipWindingOrder = true;
    }
}
