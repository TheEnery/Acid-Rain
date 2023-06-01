
using System;

namespace AcidRain.Entities.Drone
{
    public interface IConnector
    {
        public event EventHandler<EventArgs> AllDronesDischarged;

        public bool Empty { get; }
        public bool Full { get; }

        public void Connect(Controller drone);
        public void Disconnect(Controller drone);
    }
}