
using System;
using UnityEngine;

namespace AcidRain.Entities.Drone
{
    public partial class Controller : MonoBehaviour
    {
        public interface IControllingState : Utilities.General.IState<IControllingState>
        {
            public Controller Drone { get; set; }
            public bool IsDefault { get; }
            public bool NeedAttaching { get; }

            public IControllingState Clone();
            public Vector3 GetNextPosition();
            public Quaternion GetNextRotation();
        }

        public abstract class ControllingStateBase : IControllingState
        {
            public event EventHandler<IControllingState.ChangeStateEventArgs> ChangeRequested;

            public Controller Drone { get; set; }
            public abstract bool IsDefault { get; }
            public abstract bool NeedAttaching { get; }

            public virtual IControllingState Clone()
            {
                IControllingState clone = (IControllingState)MemberwiseClone();
                clone.Drone = null;
                clone.ChangeRequested -= ChangeRequested;
                return clone;
            }
            public abstract void EnterState();
            public abstract void ExitState();
            public abstract Vector3 GetNextPosition();
            public abstract Quaternion GetNextRotation();

            protected virtual void OnChangeRequested(IControllingState.ChangeStateEventArgs e)
            {
                ChangeRequested?.Invoke(this, e);
            }
        }

        public abstract class PlayerSharedState : ControllingStateBase // this one is here temporaryly
        {
            public PlayerConnector Connector { get => (PlayerConnector)Drone.Connector; }
        }
    }
}