
using System;

namespace AcidRain.Utilities.General
{
    public interface IState<T> where T : IState<T>
    {
        public event EventHandler<ChangeStateEventArgs> ChangeRequested;

        public void EnterState();
        public void ExitState();

        public class ChangeStateEventArgs : EventArgs
        {
            public T NextState;
        }
    }
}