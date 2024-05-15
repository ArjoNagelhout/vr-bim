using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    public class ChangedElement
    {
        long elementId;
        
        // send metadata about the type of element


        // send geometry

    }

    // this is the data structure that is serialized to json and sent from the Revit plugin to the Unity VR app
    // the data is sent in two parts: first this data structure, and then the binary mesh data. 
    // this binary mesh data is then combined and batched where necessary to reduce rendering overhead and excess draw calls. 
    public class RevitToVRDocumentChangedEvent
    {
        Guid documentId; // to know which document we receive an event for
        List<long> deletedElements;
        List<ChangedElement> changedElements; // either created or modified
    }
}
