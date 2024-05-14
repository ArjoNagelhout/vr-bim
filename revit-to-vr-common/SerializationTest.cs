using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace revit_to_vr_common
{
    enum EventType
    {
        A,
        B,
        C
    }

    [System.Serializable]
    public struct NestedData
    {
        int a;
        int b;
        int c;
        bool yes;
        bool no;
    }

    [System.Serializable]
    public struct TestData
    {
        EventType eventType;
        float someValue;
        string anotherValue;
        NestedData nestedData;
        NestedData nestedData2;
    }
}
