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
    public class Event
    {
        
    }

    public class DocumentChangedEvent : Event
    {
        
    }

    // send all data this document contains
    public class DocumentOpenedEvent : Event
    {
        
    }

    public class DocumentClosedEvent : Event
    {
        
    }

    public class SelectionChangedEvent : Event
    {
        
    }
}
