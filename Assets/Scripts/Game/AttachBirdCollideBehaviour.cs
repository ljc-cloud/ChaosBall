using System;
using ChaosBall.Event.Game;
using ChaosBall.Game.State;
using UnityEngine;

namespace ChaosBall.Game
{
    public class AttachBirdCollideBehaviour : MonoBehaviour, IBirdCollideBehaviour
    {
        // [SerializeField] private Transform attachParent;
        
        private BirdStateMachine _birdStateMachine;
        private Rigidbody _rigidbody;
        public Vector3 LastVelocity { get; private set; }

        private Vector3 _positionConstraint1;
        private Vector3 _positionConstraint2;

        private Transform _attachParentTransform;
        private Transform _otherTransform;
        private AttachParentCollide _attachParentCollide;
        private bool _hasCollided;
        private bool _attachParentStopped;

        private void Awake()
        {
            _birdStateMachine = GetComponent<BirdStateMachine>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!_hasCollided || _attachParentStopped) return;
            // transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, _mPositionConstraint1);
            // _mOtherTransform.localPosition = Vector3.ClampMagnitude(_mOtherTransform.localPosition, _mPositionConstraint2);

            transform.localPosition = _positionConstraint1;
            _otherTransform.localPosition = _positionConstraint2;
            _attachParentTransform.rotation = Quaternion.LookRotation(_otherTransform.position - transform.position);
            
            
            // transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, _mBird1PositionConstraint);
            // _mOtherTransform.localPosition = Vector3.ClampMagnitude(_mOtherTransform.position, _mBird2PositionConstraint);
        }

        private void LateUpdate()
        {
            if (!_birdStateMachine.Initialized) return;
            if (_birdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                LastVelocity = _rigidbody.velocity;
            }
        }

        private void OnDestroy()
        {
            // _mAttachParentCollide.OnAttachParentStop -= OnAttachParentStop;
            GameInterface.Interface.EventSystem.Unsubscribe<AttachParentStopEvent>(OnAttachParentStop);
        }

        // 吸附球实现
        public void OnCollideOtherBird(Transform thisTransform, Collision other)
        {
            // Debug.Log("LastVelocity: " + LastVelocity);
            
            other.transform.GetComponent<BirdStateMachine>().ChangeState(BirdState.BirdStateEnum.Effected);
            
            _otherTransform = other.transform;

            GameObject attachParentGameObject = new GameObject("AttachParent");
            _attachParentTransform = attachParentGameObject.transform;
            _attachParentTransform.position = (transform.position + _otherTransform.position) / 2f;
            _attachParentTransform.rotation = Quaternion.LookRotation(_otherTransform.position - transform.position);

            CapsuleCollider capsuleCollider = attachParentGameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 16f;
            capsuleCollider.height = 64f;
            capsuleCollider.center = new Vector3(0, 12, 0);
            capsuleCollider.direction = 2;

            Rigidbody attachParentRigidbody = attachParentGameObject.AddComponent<Rigidbody>();
            attachParentRigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                                                RigidbodyConstraints.FreezeRotationX |
                                                RigidbodyConstraints.FreezeRotationZ;
            attachParentRigidbody.drag = 0.1f;

            _attachParentCollide = attachParentGameObject.AddComponent<AttachParentCollide>();
            _attachParentCollide.CollideCostParam = 0.8f;
            
            GameInterface.Interface.EventSystem.Subscribe<AttachParentStopEvent>(OnAttachParentStop);
            // _mAttachParentCollide.OnAttachParentStop += OnAttachParentStop;

            transform.GetComponent<Collider>().enabled = false;
            _otherTransform.GetComponent<Collider>().enabled = false;
            
            transform.SetParent(attachParentGameObject.transform);
            _otherTransform.SetParent(attachParentGameObject.transform);

            _positionConstraint1 = transform.localPosition;
            _positionConstraint2 = _otherTransform.localPosition;

            attachParentRigidbody.velocity = LastVelocity;
            
            // _mOtherTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // _mRigidbody.velocity = Vector3.zero;

            _hasCollided = true;
        }

        private void OnAttachParentStop(AttachParentStopEvent _)
        {
            // Debug.Log("吸附球停止移动");
            _attachParentStopped = true;
            transform.SetParent(null);
            _otherTransform.SetParent(null);

            transform.GetComponent<Collider>().enabled = true;
            _otherTransform.GetComponent<Collider>().enabled = true;
            
            _otherTransform.GetComponent<BirdStateMachine>().ChangeState(BirdState.BirdStateEnum.Stop);
            
            Destroy(_attachParentCollide.gameObject);
        }
    }
}