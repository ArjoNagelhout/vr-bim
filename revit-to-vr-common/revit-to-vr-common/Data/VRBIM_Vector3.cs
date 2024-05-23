using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(
        System.Runtime.InteropServices.LayoutKind.Sequential,
        Pack = 4)] // size in bytes = 12 bytes (3 * 4)
    public struct VRBIM_Vector3
    {
        public float x; // 4 bytes
        public float y; // 4 bytes
        public float z; // 4 bytes

        public static VRBIM_Vector3 operator +(VRBIM_Vector3 lhs, VRBIM_Vector3 rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ + rhs_; });
        }

        public static VRBIM_Vector3 operator -(VRBIM_Vector3 lhs, VRBIM_Vector3 rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ - rhs_; });
        }

        public static VRBIM_Vector3 operator *(VRBIM_Vector3 lhs, float rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ * rhs_; });
        }

        public static VRBIM_Vector3 operator /(VRBIM_Vector3 lhs, float rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ / rhs_; });
        }

        public static bool operator ==(VRBIM_Vector3 lhs, VRBIM_Vector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(VRBIM_Vector3 lhs, VRBIM_Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        private delegate float F(float lhs, float rhs);

        private static VRBIM_Vector3 Execute(VRBIM_Vector3 lhs, VRBIM_Vector3 rhs, F func)
        {
            return new VRBIM_Vector3()
            {
                x = func(lhs.x, rhs.x),
                y = func(lhs.y, rhs.y),
                z = func(lhs.z, rhs.z)
            };
        }

        private static VRBIM_Vector3 Execute(VRBIM_Vector3 lhs, float rhs, F func)
        {
            return new VRBIM_Vector3()
            {
                x = func(lhs.x, rhs),
                y = func(lhs.y, rhs),
                z = func(lhs.z, rhs)
            };
        }
    }
}
