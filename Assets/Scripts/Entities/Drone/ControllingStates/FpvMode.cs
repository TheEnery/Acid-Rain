
using System;
using UnityEngine;

namespace AcidRain.Entities.Drone
{
    public partial class Controller
    {
        public class FpvMode : PlayerSharedState
        {
            private Vector3 _jointPosition;

            public override bool IsDefault => true;
            public override bool NeedAttaching => true;

            public override void EnterState()
            {
                _jointPosition = Connector.GetJointPosition(Drone);
                Drone.IsCameraOn = false;
                Connector.DroneReplacementRequired += ReplaceState;
                Connector.FpvToTpvSwitching += SwitchToTpv;
            }

            public override void ExitState()
            {
                Connector.DroneReplacementRequired -= ReplaceState;
                Connector.FpvToTpvSwitching -= SwitchToTpv;
            }

            public override Vector3 GetNextPosition()
            {
                return Connector.Player.Position + Quaternion.Euler(0f, Connector.Player.BodyYaw, 0f) * _jointPosition;
            }

            public override Quaternion GetNextRotation()
            {
                return Quaternion.Euler(20f, Connector.Player.BodyYaw, 0f);
            }

            private void ReplaceState(object sender, DroneReplacementEventArgs e)
            {
                if (Connector.NextActiveDrone == Drone)
                {
                    OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = e.DesiredState });
                }
            }

            private void SwitchToTpv(object sender, EventArgs e)
            {
                if (Connector.NextActiveDrone == Drone)
                {
                    OnChangeRequested(new IControllingState.ChangeStateEventArgs() { NextState = new TpvMode() });
                }
            }
        }
    }
}