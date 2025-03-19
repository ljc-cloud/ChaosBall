using System;
using ChaosBall.Game.State;
using UnityEngine;

namespace ChaosBall.Game
{
    public class AttachBirdCollideBehaviour : MonoBehaviour, IBirdCollideBehaviour
    {
        // [SerializeField] private Transform attachParent;
        
        private BirdStateManager _mBirdStateManager;
        private Rigidbody _mRigidbody;
        public Vector3 LastVelocity { get; private set; }

        private Vector3 _mPositionConstraint1;
        private Vector3 _mPositionConstraint2;

        private Transform _mAttachParentTransform;
        private Transform _mOtherTransform;
        private AttachParentCollide _mAttachParentCollide;
        private bool _mHasCollided;
        private bool _mAttachParentStopped;

        private void Awake()
        {
            _mBirdStateManager = GetComponent<BirdStateManager>();
            _mRigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!_mHasCollided || _mAttachParentStopped) return;
            // transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, _mPositionConstraint1);
            // _mOtherTransform.localPosition = Vector3.ClampMagnitude(_mOtherTransform.localPosition, _mPositionConstraint2);

            transform.localPosition = _mPositionConstraint1;
            _mOtherTransform.localPosition = _mPositionConstraint2;
            _mAttachParentTransform.rotation = Quaternion.LookRotation(_mOtherTransform.position - transform.position);
            
            
            // transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, _mBird1PositionConstraint);
            // _mOtherTransform.localPosition = Vector3.ClampMagnitude(_mOtherTransform.position, _mBird2PositionConstraint);
        }

        private void LateUpdate()
        {
            if (!_mBirdStateManager.Initialized) return;
            if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                LastVelocity = _mRigidbody.velocity;
            }
        }

        private void OnDestroy()
        {
            _mAttachParentCollide.OnAttachParentStop -= OnAttachParentStop;
        }

        // 吸附球实现
        public void OnCollideOtherBird(Transform thisTransform, Collision other)
        {
            Debug.Log("LastVelocity: " + LastVelocity);
            
            other.transform.GetComponent<BirdStateManager>().ChangeState(BirdState.BirdStateEnum.Effected);
            
            _mOtherTransform = other.transform;

            GameObject attachParentGameObject = new GameObject("AttachParent");
            _mAttachParentTransform = attachParentGameObject.transform;
            _mAttachParentTransform.position = (transform.position + _mOtherTransform.position) / 2f;
            _mAttachParentTransform.rotation = Quaternion.LookRotation(_mOtherTransform.position - transform.position);

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

            _mAttachParentCollide = attachParentGameObject.AddComponent<AttachParentCollide>();
            _mAttachParentCollide.CollideCostParam = 0.8f;
            _mAttachParentCollide.OnAttachParentStop += OnAttachParentStop;

            transform.GetComponent<Collider>().enabled = false;
            _mOtherTransform.GetComponent<Collider>().enabled = false;
            
            transform.SetParent(attachParentGameObject.transform);
            _mOtherTransform.SetParent(attachParentGameObject.transform);

            _mPositionConstraint1 = transform.localPosition;
            _mPositionConstraint2 = _mOtherTransform.localPosition;

            attachParentRigidbody.velocity = LastVelocity;
            
            // _mOtherTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // _mRigidbody.velocity = Vector3.zero;

            _mHasCollided = true;
        }

        private void OnAttachParentStop()
        {
            Debug.Log("吸附球停止移动");
            _mAttachParentStopped = true;
            transform.SetParent(null);
            _mOtherTransform.SetParent(null);

            transform.GetComponent<Collider>().enabled = true;
            _mOtherTransform.GetComponent<Collider>().enabled = true;
            
            _mOtherTransform.GetComponent<BirdStateManager>().ChangeState(BirdState.BirdStateEnum.Stop);
            
            Destroy(_mAttachParentCollide.gameObject);
        }
    }
}