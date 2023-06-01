
using System;
using UnityEngine;

namespace AcidRain.Entities.Player
{
    public partial class Controller
    {
        public class SpectatorMode : IControllingStateBase
        {
            public override bool PlayerShouldMove => false;

            public override void EnterState()
            {
                Player.IsCameraOn = false;
                Player.DroneConnector.AllDronesDischarged += ReturnToDefault;
                Player.DroneConnector.TpvToSpectatorSwitching += SwitchToTpv;
            }

            public override void ExitState()
            {
                Player.DroneConnector.AllDronesDischarged -= ReturnToDefault;
                Player.DroneConnector.TpvToSpectatorSwitching -= SwitchToTpv;
            }

            public override Quaternion GetNextBodyRotation()
            {
                return Quaternion.Euler(0f, Player.BodyYaw, 0f);
            }

            public override Quaternion GetNextHeadRotation()
            {
                if (Mathf.Abs(Player.HeadYaw - Player.BodyYaw) > MaxHeadYawAngle)
                {
                    return Quaternion.Euler(0f, Player.BodyYaw, 0f);
                }
                return Quaternion.Euler(Player.HeadPitch, Player.HeadYaw, 0f);
            }

            public override Vector3 GetNextPosition()
            {
                return Player.Position;
            }

            private void ReturnToDefault(object sender, EventArgs e)
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