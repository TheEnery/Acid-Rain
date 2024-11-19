using UnityEngine;

namespace AcidRain.Utilities.General
{
    public interface IRotationController
    {
        public Vector3 GetTorque(Quaternion desRotation);
    }
}