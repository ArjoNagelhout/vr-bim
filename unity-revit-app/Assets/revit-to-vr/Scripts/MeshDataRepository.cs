using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RevitToVR
{
    public interface IMeshDataRepositoryListener
    {
        public void OnMeshAdded(VRBIM_MeshId meshId, UnityEngine.Mesh mesh);

        public void OnMeshDeleted(VRBIM_MeshId meshId);
    }
    
    public class MeshDataRepository : MonoBehaviour
    {
        public static MeshDataRepository Instance => _instance;
        public static MeshDataRepository _instance;

        public IMeshDataRepositoryListener listener;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }
            _instance = this;
        }

        public Dictionary<VRBIM_MeshId, UnityEngine.Mesh> Meshes = new Dictionary<VRBIM_MeshId, UnityEngine.Mesh>();

        public void AddMesh(VRBIM_MeshId meshId, UnityEngine.Mesh mesh)
        {
            if (Meshes.ContainsKey(meshId))
            {
                Meshes.Remove(meshId);
                listener?.OnMeshDeleted(meshId);
            }
            
            Meshes.Add(meshId, mesh);
            listener?.OnMeshAdded(meshId, mesh);
        }
    }
}
