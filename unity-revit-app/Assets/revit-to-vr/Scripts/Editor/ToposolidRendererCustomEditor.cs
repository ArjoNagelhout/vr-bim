using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RevitToVR
{
    [CustomEditor(typeof(ToposolidRenderer))]
    public class ToposolidRendererCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!EditorApplication.isPlaying)
            {
                return;
            }
            
            if (GUILayout.Button("Edit Sketch"))
            {
                ToposolidRenderer toposolidRenderer = (ToposolidRenderer)target;
                toposolidRenderer.EditSketch();
            }

            if (GUILayout.Button("Modify Sub Element"))
            {
                ToposolidRenderer toposolidRenderer = (ToposolidRenderer)target;
                toposolidRenderer.ModifySubElements();
            }
        }
    }
}
