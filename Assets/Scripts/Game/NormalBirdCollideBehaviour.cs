using System;
using ChaosBall.Game.State;
using Unity.VisualScripting;
using UnityEngine;

namespace ChaosBall.Game
{
    public class NormalBirdCollideBehaviour : MonoBehaviour, IBirdCollideBehaviour
    {
        
        private BirdStateMachine _birdStateMachine;
        private Rigidbody _rigidbody;
        
        public Vector3 LastVelocity { get; private set; }

        private void Awake()
        {
            _birdStateMachine = GetComponent<BirdStateMachine>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            if (!_birdStateMachine.Initialized) return;
            if (_birdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                LastVelocity = _rigidbody.velocity;
            }
            else if (_birdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Stop)
            {
                LastVelocity = Vector3.zero;
            }
        }

        public void OnCollideOtherBird(Transform thisTransform, Collision other)
        {
            ContactPoint ct = other.GetContact(0);
            var collidePoint = ct.point;
            var dir = Vector3.Reflect(LastVelocity.normalized, ct.normal).normalized;
            if (_birdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Stop)
            {
                Vector3 otherLastVelocity = other.transform.GetComponent<BirdCollide>().LastVelocity;
                _rigidbody.AddForceAtPosition(dir * otherLastVelocity.magnitude * .5f, collidePoint, ForceMode.Impulse);
            }
            else if (_birdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.AddForceAtPosition(dir * LastVelocity.magnitude * .5f, collidePoint, ForceMode.Impulse);
            }
        }
    }
}