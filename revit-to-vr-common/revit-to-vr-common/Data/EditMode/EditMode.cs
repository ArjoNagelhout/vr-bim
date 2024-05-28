using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [JsonDerivedType(typeof(EditMode), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(EditMode), typeDiscriminator: "toposolidEditSketch")]
    [JsonDerivedType(typeof(EditMode), typeDiscriminator: "toposolidModifySubElements")]
    public abstract class EditMode
    {
        public abstract void StartEditMode();

        public abstract void StopEditMode();
    }

    [JsonDerivedType(typeof(EditMode), typeDiscriminator: "toposolidEditSketch")]
    public class ToposolidEditSketchEditMode : EditMode
    {
        public long toposolidId;

        public override void StartEditMode()
        {
            
        }

        public override void StopEditMode()
        {
            
        }
    }

    [JsonDerivedType(typeof(EditMode), typeDiscriminator: "toposolidModifySubElements")]
    public class ToposolidModifySubElementsEditMode : EditMode
    {
        public override void StartEditMode()
        {
            
        }

        public override void StopEditMode()
        {
            
        }
    }
}
