using System;
using ChaosBall.Game.State;
using Unity.VisualScripting;
using UnityEngine;

namespace ChaosBall.Game
{
    public class NormalBirdCollideBehaviour : MonoBehaviour, IBirdCollideBehaviour
    {
        
        private BirdStateManager _mBirdStateManager;
        private Rigidbody _mRigidbody;
        
        public Vector3 LastVelocity { get; private set; }

        private void Awake()
        {
            _mBirdStateManager = GetComponent<BirdStateManager>();
            _mRigidbody = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            if (!_mBirdStateManager.Initialized) return;
            if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                LastVelocity = _mRigidbody.velocity;
            }
            else if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Stop)
            {
                LastVelocity = Vector3.zero;
            }
        }

        public void OnCollideOtherBird(Transform thisTransform, Collision other)
        {
            ContactPoint ct = other.GetContact(0);
            var collidePoint = ct.point;
            var dir = Vector3.Reflect(LastVelocity.normalized, ct.normal).normalized;
            if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Stop)
            {
                Vector3 otherLastVelocity = other.transform.GetComponent<BirdCollide>().LastVelocity;
                _mRigidbody.AddForceAtPosition(dir * otherLastVelocity.magnitude * .5f, collidePoint, ForceMode.Impulse);
            }
            else if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                _mRigidbody.velocity = Vector3.zero;
                _mRigidbody.AddForceAtPosition(dir * LastVelocity.magnitude * .5f, collidePoint, ForceMode.Impulse);
            }
        }
    }
}