
using UnityEngine;

namespace AcidRain.Entities.Player
{
    public partial class Controller : MonoBehaviour
    {
        public const float MaxHeadPitchAngle = 70f;
        public const float MaxHeadYawAngle = 60f;
        public const float MovementSpeed = 15f;

        private IControllingState _aimer;
        private Rigidbody _bodyRigidbody;
        private Camera _camera;
        private Rigidbody _headRigidbody;
        [SerializeReference] private Utilities.Physics.IPdController _pd; //

        public float BodyYaw { get; private set; } = 0f;
        public Drone.PlayerConnector DroneConnector { get; private set; }
        public float HeadPitch { get; private set; } = 0f;
        public float HeadYaw { get; private set; } = 0f;
        public bool IsCameraOn { get { return _camera.enabled; } private set { _camera.enabled = value; } }
        public bool IsWalking { get; private set; }
        public Vector3 Position { get { return _bodyRigidbody.position; } }
        public Quaternion Rotation { get { return _headRigidbody.rotation; } }

        private void AimBodyTo(Quaternion desiredRotation)
        {
            Vector3 neededTorque = _pd.GetTorque(_bodyRigidbody, desiredRotation);
            _bodyRigidbody.AddTorque(neededTorque);
        }

        private void AimHeadTo(Quaternion desiredRotation)
        {

            Vector3 neededTorque = _pd.GetTorque(_headRigidbody, desiredRotation);
            _headRigidbody.AddTorque(neededTorque);
        }

        private void Awake()
        {
            _bodyRigidbody = GetComponent<Rigidbody>();
            Transform head = transform.GetChild(0);
            _headRigidbody = head.GetComponent<Rigidbody>();
            _camera = head.GetComponent<Camera>();

            _pd = new Utilities.Physics.ForwardPd();
            DroneConnector = Drone.PlayerConnector.Create(this, _bodyRigidbody);
            SetAimer(new FpvMode());
        }

        private void ChangeAimer(object sender, IControllingState.ChangeStateEventArgs e)
        {
            SetAimer(e.NextState);
        }

        private void MoveTo(Vector3 desiredPosition)
        {
            if (desiredPosition == Position)
            {
                IsWalking = false;
                return;
            }
            IsWalking = true;
            Vector3 desiredVelocity = (desiredPosition - Position) * MovementSpeed;
            Vector3 neededForce = _pd.GetForce(_bodyRigidbody.position, desiredPosition, _bodyRigidbody.velocity, desiredVelocity);
            _bodyRigidbody.AddForce(neededForce);
        }

        private void FixedUpdate()
        {
            if (_aimer.PlayerShouldMove)
            {
                MoveTo(_aimer.GetNextPosition());
            }
            AimHeadTo(_aimer.GetNextHeadRotation());
            AimBodyTo(_aimer.GetNextBodyRotation());
        }

        private void SetAimer(IControllingState aimer)
        {
            if (_aimer != null)
            {
                _aimer.ExitState();
                _aimer.ChangeRequested -= ChangeAimer;
            }
            aimer.Player = this;
            _aimer = aimer;
            _aimer.ChangeRequested += ChangeAimer;
            _aimer.EnterState();
        }
    }
}