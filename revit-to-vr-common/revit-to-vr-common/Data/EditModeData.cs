using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [JsonDerivedType(typeof(EditModeData), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(EditModeData), typeDiscriminator: "toposolidEditSketch")]
    [JsonDerivedType(typeof(EditModeData), typeDiscriminator: "toposolidModifySubElements")]
    public class EditModeData
    {

    }

    [JsonDerivedType(typeof(ToposolidEditSketchEditModeData), typeDiscriminator: "toposolidEditSketch")]
    public class ToposolidEditSketchEditModeData : EditModeData
    {
        public long toposolidId;
    }

    [JsonDerivedType(typeof(ToposolidModifySubElementsEditModeData), typeDiscriminator: "toposolidModifySubElements")]
    public class ToposolidModifySubElementsEditModeData : EditModeData
    {
        
    }
}
