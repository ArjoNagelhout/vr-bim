using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace revit_to_vr_common
{
    public static class Configuration
    {
        public static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };
    }
}
