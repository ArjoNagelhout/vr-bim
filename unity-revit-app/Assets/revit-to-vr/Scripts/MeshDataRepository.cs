using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class MeshDataRepository : MonoBehaviour
    {
        public static MeshDataRepository Instance => _instance;
        public static MeshDataRepository _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }
            _instance = this;
        }

        public Dictionary<Guid, UnityEngine.Mesh> Meshes = new Dictionary<Guid, UnityEngine.Mesh>();
    }
}
