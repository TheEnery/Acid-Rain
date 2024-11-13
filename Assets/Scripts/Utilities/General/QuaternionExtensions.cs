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
    }
}