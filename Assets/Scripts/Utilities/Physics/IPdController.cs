
using UnityEngine;

namespace AcidRain.Utilities.Physics
{
    public interface IPdController
    {
        public Vector3 GetForce(Vector3 curPosition, Vector3 desPosition, Vector3 curVelocity, Vector3 desVelocity);
        public Vector3 GetTorque(Rigidbody rigidbody, Quaternion desRotation);
    }
}