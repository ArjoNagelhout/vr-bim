using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [JsonDerivedType(typeof(ServerEvent), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(DocumentChangedEvent), typeDiscriminator: "documentChanged")]
    [JsonDerivedType(typeof(DocumentOpenedEvent), typeDiscriminator: "documentOpened")]
    [JsonDerivedType(typeof(DocumentClosedEvent), typeDiscriminator: "documentClosed")]
    [JsonDerivedType(typeof(SelectionChangedEvent), typeDiscriminator: "selectionChangedEvent")]
    [JsonDerivedType(typeof(SendMeshDataEvent), typeDiscriminator: "sendMeshData")]
    [JsonDerivedType(typeof(StartedEditMode), typeDiscriminator: "startedEditMode")]
    [JsonDerivedType(typeof(StoppedEditMode), typeDiscriminator: "stoppedEditMode")]
    [JsonDerivedType(typeof(StartEditModeNotAllowed), typeDiscriminator: "startEditModeNotAllowed")]
    [System.Serializable]
    public class ServerEvent
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(DocumentChangedEvent), typeDiscriminator: "documentChanged")]
    public class DocumentChangedEvent : ServerEvent
    {
        public Dictionary<long, VRBIM_Element> changedElements;
        public List<long> deletedElementIds;
    }

    // send all data this document contains
    [System.Serializable]
    [JsonDerivedType(typeof(DocumentOpenedEvent), typeDiscriminator: "documentOpened")]
    public class DocumentOpenedEvent : ServerEvent
    {
        public Guid documentGuid;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(DocumentClosedEvent), typeDiscriminator: "documentClosed")]
    public class DocumentClosedEvent : ServerEvent
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(SelectionChangedEvent), typeDiscriminator: "selectionChangedEvent")]
    public class SelectionChangedEvent : ServerEvent
    {
        public List<long> selectedElementIds;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(SendMeshDataEvent), typeDiscriminator: "sendMeshData")]
    public class SendMeshDataEvent : ServerEvent
    {
        public VRBIM_MeshDataDescriptor descriptor;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(StartedEditMode), typeDiscriminator: "startedEditMode")]
    public class StartedEditMode : ServerEvent
    {
        public EditModeData populatedEditModeData;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(StartEditModeNotAllowed), typeDiscriminator: "startEditModeNotAllowed")]
    public class StartEditModeNotAllowed : ServerEvent
    {
        public string errorMessage;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(StoppedEditMode), typeDiscriminator: "stoppedEditMode")]
    public class StoppedEditMode : ServerEvent
    {
        public EditModeData stoppedEditModeData; // to validate whether the correct edit mode is stopped
    }
}
