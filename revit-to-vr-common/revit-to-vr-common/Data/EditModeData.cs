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
        public long toposolidId;
        public VRBIM_SlabShapeData slabShapeData;
    }

    // update from client:

    [JsonDerivedType(typeof(UpdateEditModeData), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(UpdateModifySubElements), typeDiscriminator: "updateModifySubElements")]
    public class UpdateEditModeData
    {

    }

    [JsonDerivedType(typeof(UpdateModifySubElements), typeDiscriminator: "updateModifySubElements")]
    public class UpdateModifySubElements : UpdateEditModeData
    {
        public class Entry
        {
            public int index; // index of vertex
            public float offset; // offset
        }

        public List<Entry> entries;
    }
}
