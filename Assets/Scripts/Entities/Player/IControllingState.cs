
using System;
using UnityEngine;

namespace AcidRain.Entities.Player
{
    public partial class Controller
    {
        public interface IControllingState : Utilities.General.IState<IControllingState>
        {
            public Controller Player { get; set; }
            public bool PlayerShouldMove { get; }

            public Quaternion GetNextBodyRotation();
            public Quaternion GetNextHeadRotation();
            public Vector3 GetNextPosition();
        }

        public abstract class IControllingStateBase : IControllingState
        {
            public event EventHandler<IControllingState.ChangeStateEventArgs> ChangeRequested;

            public Controller Player { get; set; }
            public abstract bool PlayerShouldMove { get; }

            public abstract void EnterState();
            public abstract void ExitState();
            public abstract Quaternion GetNextBodyRotation();
            public abstract Quaternion GetNextHeadRotation();
            public abstract Vector3 GetNextPosition();

            protected virtual void OnChangeRequested(IControllingState.ChangeStateEventArgs e)
            {
                ChangeRequested?.Invoke(this, e);
            }
        }
    }
}
