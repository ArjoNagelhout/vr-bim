using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    [System.Serializable]
    public class StateMaterials
    {
        public Material normal;
        public Material hovered;
        public Material selected;
    }

    public class UnityAssetProvider : MonoBehaviour
    {
        // default mesh material 
        public StateMaterials defaultMaterials;
        
        // editing of toposolid

        public StateMaterials curveMaterials; // for editing the sketch

        public StateMaterials slabShapeVertexInteriorMaterials;

        public StateMaterials slabShapeVertexEdgeAndCornerMaterials; // green

        public StateMaterials slabShapeCreaseMaterials;

        public GameObject toposolidEditSketchRendererUIPrefab;

        public GameObject toposolidModifySubElementsUIPrefab;

        // modify sub elements
        
        public GameObject slabShapeCreasePrefab;

        public GameObject slabShapeVertexPrefab;

        public GameObject elementRendererPrefab;
        
        // singleton implementation
        
        public static UnityAssetProvider instance => _instance;
        private static UnityAssetProvider _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }
            _instance = this;
        }
    }
}