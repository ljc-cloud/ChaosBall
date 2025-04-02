using System;
using ChaosBall.Math;
using ChaosBall.Net;
using ChaosBall.Utility;
using UnityEngine;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// Shoot 状态机实现
    /// </summary>
    public class BirdShootState : BirdState
    {
        private Rigidbody _mRigidBody;
        private Func<float> _mGetShootForce;
        private Func<Vector3> _mGetDirection;

        private int _mWaitFrames;
        private float _mOriginVelocityMagnitude;
        
        public BirdShootState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity, Rigidbody rigidbody
            , Func<float> getShootForce, Func<Vector3> getDirection) 
            : base(birdStateManager, targetTransform, entity)
        {
            _mRigidBody = rigidbody;
            _mGetShootForce = getShootForce;
            _mGetDirection = getDirection;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Shoot State Enter");
            State = BirdStateEnum.Shoot;
            _mWaitFrames = GameAssets.WAIT_FRAMES;
            _mBirdStateManager.BirdAnimation.SetCurrentAnimationSpeed(1f);
            
            _mBirdStateManager.BirdAnimation.PlayShoot();
            
            _mBirdStateManager.ArrowForceUI.Hide();

            Vector3 direction = Vector3.zero;
            float shootForce;
            if (Entity.IsLocal)
            {
                Vector3 localDirection = Entity.localShootDirection;
                direction = new Vector3(MathUtil.GetFloat(localDirection.x)
                    , MathUtil.GetFloat(localDirection.y),
                    MathUtil.GetFloat(localDirection.z));
                shootForce = MathUtil.GetFloat(Entity.localShootForce);
            }
            else
            {
                direction = Entity.shootDirection;
                shootForce = Entity.shootForce;
            }
            
            _mTargetTransform.forward = direction;
            _mRigidBody.AddForce(direction * shootForce, ForceMode.Impulse);
        }

        public override void Update()
        {
            if (_mRigidBody.velocity.sqrMagnitude <= 900f)
            {
                if (_mWaitFrames > 0)
                {
                    _mWaitFrames--;
                }
                else
                {
                    _mBirdStateManager.ChangeState(BirdStateEnum.Stop);
                }
            }
            else
            {
                _mOriginVelocityMagnitude = Mathf.Max(_mRigidBody.velocity.magnitude, _mOriginVelocityMagnitude);
                float normalizedSpeed = _mRigidBody.velocity.magnitude / _mOriginVelocityMagnitude;
                _mBirdStateManager.BirdAnimation.SetCurrentAnimationSpeed(normalizedSpeed);
            }
        }

        public override void Exit()
        {
            _mRigidBody.velocity = Vector3.zero;
            _mBirdStateManager.BirdAnimation.SetCurrentAnimationSpeed(1f);
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Shoot State Exit");
        }
    }
}