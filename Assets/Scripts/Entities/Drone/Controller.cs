using AcidRain.Utilities.General;
using System;
using UnityEngine;

namespace AcidRain.Entities.Drone
{
    public class DischargedEventArgs : EventArgs
    {
        private Controller.IControllingState _savedState;

        public Controller.IControllingState SavedState
        {
            get { return _savedState.Clone(); }
            private set { _savedState = value; }
        }

        public DischargedEventArgs(Controller.IControllingState savedState)
        {
            _savedState = savedState;
        }
    }

    public partial class Controller : MonoBehaviour
    {
        public const float MaxMovementForce = 300f;
        private const float _positionAccuracy = 0.2f;
        private const float _rotationAccuracy = 7f;

        private IControllingState _aimer;
        private Camera _camera;
        private Rigidbody _cameraRigidbody;
        private Rigidbody _droneRigidbody;
        private FixedJoint _cameraToConnectorJoint;
        private FixedJoint _cameraToDroneJoint;
        private Rigidbody _connectorRigidbody;
        private bool _isDischarged = false;
        [SerializeReference] private Utilities.General.IRotationController _cameraRotationPd;
        [SerializeReference] private Utilities.General.IRotationController _droneRotationPd;
        [SerializeReference] private Utilities.General.IMovementController _droneMovementPd;

        public event EventHandler<DischargedEventArgs> Discharged;

        public float Charge { get; private set; } = 100000f;
        public IConnector Connector { get; private set; }
        public bool IsCameraOn
        {
            get { return _camera.enabled; }
            private set { _camera.enabled = value; }
        }
        public bool InDefaultState { get => _aimer.IsDefault; }
        public bool IsDischarged
        {
            get
            {
                return _isDischarged;
            }
            private set
            {
                if (value != _isDischarged)
                {
                    _isDischarged = value;
                    if (value)
                    {
                        OnDischarged(new DischargedEventArgs(_aimer));
                    }
                }
            }
        }
        public bool IsEnabled { get; private set; } = false;
        public float LowCharge { get; private set; } = 100f;
        public float Mass => _cameraRigidbody.mass + _droneRigidbody.mass;
        public float MaxCharge { get; private set; } = 100000f;
        public Vector3 Position { get { return _droneRigidbody.position; } }
        public Quaternion Rotation { get { return _cameraRigidbody.rotation; } }

        public void ConnectTo(IConnector connector, Rigidbody rigidbody, IControllingState aimer)
        {
            Connector = connector;
            SetAimer(aimer);

            _cameraRotationPd = new Utilities.Controllers.ForwardRotationPd()
            {
                Rigidbody = _cameraRigidbody,
                Derivative = 1400f,
                Proportional = 800f
            };
            _droneRotationPd = new Utilities.Controllers.ForwardRotationPd()
            {
                Rigidbody = _droneRigidbody,
                Derivative = 1800f,
                Proportional = 900f
            };
            _droneMovementPd = new Utilities.Controllers.StableMovementControllerV1()
            {
                Rigidbody = _droneRigidbody,
                MaxForce = MaxMovementForce,
                Mass = Mass
            };

            _connectorRigidbody = rigidbody;
            IsEnabled = true;
        }

        public void Disconnect()
        {
            Connector = null;
            SetAimer(null);
            _connectorRigidbody = null;
            IsEnabled = false;
        }

        protected virtual void OnDischarged(DischargedEventArgs e)
        {
            Discharged?.Invoke(this, e);
        }

        private void AimTo(Quaternion rotation)
        {
            Vector3 droneUp = Vector3.up;
            Quaternion droneRotation = rotation.ProjectOnPlane(droneUp);

            Vector3 droneTorque = _droneRotationPd.GetTorque(droneRotation);
            _droneRigidbody.AddTorque(droneTorque);

            Vector3 cameraRight = _droneRigidbody.rotation * Vector3.right;
            Quaternion cameraRotation = rotation.ProjectOnPlane(cameraRight);

            Vector3 cameraTorque = _cameraRotationPd.GetTorque(cameraRotation);
            _cameraRigidbody.AddTorque(cameraTorque);
        }

        private void Attach()
        {
            _cameraToConnectorJoint = _cameraRigidbody.gameObject.AddComponent<FixedJoint>();
            _cameraToConnectorJoint.connectedBody = _connectorRigidbody;

            _cameraToDroneJoint = _cameraRigidbody.gameObject.AddComponent<FixedJoint>();
            _cameraToDroneJoint.connectedBody = _droneRigidbody;

            IsEnabled = false;
        }

        private void Awake()
        {
            var cameraTransform = transform.GetChild(0);
            _camera = cameraTransform.GetChild(0).GetComponent<Camera>();
            _cameraRigidbody = cameraTransform.GetComponent<Rigidbody>();
            _droneRigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void ChangeAimer(object sender, IControllingState.ChangeStateEventArgs e)
        {
            SetAimer(e.NextState);
        }

        private void Detach()
        {
            Destroy(_cameraToConnectorJoint);
            _cameraToConnectorJoint = null;

            Destroy(_cameraToDroneJoint);
            _cameraToDroneJoint = null;

            IsEnabled = true;
        }

        private void MoveTo(Vector3 desiredPosition)
        {
            Vector3 neededForce = _droneMovementPd.GetForce(desiredPosition);
            _droneRigidbody.AddForce(neededForce);
        }

        private void FixedUpdate()
        {
            if (!IsEnabled)
            {
                return;
            }

            Vector3 nextPosition = _aimer.GetNextPosition();
            Quaternion nextRotation = _aimer.GetNextRotation();

            MoveTo(nextPosition);
            AimTo(nextRotation);

            if (_aimer.NeedAttaching)
            {
                bool positionReached = Vector3.Distance(Position, nextPosition) < _positionAccuracy;
                bool rotationReached = Quaternion.Angle(Rotation, nextRotation) < _rotationAccuracy;
                if (positionReached && rotationReached)
                {
                    Attach();
                }
            }

            Charge--;
            IsDischarged = Charge < LowCharge;
        }

        private void SetAimer(IControllingState aimer)
        {
            if (_aimer != null)
            {
                _aimer.ExitState();
                _aimer.ChangeRequested -= ChangeAimer;
            }

            if (!IsEnabled)
            {
                Detach();
            }
            aimer.Drone = this;
            _aimer = aimer;
            _aimer.ChangeRequested += ChangeAimer;
            _aimer.EnterState();
        }
    }
}