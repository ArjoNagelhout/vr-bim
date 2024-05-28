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
        public EditMode data;
    }

    [System.Serializable]
    [JsonDerivedType(typeof(StopEditMode), typeDiscriminator: "stopEditMode")]
    public class StopEditMode : ClientEvent
    {
        public EditMode data; // to check whether the client tries to close the correct edit mode
    }
}
