using UnityEngine;

namespace AcidRain.Utilities.General
{
    public interface IMovementController
    {
        public Vector3 GetForce(Vector3 desPosition);
    }
}
