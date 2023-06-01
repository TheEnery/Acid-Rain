
using System;
using UnityEngine;

namespace AcidRain.Entities.Player
{
    public partial class Controller
    {
        public class FpvMode : IControllingStateBase
        {
            public override bool PlayerShouldMove => true;

            public override void EnterState()
            {
                Player.IsCameraOn = true;
                Player.DroneConnector.FpvToTpvSwitching += SwitchToTpv;
            }

            public override void ExitState()
            {
                Player.DroneConnector.FpvToTpvSwitching -= SwitchToTpv;
            }

            public override Quaternion GetNextBodyRotation()
            {
                if (Mathf.Abs(Player.HeadYaw - Player.BodyYaw) > MaxHeadYawAngle || Player.IsWalking)
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
                return Quaternion.Euler(Player.HeadPitch, Player.HeadYaw, 0f);
            }

            public override Vector3 GetNextPosition()
            {
                Vector3 desiredMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                return Quaternion.Euler(0f, Player.HeadYaw, 0f) * desiredMove + Player.Position;
            }

            private void SwitchToTpv(object sender, EventArgs e)
            {
                if (Player.DroneConnector.HaveChargedDrone)
                {
                    OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new TpvMode() });
                }
            }
        }
    }
}
