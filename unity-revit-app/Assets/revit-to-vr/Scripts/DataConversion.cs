using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using revit_to_vr_common;

namespace RevitToVR
{
    public static class DataConversion
    {
        public static bool FlipYAndZAxes = true;
        
        public static VRBIM_Vector3 GetVector3FromBytes(byte[] buffer, int startIndex)
        {
            return new VRBIM_Vector3()
            {
                x = BitConverter.ToSingle(buffer, startIndex + 0),
                y = BitConverter.ToSingle(buffer, startIndex + (FlipYAndZAxes ? 8 : 4)),
                z = BitConverter.ToSingle(buffer, startIndex + (FlipYAndZAxes ? 4 : 8))
            };
        }
        
        public static Vector3 ToUnityVector3(VRBIM_Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static Bounds ToUnityBounds(VRBIM_AABB bounds)
        {
            Vector3 center = ToUnityVector3(bounds.center);
            Vector3 extents = ToUnityVector3(bounds.extents);
            return new Bounds()
            {
                center = center,
                extents = extents,
                size = 2.0f * extents
            };
        }
    }
}
