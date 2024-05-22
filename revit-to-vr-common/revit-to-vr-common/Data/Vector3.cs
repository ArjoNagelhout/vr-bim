using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_to_vr_common
{
    [System.Serializable]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ + rhs_; });
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ - rhs_; });
        }

        public static Vector3 operator *(Vector3 lhs, float rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ * rhs_; });
        }

        public static Vector3 operator /(Vector3 lhs, float rhs)
        {
            return Execute(lhs, rhs, (lhs_, rhs_) => { return lhs_ / rhs_; });
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        private delegate float F(float lhs, float rhs);

        private static Vector3 Execute(Vector3 lhs, Vector3 rhs, F func)
        {
            return new Vector3()
            {
                x = func(lhs.x, rhs.x),
                y = func(lhs.y, rhs.y),
                z = func(lhs.z, rhs.z)
            };
        }

        private static Vector3 Execute(Vector3 lhs, float rhs, F func)
        {
            return new Vector3()
            {
                x = func(lhs.x, rhs),
                y = func(lhs.y, rhs),
                z = func(lhs.z, rhs)
            };
        }
    }
}
