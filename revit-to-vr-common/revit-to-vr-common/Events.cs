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
    [JsonDerivedType(typeof(SendMeshDataEvent), typeDiscriminator: "sendMeshData")]
    [System.Serializable]
    public class Event
    {
        
    }

    [System.Serializable]
    [JsonDerivedType(typeof(DocumentChangedEvent), typeDiscriminator: "documentChanged")]
    public class DocumentChangedEvent : Event
    {
        public Dictionary<long, VRBIM_Element> changedElements;
        public List<long> deletedElementIds;
    }

    // send all data this document contains
    [System.Serializable]
    [JsonDerivedType(typeof(DocumentOpenedEvent), typeDiscriminator: "documentOpened")]
    public class DocumentOpenedEvent : Event
    {
        public Guid documentGuid;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(DocumentClosedEvent), typeDiscriminator: "documentClosed")]
    public class DocumentClosedEvent : Event
    {
        
    }

    [System.Serializable]
    [JsonDerivedType(typeof(SelectionChangedEvent), typeDiscriminator: "selectionChangedEvent")]
    public class SelectionChangedEvent : Event
    {
        
    }

    [System.Serializable]
    [JsonDerivedType(typeof(SendMeshDataEvent), typeDiscriminator: "sendMeshData")]
    public class SendMeshDataEvent : Event
    {
        public VRBIM_MeshDataDescriptor descriptor;
    }
}
