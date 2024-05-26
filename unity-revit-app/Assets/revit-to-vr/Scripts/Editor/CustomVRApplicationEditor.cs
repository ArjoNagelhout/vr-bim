using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RevitToVR
{
    [CustomEditor(typeof(VRApplication))]
    public class CustomVRApplicationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Resend configuration data"))
            {
                if (EditorApplication.isPlaying)
                {
                    VRApplication app = (VRApplication)target;
                    app.SendConfigurationDataAndStartListening();                    
                }
            }
        }
    }
}
