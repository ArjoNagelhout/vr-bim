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

        public StateMaterials slabShapeVertexMaterials;

        public StateMaterials slabShapeCreaseMaterials;
        
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