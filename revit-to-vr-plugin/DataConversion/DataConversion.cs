using Autodesk.Revit.DB;
using revit_to_vr_common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace revit_to_vr_plugin
{
    public static partial class DataConversion
    {
        // converts doubles to floats, determine whether this is an issue for precision
        // in large models
        public static VRBIM_Vector3 ConvertXYZ(XYZ xyz)
        {
            return new VRBIM_Vector3()
            {
                x = (float)xyz.X,
                y = (float)xyz.Y,
                z = (float)xyz.Z
            };
        }

        public static byte[] GetBytes(XYZ xyz)
        {
            byte[] data = new byte[3 * 4];
            byte[] x = BitConverter.GetBytes((float)xyz.X);
            byte[] y = BitConverter.GetBytes((float)xyz.Y);
            byte[] z = BitConverter.GetBytes((float)xyz.Z);

            Buffer.BlockCopy(x, 0, data, 0, 4);
            Buffer.BlockCopy(y, 0, data, 4, 4);
            Buffer.BlockCopy(z, 0, data, 8, 4);

            return data;
        }

        public static ViewDetailLevel ConvertViewDetailLevel(VRBIM_ViewDetailLevel level)
        {
            switch (level)
            {
                case VRBIM_ViewDetailLevel.Coarse:
                    return ViewDetailLevel.Coarse;
                case VRBIM_ViewDetailLevel.Medium:
                    return ViewDetailLevel.Medium;
                case VRBIM_ViewDetailLevel.Fine:
                    return ViewDetailLevel.Fine;
            }
            return ViewDetailLevel.Undefined;
        }

        public static VRBIM_AABB ConvertBoundingBox(BoundingBoxXYZ bounds)
        {
            VRBIM_Vector3 min = ConvertXYZ(bounds.Min);
            VRBIM_Vector3 max = ConvertXYZ(bounds.Max);

            VRBIM_Vector3 center = (min + max) / 2.0f;
            VRBIM_Vector3 extents = (max - min) / 2.0f;

            return new VRBIM_AABB()
            {
                center = center,
                extents = extents
            };
        }

        

        public static bool XYZIsEqual(XYZ lhs, XYZ rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
        }

        public static VRBIM_LocationPoint ConvertLocationPoint(LocationPoint locationPoint)
        {
            VRBIM_LocationPoint output = new VRBIM_LocationPoint();

            try
            {
                output.point = ConvertXYZ(locationPoint.Point);
                output.rotation = (float)locationPoint.Rotation;
            }
            catch
            {
                return output;
            }

            return output;
        }

        public static VRBIM_LocationCurve ConvertLocationCurve(LocationCurve locationCurve)
        {
            return new VRBIM_LocationCurve()
            {
                curve = ConvertCurve(locationCurve.Curve)
            };
        }

        public static VRBIM_Location ConvertLocation(Location location)
        {
            try
            {
                switch (location)
                {
                    case LocationPoint locationPoint:
                        return ConvertLocationPoint(locationPoint);

                    case LocationCurve locationCurve:
                        return ConvertLocationCurve(locationCurve);
                }
            }
            catch
            {
                return null;
            }
            
            //Debug.Assert(false); auto join tracker element can have no location for example
            return null;
        }

        
    }
}
