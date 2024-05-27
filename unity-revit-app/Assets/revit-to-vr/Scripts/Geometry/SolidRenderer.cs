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

        protected void Awake()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = UnityAssetProvider.instance.defaultMaterial;
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
            _meshFilter.mesh = mesh;
        }

        void IMeshDataEventListener.OnMeshRemoved()
        {
            _meshFilter.mesh = null;
        }
        
        // ISelectionChangedListener

        public override void OnSelect()
        {
            base.OnSelect();
            _meshRenderer.material = UnityAssetProvider.instance.defaultSelectedMaterial;
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            _meshRenderer.material = UnityAssetProvider.instance.defaultMaterial;
        }
    }
}