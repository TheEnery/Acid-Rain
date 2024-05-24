using System;
using UnityEngine;

namespace AcidRain.Entities.Drone
{
    public partial class Controller
    {
        public class SpectatorMode : PlayerSharedState
        {
            private bool _initialized = false;
            private float _pitch;
            private Vector3 _position;
            private float _yaw;

            public override bool IsDefault => false;
            public override bool NeedAttaching => false;

            public override void EnterState()
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _pitch = Connector.Player.HeadPitch;
                    _position = Drone.Position;
                    _yaw = Connector.Player.HeadYaw;
                }
                Drone.IsCameraOn = true;
                Drone.Discharged += PreventDroneDisabling;
                Connector.TpvToSpectatorSwitching += SwitchToTpv;
            }

            public override void ExitState()
            {
                Drone.Discharged -= PreventDroneDisabling;
                Connector.TpvToSpectatorSwitching -= SwitchToTpv;
            }

            public override Vector3 GetNextPosition()
            {
                Vector3 rawMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                Vector3 desiredMove = Drone.Rotation * rawMove;
                return _position += desiredMove;
            }

            public override Quaternion GetNextRotation()
            {
                _yaw += Settings.MouseSensitivity * Input.GetAxis("Mouse X");
                _pitch += Settings.MouseSensitivity * Input.GetAxis("Mouse Y") * (Settings.InvertCamera ? -1 : 1);
                _pitch = Mathf.Clamp(_pitch, -90f, 90f);
                return Quaternion.Euler(_pitch, _yaw, 0f);
            }

            private void PreventDroneDisabling(object sender, EventArgs e)
            {
                OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new FpvMode() });
            }

            private void SwitchToTpv(object sender, EventArgs e)
            {
                OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new TpvMode() });
            }
        }
    }
}