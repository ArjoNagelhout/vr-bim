using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class SolidRenderer : GeometryObjectRenderer
    {
        private VRBIM_Solid solid => _geometry as VRBIM_Solid;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private void Start()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = UnityAssetProvider.instance.defaultMaterial;
        }

        protected override void OnMeshAdded(VRBIM_MeshId meshId, Mesh mesh)
        {
            if (meshId.temporaryId == solid.temporaryMeshId)
            {
                _meshFilter.mesh = mesh;
            }
        }
    }
}
