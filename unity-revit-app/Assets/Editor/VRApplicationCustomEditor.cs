using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RevitToVR
{
    [CustomEditor(typeof(VRApplication))]
    public class VRApplicationCustomEditor : Editor
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
