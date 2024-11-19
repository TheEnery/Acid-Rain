using AcidRain.Utilities.General;
using UnityEngine;

namespace AcidRain.Utilities.Controllers
{
    public class ForwardRotationPd : IRotationController
    {
        [SerializeField] public float Derivative = 85f;
        [SerializeField] public float Proportional = 1300f;
        [SerializeField] public Rigidbody Rigidbody;

        public Vector3 GetTorque(Quaternion desRotation)
        {
            Quaternion difRotation = desRotation * Quaternion.Inverse(Rigidbody.rotation);

            difRotation.EnsureShortWayAround();

            difRotation.ToAngleAxis(out float magnitude, out Vector3 axis);
            axis.Normalize();

            Vector3 rotation = magnitude * Mathf.Deg2Rad * axis;
            Vector3 velocity = -Time.fixedDeltaTime * Rigidbody.angularVelocity;

            Vector3 acceleration = rotation * Proportional + velocity * Derivative;
            
            Quaternion rotInertia2World = Rigidbody.inertiaTensorRotation * Rigidbody.rotation;

            Vector3 torque = Quaternion.Inverse(rotInertia2World) * acceleration;
            torque.Scale(Rigidbody.inertiaTensor);
            torque = rotInertia2World * torque;

            return torque;
        }
    }
}