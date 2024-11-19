using AcidRain.Utilities.General;
using UnityEngine;

namespace AcidRain.Utilities.Controllers
{
    public class WikiRotationPid : IRotationController
    {
        [SerializeField] public float Derivative = 85f;
        [SerializeField] public float Integral = 3f;
        [SerializeField] public float Proportional = 1300f;
        [SerializeField] public Rigidbody Rigidbody;

        private Quaternion _lastError = Quaternion.identity;
        private Quaternion _integral = Quaternion.identity;

        public Vector3 GetTorque(Quaternion desRotation)
        {
            Quaternion error = desRotation * Quaternion.Inverse(Rigidbody.rotation);

            error.EnsureShortWayAround();

            error.ToAngleAxis(out float errorAngle, out Vector3 errorAxis);
            errorAxis.Normalize();

            _integral *= Quaternion.AngleAxis(errorAngle * Time.fixedDeltaTime, errorAxis);

            Quaternion derivative = error * Quaternion.Inverse(_lastError);
            derivative.EnsureShortWayAround();
            derivative.ToAngleAxis(out float derivativeAngle, out Vector3 derivativeAxis);

            _lastError = error;

            _integral.EnsureShortWayAround();
            _integral.ToAngleAxis(out float integralAngle, out Vector3 integralAxis);

            Vector3 acceleration = Proportional * errorAngle * errorAxis
                + Integral * integralAngle * integralAxis
                + Derivative * derivativeAngle / Time.fixedDeltaTime * derivativeAxis;

            Quaternion rotInertia2World = Rigidbody.inertiaTensorRotation * Rigidbody.rotation;

            Vector3 torque = Quaternion.Inverse(rotInertia2World) * acceleration;
            torque.Scale(Rigidbody.inertiaTensor);
            torque = rotInertia2World * torque;

            return torque;
        }
    }
}