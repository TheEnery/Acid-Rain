using AcidRain.Utilities.General;
using UnityEngine;

namespace AcidRain.Utilities.Controllers
{
    // This controller calculates the force based on a formula Fmax * l = (m * vmax ^ 2) / 2,
    // where Fmax is the maximal force that the body can apply, l is the remaining distance,
    // and vmax is the maximal velocity for this moment.
    // This approach ensures the body can slow down after reaching the braking distance
    // using the maximal force and achieve the desired position as fast as possible.
    // The maximal force in calculation subtracts gravity from [MaxForce]
    // for the ability to resist it when decelerating free falling.
    // It would be better not to rely on this (note for V2).
    // After reaching the target, it starts to oscillate (as far as it constantly gets the target),
    // so [Presicion] is added to make it smoother (another note for V2).
    public class StableMovementControllerV1 : IMovementController
    {
        [SerializeField] public float Mass = 2f;
        [SerializeField] public float MaxForce = 300f;
        [SerializeField] public float Precision = 1f;
        [SerializeField] public Rigidbody Rigidbody;

        public Vector3 GetForce(Vector3 desPosition)
        {
            Vector3 nextPosition = Rigidbody.position + Time.fixedDeltaTime * Rigidbody.velocity;
            float distance = (desPosition - nextPosition).magnitude;
            Vector3 desVelocityDirection = (desPosition - nextPosition).normalized;
            float maxForce = MaxForce - UnityEngine.Physics.gravity.magnitude * Mass;

            if (distance < Precision) maxForce *= distance / Precision;

            float desVelocityMag = Mathf.Sqrt(2f * maxForce * distance / Mass);
            Vector3 desVelocity = desVelocityMag * desVelocityDirection;
            Vector3 force = Mass / Time.fixedDeltaTime * (desVelocity - Rigidbody.velocity);

            return Vector3.ClampMagnitude(force, MaxForce);
        }
    }
}

