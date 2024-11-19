using AcidRain.Utilities.General;
using UnityEngine;

namespace AcidRain.Utilities.Controllers
{
    public class BackwardsRotationPd : IRotationController
    {
        [SerializeField] public float Derivative = 85f;
        [SerializeField] public float Proportional = 1300f;
        [SerializeField] public Rigidbody Rigidbody;

        public Vector3 GetTorque(Quaternion desRotation)
        {
            Vector3 desVelocity = Vector3.zero;
            Vector3 curVelocity = Rigidbody.angularVelocity;
            Quaternion curRotation = Rigidbody.rotation;

            Quaternion nextDesRotation;
            float desVelocityAngle = desVelocity.magnitude;
            if (desVelocityAngle < 10e-8f)
            {
                nextDesRotation = desRotation;
            }
            else
            {
                Quaternion desDelta = Quaternion.AngleAxis(
                    desVelocityAngle * Time.fixedDeltaTime * Mathf.Rad2Deg, 
                    desVelocity / desVelocityAngle);
                nextDesRotation = desDelta * desRotation;
            }
            
            Quaternion nextRotation;
            float curVelocityAngle = curVelocity.magnitude;
            if (curVelocityAngle < 10e-8f)
            {
                nextRotation = curRotation;
            }
            else
            {
                Quaternion curDelta = Quaternion.AngleAxis(
                    curVelocityAngle * Time.fixedDeltaTime * Mathf.Rad2Deg, 
                    curVelocity / curVelocityAngle);
                nextRotation = curDelta * curRotation;
            }

            Quaternion delta = nextRotation * Quaternion.Inverse(nextDesRotation);
            delta.EnsureShortWayAround();
            delta.ToAngleAxis(out float deltaAngle, out Vector3 deltaDirection);
            deltaDirection.Normalize();
            deltaAngle *= Mathf.Deg2Rad;
            
            Vector3 acceleration = Proportional * deltaAngle  * deltaDirection
                + Derivative * (desVelocity - curVelocity);

            Quaternion rotInertia2World = Rigidbody.inertiaTensorRotation * Rigidbody.rotation;

            Vector3 torque = Quaternion.Inverse(rotInertia2World) * acceleration;
            torque.Scale(Rigidbody.inertiaTensor);
            torque = rotInertia2World * torque;

            return torque;
        }
    }
}