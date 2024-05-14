using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using revit_to_vr_common;
using System.Text.Json;

namespace RevitToVR
{
    public class SerializationTest : MonoBehaviour
    {
        public TestData testData;
        
        // Start is called before the first frame update
        void Start()
        {
            
            string data = JsonSerializer.Serialize(testData, Configuration.jsonSerializerOptions);
            Debug.Log("Serialized TestData to: " + data);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
