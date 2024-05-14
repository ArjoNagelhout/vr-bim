using System;
using System.Text.Json;

namespace revit_to_vr_common
{

    public enum EventType
    {
        A,
        B,
        C
    }

    [System.Serializable]
    public class NestedData
    {
        public int a;
        public int b;
        public int c;
        public bool yes;
        public bool no;
    }

    [System.Serializable]
    public class TestData
    {
        public EventType eventType;
        public float someValue;
        public string anotherValue;
        public NestedData nestedData = new NestedData();
        public NestedData nestedData2 = new NestedData();
    }
}
