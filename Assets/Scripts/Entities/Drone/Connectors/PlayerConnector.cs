
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AcidRain.Entities.Drone
{
    public class DroneReplacementEventArgs : EventArgs
    {
        private Controller.IControllingState _desiredState;

        public Controller.IControllingState DesiredState
        {
            get { return _desiredState.Clone(); }
            private set { _desiredState = value; }
        }

        public DroneReplacementEventArgs(Controller.IControllingState desiredState)
        {
            _desiredState = desiredState;
        }
    }

    public class PlayerConnector : MonoBehaviour, IConnector
    {
        private List<Slot> _slots = new List<Slot>();
        private Rigidbody _rigidbodyForAttaching;

        public event EventHandler<EventArgs> AllDronesDischarged;
        public event EventHandler<DroneReplacementEventArgs> DroneReplacementRequired;
        public event EventHandler<EventArgs> FpvToTpvSwitching;
        public event EventHandler<EventArgs> TpvToSpectatorSwitching;

        public virtual int Capacity { get => 2; }
        public int DroneCount { get; private set; } = 0;
        public bool Empty { get => DroneCount == 0; }
        public bool Full { get => DroneCount == Capacity; }
        public bool HaveChargedDrone
        {
            get
            {
                return _slots.Any((slot) => slot.Drone != null && slot.Drone.IsDischarged == false);
            }
        }
        public Controller NextActiveDrone
        {
            get
            {
                if (Empty || !HaveChargedDrone)
                {
                    return null;
                }
                if ((_slots[0].Drone.InDefaultState || _slots[0].Drone.IsDischarged) && DroneCount > 1)
                {
                    _slots.Sort((s1, s2) => s2.Drone.Charge.CompareTo(s1.Drone.Charge));
                }
                return _slots[0].Drone;
            }
        }
        public Player.Controller Player { get; private set; }

        public static PlayerConnector Create(Player.Controller player, Rigidbody playerRigidbody)
        {
            GameObject gameObject = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerConnector"));
            PlayerConnector connector = gameObject.GetComponent<PlayerConnector>();

            connector.Player = player;
            connector._rigidbodyForAttaching = playerRigidbody;

            connector.InitializeSlots();

            return connector;
        }

        public void Connect(Controller drone)
        {
            if (Full)
            {
                Debug.Log("ConnectDrone error");
                return;
            }

            DroneCount++;
            Slot slot = _slots.Find((slot) => slot.Drone == null);
            slot.Drone = drone;
            drone.ConnectTo(this, _rigidbodyForAttaching, new Controller.FpvMode(), new Utilities.Physics.ForwardPd());
            drone.Discharged += PreventDroneDisabling;
        }

        public void Disconnect(Controller drone)
        {
            Slot slot = _slots.Find((slot) => slot.Drone == drone);
            if (Empty || slot == null)
            {
                Debug.Log("DisconnectDrone error");
                return;
            }

            DroneCount--;
            slot.Drone = null;
            drone.Disconnect();
            drone.Discharged -= PreventDroneDisabling;
            slot.Bar.Level = null;
        }

        public Vector3 GetJointPosition(Controller drone)
        {
            return _slots.Find((slot) => slot.Drone == drone).JointPosition;
        }

        protected virtual void InitializeSlots()
        {
            _slots.Add(new Slot()
            {
                Bar = GameObject.Find("LeftDroneBar").GetComponent<UI.DroneBar>(),
                Drone = null,
                JointPosition = new Vector3(-0.3f, 0.4f, -0.5f),
            });
            _slots.Add(new Slot()
            {
                Bar = GameObject.Find("RightDroneBar").GetComponent<UI.DroneBar>(),
                Drone = null,
                JointPosition = new Vector3(0.3f, 0.4f, -0.5f),
            });
        }

        protected virtual void OnAllDronesDischarged(EventArgs e)
        {
            AllDronesDischarged?.Invoke(this, e);
        }

        protected virtual void OnDroneReplacementRequired(DroneReplacementEventArgs e)
        {
            DroneReplacementRequired?.Invoke(this, e);
        }

        protected virtual void OnFpvToTpvSwithing(EventArgs e)
        {
            FpvToTpvSwitching?.Invoke(this, e);
        }

        protected virtual void OnTpvToSpectatorSwitching(EventArgs e)
        {
            TpvToSpectatorSwitching?.Invoke(this, e);
        }

        private void PreventDroneDisabling(object sender, DischargedEventArgs e)
        {
            if (!HaveChargedDrone)
            {
                OnAllDronesDischarged(EventArgs.Empty);
            }
            else
            {
                OnDroneReplacementRequired(new DroneReplacementEventArgs(e.SavedState));
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(Settings.SwitchFpvToTpvModeKey))
            {
                OnFpvToTpvSwithing(EventArgs.Empty);
            }
            if (Input.GetKeyDown(Settings.SwitchTpvToSpectatorModeKey))
            {
                OnTpvToSpectatorSwitching(EventArgs.Empty);
            }

            _slots.ForEach((slot) =>
            {
                if (slot.Drone != null)
                {
                    slot.Bar.Level = slot.Drone.Charge / slot.Drone.MaxCharge;
                }
            });
        }

        protected class Slot
        {
            public UI.DroneBar Bar;
            public Controller Drone;
            public Vector3 JointPosition;
        }
    }
}
