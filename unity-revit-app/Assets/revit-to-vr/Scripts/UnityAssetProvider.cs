using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public class UnityAssetProvider : MonoBehaviour
    {
        // assets
        public Material defaultMaterial;
        
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