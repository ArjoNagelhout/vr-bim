using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace revit_to_vr_common
{
    // common configuration parameters for the program to be used by both the VR app and the Revit plugin
    public static class Configuration
    {
        public static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        // make sure this is statically assigned in the DHCP server. 
        public static string ipAddress = "192.168.0.100";

        public static string protocolPrefix = "ws://";

        public static string uri => protocolPrefix + ipAddress;

        public static string mainPath = "/main";
    }
}
