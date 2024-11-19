using UnityEngine;

namespace AcidRain.Utilities.General
{
    static class QuaternionExtensions
    {
        static public Quaternion ProjectOnPlane(this Quaternion rotation, Vector3 planeNormal)
        {
            Vector3 forward = Vector3.forward;
            Vector3 projection = Vector3.ProjectOnPlane(rotation * forward, planeNormal);
            if (projection != Vector3.zero)
            {
                return Quaternion.LookRotation(projection, planeNormal);
            }
            else
            {
                Quaternion dif = Quaternion.FromToRotation(rotation * forward, forward);
                return rotation * dif;
            }
        }

        static public void EnsureShortWayAround(this ref Quaternion rotation)
        {
            if (rotation.w < 0f)
            {
                rotation.x = -rotation.x;
                rotation.y = -rotation.y;
                rotation.z = -rotation.z;
                rotation.w = -rotation.w;
            }
        }
    }
}