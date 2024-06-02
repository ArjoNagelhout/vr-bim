using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [System.Serializable]
    [JsonDerivedType(typeof(ClientEvent), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(SendClientConfigurationEvent), typeDiscriminator: "sendClientConfiguration")]
    [JsonDerivedType(typeof(StartListeningToEvents), typeDiscriminator: "startListeningToEvents")]
    [JsonDerivedType(typeof(StopListeningToEvents), typeDiscriminator: "stopListeningToEvents")]
    [JsonDerivedType(typeof(StartEditMode), typeDiscriminator: "startEditMode")]
    [JsonDerivedType(typeof(StopEditMode), typeDiscriminator: "stopEditMode")]
    [JsonDerivedType(typeof(SelectElementClientEvent), typeDiscriminator: "selectElementClientEvent")]
    [JsonDerivedType(typeof(UpdateEditMode), typeDiscriminator: "updateEditMode")]
    [JsonDerivedType(typeof(PerformSingleActionClientEvent), typeDiscriminator: "performSingleActionClientEvent")]
    public class ClientEvent
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(SendClientConfigurationEvent), typeDiscriminator: "sendClientConfiguration")]
    public class SendClientConfigurationEvent : ClientEvent
    {
        public ClientConfiguration clientConfiguration;
    }

    // gets called by the client when it wants to start listening to events
    [System.Serializable]
    [JsonDerivedType(typeof(StartListeningToEvents), typeDiscriminator: "startListeningToEvents")]
    public class StartListeningToEvents : ClientEvent
    {

    }

    // gets called by the client when it wants to stop listening to events, e.g. DocumentChangedEvents
    [System.Serializable]
    [JsonDerivedType(typeof(StopListeningToEvents), typeDiscriminator: "stopListeningToEvents")]
    public class StopListeningToEvents : ClientEvent
    {

    }

    [System.Serializable]
    [JsonDerivedType(typeof(StartEditMode), typeDiscriminator: "startEditMode")]
    public class StartEditMode : ClientEvent
    {
        public EditModeData data;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(StopEditMode), typeDiscriminator: "stopEditMode")]
    public class StopEditMode : ClientEvent
    {
        public EditModeData data; // to check whether the client tries to close the correct edit mode
    }

    [System.Serializable]
    [JsonDerivedType(typeof(UpdateEditMode), typeDiscriminator: "updateEditMode")]
    public class UpdateEditMode : ClientEvent
    {
        public UpdateEditModeData data;
    }

    public enum SingleActionType
    {
        Undo = 0,
        Redo
    }

    [System.Serializable]
    [JsonDerivedType(typeof(PerformSingleActionClientEvent), typeDiscriminator: "performSingleActionClientEvent")]
    public class PerformSingleActionClientEvent : ClientEvent
    {
        public SingleActionType actionType;
    }

    public enum SelectElementType
    {
        New, // create a new selection with the provided element ids
        Add, // add the provided element ids to the selection
        Remove // remove the provided element ids from the selection
    }

    [System.Serializable]
    [JsonDerivedType(typeof(SelectElementClientEvent), typeDiscriminator: "selectElementClientEvent")]
    public class SelectElementClientEvent : ClientEvent
    {
        public SelectElementType type;
        public List<long> selectedElementIds;
    }
}
