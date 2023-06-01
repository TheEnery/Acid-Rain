
using UnityEngine;

namespace AcidRain.Utilities.Physics
{
    public class BackwardsPd : IPdController
    {
        public Vector3 GetForce(Vector3 curPosition, Vector3 desPosition, Vector3 curVelocity, Vector3 desVelocity)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetTorque(Rigidbody rigidbody, Quaternion desRotation)
        {
            throw new System.NotImplementedException();
        }
    }
}