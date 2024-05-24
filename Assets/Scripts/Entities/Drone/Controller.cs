
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
        public const float MovementSpeed = 0.3f;
        private const float _positionAccuracy = 0.2f;
        private const float _rotationAccuracy = 7f;

        private IControllingState _aimer;
        private Camera _camera;
        private Rigidbody _cameraRigidbody;
        private Rigidbody _droneRigidbody;
        private bool _isDischarged = false;
        private Utilities.Physics.IPdController _pd;
        private Rigidbody _rigidbodyForAttaching;

        public event EventHandler<DischargedEventArgs> Discharged;

        public float Charge { get; private set; } = 100000f;
        public IConnector Connector { get; private set; }
        public bool IsCameraOn { get { return _camera.enabled; } private set { _camera.enabled = value; } }
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
        public float MaxCharge { get; private set; } = 1000f;
        public Vector3 Position { get { return _droneRigidbody.position; } }
        public Quaternion Rotation { get { return _cameraRigidbody.rotation; } }

        public void ConnectTo(IConnector connector, Rigidbody rigidbody, IControllingState aimer, Utilities.Physics.IPdController pd)
        {
            Connector = connector;
            SetAimer(aimer);
            _pd = pd;
            _rigidbodyForAttaching = rigidbody;
            IsEnabled = true;
        }

        public void Disconnect()
        {
            Connector = null;
            SetAimer(null);
            _pd = null;
            _rigidbodyForAttaching = null;
            IsEnabled = false;
        }

        protected virtual void OnDischarged(DischargedEventArgs e)
        {
            Discharged?.Invoke(this, e);
        }

        private void AimTo(Quaternion rotation)
        {
            Vector3 euler = rotation.eulerAngles;

            Vector3 droneTorque = _pd.GetTorque(_droneRigidbody, Quaternion.Euler(0f, euler.y, 0f));
            _droneRigidbody.AddTorque(droneTorque);

            Vector3 cameraTorque = _pd.GetTorque(_cameraRigidbody, Quaternion.Euler(euler.x, 0f, euler.z));
            _cameraRigidbody.AddTorque(cameraTorque);
        }

        private void Attach()
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = _rigidbodyForAttaching;
            IsEnabled = false;
        }

        private void Awake()
        {
            _cameraRigidbody = gameObject.transform.GetChild(0).GetComponent<Rigidbody>();
            _droneRigidbody = gameObject.GetComponent<Rigidbody>();
            _camera = transform.GetChild(0).GetComponent<Camera>();
        }

        private void ChangeAimer(object sender, IControllingState.ChangeStateEventArgs e)
        {
            SetAimer(e.NextState);
        }

        private void Detach()
        {
            Destroy(GetComponent<FixedJoint>());
            IsEnabled = true;
        }

        private void MoveTo(Vector3 desiredPosition)
        {
            Vector3 desiredVelocity = (desiredPosition - Position) * MovementSpeed;
            Vector3 neededForce = _pd.GetForce(_droneRigidbody.position, desiredPosition, _droneRigidbody.velocity, desiredVelocity);
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