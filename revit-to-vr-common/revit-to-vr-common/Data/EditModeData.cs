using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [JsonDerivedType(typeof(EditModeData), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(ToposolidEditSketchEditModeData), typeDiscriminator: "toposolidEditSketch")]
    [JsonDerivedType(typeof(ToposolidModifySubElementsEditModeData), typeDiscriminator: "toposolidModifySubElements")]
    public class EditModeData
    {
        public bool isCanceled; // when the edit mode data is sent as part of the StopEditMode client event, we can see whether the client wanted to cancel or finish the modifications made in the edit mode. 
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
