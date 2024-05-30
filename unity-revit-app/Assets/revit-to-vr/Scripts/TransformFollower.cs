using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    // stupid hack we need because the controllers get disabled when opening the keyboard,
    // thus also disabling the input field, which makes sure the input field doesn't receive any input anymore....
    // does not set the scale
    public class TransformFollower : MonoBehaviour
    {
        [SerializeField] private Transform transformToFollow;

        private Matrix4x4 startTransform;

        private void Start()
        {
            startTransform = transform.localToWorldMatrix;
        }

        private void LateUpdate()
        {
            Matrix4x4 a = transformToFollow.localToWorldMatrix;

            Matrix4x4 result = a * startTransform;
            transform.SetLocalPositionAndRotation(result.GetPosition(), result.rotation);
        }
    }
}
