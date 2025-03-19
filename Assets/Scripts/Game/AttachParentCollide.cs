
using System;
using UnityEngine;

namespace ChaosBall.Game
{
    public class AttachParentCollide : MonoBehaviour
    {
        public float CollideCostParam { get; set; }

        private Rigidbody _mRigidbody;

        private Vector3 LastVelocity;
        private Vector3 _mCollideDirection;
        private int _mWaitFrames;

        public event Action OnAttachParentStop;

        private void Awake()
        {
            _mRigidbody = GetComponent<Rigidbody>();
            _mWaitFrames = GameAssets.WAIT_FRAMES;
        }

        private void Update()
        {
            if (Vector3.Angle(transform.forward, _mCollideDirection) > 10f)
            {
                transform.forward = Vector3.Lerp(transform.forward, _mCollideDirection, Time.deltaTime * .3f);
            }

            if (_mWaitFrames-- < 0)
            {
                // parent stop
                if (_mRigidbody.velocity.magnitude < 30f)
                {
                    _mRigidbody.velocity = Vector3.zero;
                    OnAttachParentStop?.Invoke();
                }
            }
        }

        private void LateUpdate()
        {
            LastVelocity = _mRigidbody.velocity;
            Debug.Log("AttachParent, LastVelocity:" + LastVelocity);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == GameAssets.WALL_LAYER)
            {
                Debug.Log("AttachParentCollide, LastVelocity:" + LastVelocity);
                // 反弹
                ContactPoint contactPoint = other.GetContact(0);
                Vector3 reflect = Vector3.Reflect(LastVelocity.normalized, contactPoint.normal);
                var direction = reflect.normalized;
                _mCollideDirection = direction;
                _mRigidbody.velocity = direction * LastVelocity.magnitude * CollideCostParam;
            }
        }
    }
}