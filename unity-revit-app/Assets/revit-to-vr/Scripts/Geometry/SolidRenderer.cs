using System;
using System.Collections;
using System.Collections.Generic;
using revit_to_vr_common;
using UnityEngine;

namespace RevitToVR
{
    public class SolidRenderer : GeometryObjectRenderer, IMeshDataEventListener
    {
        private VRBIM_Solid solid => _geometry as VRBIM_Solid;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        protected void Awake()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = UnityAssetProvider.instance.defaultMaterials.normal;
            _meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _documentRenderer.RegisterMeshDataEventListener(
                Utils.CreateTemporaryMeshId(solid.temporaryMeshId),
                this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _documentRenderer.UnregisterMeshDataEventListener(
                Utils.CreateTemporaryMeshId(solid.temporaryMeshId),
                this);
        }

        // IMeshDataEventListener

        void IMeshDataEventListener.OnMeshAdded(Mesh mesh)
        {
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _meshCollider.convex = true;
        }

        void IMeshDataEventListener.OnMeshRemoved()
        {
            _meshFilter.mesh = null;
            _meshCollider.sharedMesh = null;
            _meshCollider.convex = false;
        }
        
        // ISelectionChangedListener

        public override void OnSelect()
        {
            base.OnSelect();
            _meshRenderer.material = UnityAssetProvider.instance.defaultMaterials.selected;
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            _meshRenderer.material = UnityAssetProvider.instance.defaultMaterials.normal;
        }
    }
}