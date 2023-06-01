
using System;
using UnityEngine;

namespace AcidRain.Entities.Drone
{
    public partial class Controller
    {
        public class TpvMode : PlayerSharedState
        {
            private const float DefaultDistance = 7.5f;
            private readonly Vector3 DefaultShiftDirection = new Vector3(1.5f, 0.75f, -10f).normalized;

            private Vector3 _aimShift;
            private Vector3 _positionShift;

            public override bool IsDefault => false;
            public override bool NeedAttaching => false;

            public override void EnterState()
            {
                _positionShift = DefaultDistance * DefaultShiftDirection;
                _positionShift.x *= Mathf.Sign(Connector.GetJointPosition(Drone).x);
                _aimShift = new Vector3(_positionShift.x, _positionShift.y, 0f);
                Drone.IsCameraOn = true;
                Drone.Discharged += PreventDroneDisabling;
                Connector.FpvToTpvSwitching += SwitchToFpv;
                Connector.TpvToSpectatorSwitching += SwitchToSpectator;
            }

            public override void ExitState()
            {
                Drone.Discharged -= PreventDroneDisabling;
                Connector.FpvToTpvSwitching -= SwitchToFpv;
                Connector.TpvToSpectatorSwitching -= SwitchToSpectator;
            }

            public override Vector3 GetNextPosition()
            {
                Quaternion rotation = Quaternion.Euler(Connector.Player.HeadPitch, Connector.Player.HeadYaw, 0f);
                return Connector.Player.Position + rotation * _positionShift;
            }

            public override Quaternion GetNextRotation()
            {
                Quaternion rotation = Quaternion.Euler(Connector.Player.HeadPitch, Connector.Player.HeadYaw, 0f);
                Vector3 lookDirection = Connector.Player.Position + rotation * _aimShift - GetNextPosition();
                return Quaternion.LookRotation(lookDirection, Vector3.up);
            }

            private void PreventDroneDisabling(object sender, EventArgs e)
            {
                OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new FpvMode() });
            }

            private void SwitchToFpv(object sender, EventArgs e)
            {
                OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new FpvMode() });
            }

            private void SwitchToSpectator(object sender, EventArgs e)
            {
                OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new SpectatorMode() });
            }
        }
    }
}
