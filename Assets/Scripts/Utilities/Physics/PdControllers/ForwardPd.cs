
using UnityEngine;

namespace AcidRain.Utilities.Physics
{
    public class ForwardPd : IPdController
    {
        [SerializeField] private float _mkp = 90f;
        [SerializeField] private float _mkd = 40f;
        [SerializeField] private float _rkp = 1300f;
        [SerializeField] private float _rkd = 85f;

        public Vector3 GetForce(Vector3 curPosition, Vector3 desPosition, Vector3 curVelocity, Vector3 desVelocity)
        {
            return (desPosition - curPosition) * _mkp + (desVelocity - curVelocity) * _mkd;
        }

        public Vector3 GetTorque(Rigidbody rigidbody, Quaternion desRotation)
        {
            Quaternion difRotation = desRotation * Quaternion.Inverse(rigidbody.rotation);
            if (difRotation.w < 0)
            {
                // Convert the quaterion to eqivalent "short way around" quaterion
                difRotation.x = -difRotation.x;
                difRotation.y = -difRotation.y;
                difRotation.z = -difRotation.z;
                difRotation.w = -difRotation.w;
            }
            difRotation.ToAngleAxis(out float magnitude, out Vector3 axis);
            axis.Normalize();
            Vector3 desiredMove = magnitude * Mathf.Deg2Rad * axis;
            Vector3 pidv = desiredMove * _rkp - rigidbody.angularVelocity * _rkd;
            Quaternion rotInertia2World = rigidbody.inertiaTensorRotation * rigidbody.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(rigidbody.inertiaTensor);
            pidv = rotInertia2World * pidv;
            return pidv;
        }
    }
}