using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace revit_to_vr_common
{
    [JsonDerivedType(typeof(Event), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(DocumentChangedEvent), typeDiscriminator: "documentChanged")]
    [JsonDerivedType(typeof(DocumentOpenedEvent), typeDiscriminator: "documentOpened")]
    [JsonDerivedType(typeof(DocumentClosedEvent), typeDiscriminator: "documentClosed")]
    [JsonDerivedType(typeof(SelectionChangedEvent), typeDiscriminator: "selectionChangedEvent")]
    [System.Serializable]
    public class Event
    {
        
    }

    [JsonDerivedType(typeof(DocumentChangedEvent), typeDiscriminator: "documentChanged")]
    [System.Serializable]
    public class DocumentChangedEvent : Event
    {
        public Dictionary<long, VRBIM_Element> changedElements;
        public List<long> deletedElementIds;
    }

    // send all data this document contains
    [JsonDerivedType(typeof(DocumentOpenedEvent), typeDiscriminator: "documentOpened")]
    [System.Serializable]
    public class DocumentOpenedEvent : Event
    {
        
    }

    [JsonDerivedType(typeof(DocumentClosedEvent), typeDiscriminator: "documentClosed")]
    [System.Serializable]
    public class DocumentClosedEvent : Event
    {
        
    }

    [JsonDerivedType(typeof(SelectionChangedEvent), typeDiscriminator: "selectionChangedEvent")]
    [System.Serializable]
    public class SelectionChangedEvent : Event
    {
        
    }

    [JsonDerivedType(typeof(SendMeshDataEvent), typeDiscriminator: "sendMeshData")]
    [System.Serializable]
    public class SendMeshDataEvent : Event
    {
        public VRBIM_MeshDataDescriptor descriptor;
    }
}
