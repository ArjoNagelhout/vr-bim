using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Reflection;
using UnityEngine.Serialization;

namespace RevitToVR
{
    public class LocomotionSystemFix : MonoBehaviour
    {
        private static LocomotionSystemFix _instance;
        public static LocomotionSystemFix instance => _instance;

        [SerializeField] private LocomotionSystem locomotionSystem;

        private FieldInfo _cachedFieldInfo;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }

            _instance = this;
            SetCachedFieldInfo();
        }

        private void SetCachedFieldInfo()
        {
            if (_cachedFieldInfo != null)
            {
                return;
            }

            _cachedFieldInfo = typeof(LocomotionSystem).GetField("m_CurrentExclusiveProvider",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(_cachedFieldInfo != null);
        }

        public LocomotionProvider GetCurrentExclusiveProvider()
        {
            SetCachedFieldInfo();
            Debug.Assert(_cachedFieldInfo != null);
            Debug.Assert(locomotionSystem != null);
            return (LocomotionProvider)_cachedFieldInfo.GetValue(locomotionSystem);
        }
    }
}