using AcidRain.Utilities.General;
using UnityEngine;

namespace AcidRain.Utilities.Controllers
{
    public class ForwardMovementPd : IMovementController
    {
        [SerializeField] public float Derivative = 40f;
        [SerializeField] public float Proportional = 90f;
        [SerializeField] public Rigidbody Rigidbody;

        public Vector3 GetForce(Vector3 desPosition)
        {
            Vector3 acceleration = (desPosition - Rigidbody.position) * Proportional 
                - Rigidbody.velocity * Derivative;
            return Rigidbody.mass * acceleration;
        }
    }
}