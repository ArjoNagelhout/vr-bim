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

        // The level of detail. Its range is from 0 to 1. 0 is the lowest level of detail and 1 is the highest.
        public float triangulationlevelOfDetail = 1.0f;

        public VRBIM_ViewDetailLevel viewDetailLevel = VRBIM_ViewDetailLevel.Medium;

        public bool flipWindingOrder = true;
    }
}
