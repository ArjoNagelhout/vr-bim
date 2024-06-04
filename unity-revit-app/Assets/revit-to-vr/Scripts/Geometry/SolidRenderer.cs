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
            _meshCollider.sharedMesh = null;
            _meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning |
                                           MeshColliderCookingOptions.CookForFasterSimulation |
                                           MeshColliderCookingOptions.UseFastMidphase |
                                           MeshColliderCookingOptions.WeldColocatedVertices;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _documentRenderer.RegisterMeshDataEventListener(
                Utils.CreateTemporaryMeshId(solid.temporaryMeshId),
                this);
            _meshRenderer.sharedMaterial = GetNormalMaterial();
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
            _meshCollider.sharedMesh = null;
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _meshCollider.convex = false;
            Debug.Assert(_meshCollider.sharedMesh.isReadable);
            UIConsole.Log(_meshCollider.sharedMesh.name);
        }

        void IMeshDataEventListener.OnMeshRemoved()
        {
            _meshFilter.mesh = null;
            _meshCollider.sharedMesh = null;
            _meshCollider.convex = true;
        }

        public override void OnSelectHoveredStateChanged(bool hovered, bool selected)
        {
            Material material = null;
            if (selected)
            {
                material = UnityAssetProvider.instance.defaultMaterials.selected;
            }
            else if (hovered)
            {
                material = UnityAssetProvider.instance.defaultMaterials.hovered;
            }
            else
            {
                material = GetNormalMaterial();
            }

            _meshRenderer.sharedMaterial = material;
        }

        private Material GetNormalMaterial()
        {
            switch (_material)
            {
                case GeometryObjectMaterial.Generic:
                    return UnityAssetProvider.instance.geometryObjectGenericMaterial;
                case GeometryObjectMaterial.Path:
                    return UnityAssetProvider.instance.geometryObjectPathMaterial;
                case GeometryObjectMaterial.Grassland:
                    return UnityAssetProvider.instance.geometryObjectGrasslandMaterial;
                default:
                    return UnityAssetProvider.instance.defaultMaterials.normal;
            }
        }
    }
}