using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class DocumentRenderer : MonoBehaviour, IMeshDataRepositoryListener
    {
        [SerializeField]
        private GameObject prefab;

        private Dictionary<VRBIM_MeshId, GameObject> _instances = new Dictionary<VRBIM_MeshId, GameObject>();

        private void Start()
        {
            MeshDataRepository.Instance.listener = this;
        }

        public void OnMeshAdded(VRBIM_MeshId meshId, Mesh mesh)
        {
            Debug.Assert(!_instances.ContainsKey(meshId));
            GameObject instance = Instantiate(prefab);
            MeshFilter meshFilter = instance.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = instance.GetComponent<MeshRenderer>(); // set materials on MeshRenderer

            instance.name += meshId.id + ", temp:";
            instance.name += meshId.temporaryId;
            
            meshFilter.mesh = mesh;
            
            _instances.Add(meshId, instance);
            
        }

        public void OnMeshDeleted(VRBIM_MeshId meshId)
        {
            Destroy(_instances[meshId]);
            _instances.Remove(meshId);
        }
    }
}
