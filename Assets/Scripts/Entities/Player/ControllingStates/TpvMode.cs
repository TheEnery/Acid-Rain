
using System;
using UnityEngine;

namespace AcidRain.Entities.Player
{
    public partial class Controller
    {
        public class TpvMode : IControllingStateBase
        {
            public override bool PlayerShouldMove => true;

            public override void EnterState()
            {
                Player.IsCameraOn = false;
                Player.DroneConnector.AllDronesDischarged += ReturnToDefault;
                Player.DroneConnector.FpvToTpvSwitching += SwitchToFpv;
                Player.DroneConnector.TpvToSpectatorSwitching += SwitchToSpectator;
            }

            public override void ExitState()
            {
                Player.DroneConnector.AllDronesDischarged -= ReturnToDefault;
                Player.DroneConnector.FpvToTpvSwitching -= SwitchToFpv;
                Player.DroneConnector.TpvToSpectatorSwitching -= SwitchToSpectator;
            }

            public override Quaternion GetNextBodyRotation()
            {
                if (Player.IsWalking)
                {
                    Player.BodyYaw = Player.HeadYaw;
                }
                return Quaternion.Euler(0f, Player.BodyYaw, 0f);
            }

            public override Quaternion GetNextHeadRotation()
            {
                Player.HeadYaw += Settings.MouseSensitivity * Input.GetAxis("Mouse X");
                Player.HeadPitch += Settings.MouseSensitivity * Input.GetAxis("Mouse Y") * (Settings.InvertCamera ? -1 : 1);
                Player.HeadPitch = Mathf.Clamp(Player.HeadPitch, -MaxHeadPitchAngle, MaxHeadPitchAngle);
                if (Mathf.Abs(Player.HeadYaw - Player.BodyYaw) > MaxHeadYawAngle)
                {
                    return Quaternion.Euler(0f, Player.BodyYaw, 0f);
                }
                return Quaternion.Euler(Player.HeadPitch, Player.HeadYaw, 0f);
            }

            public override Vector3 GetNextPosition()
            {
                Vector3 desiredMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                return Quaternion.Euler(0f, Player.HeadYaw, 0f) * desiredMove + Player.Position;
            }

            private void ReturnToDefault(object sender, EventArgs e)
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
