using AcidRain.Utilities.General;
using UnityEngine;

namespace AcidRain.Utilities.Controllers
{
    public class BackwardsMovementPd : IMovementController
    {
        [SerializeField] public float Derivative = 40f;
        [SerializeField] public float Proportional = 90f;
        [SerializeField] public Rigidbody Rigidbody;

        public Vector3 GetForce(Vector3 desPosition)
        {
            Vector3 desVelocity = Vector3.zero;

            Vector3 nextPosition = Rigidbody.position + Time.fixedDeltaTime * Rigidbody.velocity;
            Vector3 nextVelocity = Rigidbody.velocity;

            Vector3 nextDesPosition = desPosition + Time.fixedDeltaTime * desVelocity;
            Vector3 nextDesVelocity = desVelocity;

            Vector3 acceleration = Proportional * (nextDesPosition - nextPosition)
                + Derivative * (nextDesVelocity - nextVelocity);
            return Rigidbody.mass * acceleration;
        }
    }
}
