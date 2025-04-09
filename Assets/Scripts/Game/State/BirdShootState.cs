using ChaosBall.Net;
using ChaosBall.UI;
using ChaosBall.Utility;
using UnityEngine;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// Shoot 状态机实现
    /// </summary>
    public class BirdShootState : BirdState
    {
        private Transform _transform;
        private Rigidbody _rigidBody;
        private int _waitFrames;
        private float _originVelocityMagnitude;
        private ArrowForceUI _arrowForceUI;

        public BirdShootState(BirdStateMachine birdStateMachine, BirdAnimation birdAnimation, Entity entity,
            Transform transform, Rigidbody rigidbody
            , ArrowForceUI arrowForceUI)
            : base(birdStateMachine, birdAnimation, entity)
        {
            _transform = transform;
            _rigidBody = rigidbody;
            _arrowForceUI = arrowForceUI;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_transform.gameObject.name} Shoot State Enter");
            State = BirdStateEnum.Shoot;
            _waitFrames = GameAssets.WAIT_FRAMES;
            Animation.SetCurrentAnimationSpeed(1f);
            
            Animation.PlayShoot();
            
            _arrowForceUI.Hide();

            Vector3 direction = Vector3.zero;
            float shootForce;
            // if (Entity.IsLocal)
            // {
            //     Vector3 localDirection = Entity.localShootDirection;
            //     direction = new Vector3(MathUtil.GetFloat(localDirection.x)
            //         , MathUtil.GetFloat(localDirection.y),
            //         MathUtil.GetFloat(localDirection.z));
            //     shootForce = MathUtil.GetFloat(Entity.localShootForce);
            // }
            // else
            // {
            //     direction = Entity.shootDirection;
            //     shootForce = Entity.shootForce;
            // }
            // direction = Entity.shootDirection;
            direction = Entity.shootDirection;
            shootForce = Entity.shootForce;
            
            _transform.forward = direction;
            Debug.Log($"Shoot direction: {direction}");
            _rigidBody.AddForce(direction * shootForce, ForceMode.Impulse);
        }

        public override void Update()
        {
            if (_rigidBody.velocity.sqrMagnitude <= 900f)
            {
                if (_waitFrames > 0)
                {
                    _waitFrames--;
                }
                else
                {
                    BirdStateMachine.ChangeState(BirdStateEnum.Stop);
                }
            }
            else
            {
                _originVelocityMagnitude = Mathf.Max(_rigidBody.velocity.magnitude, _originVelocityMagnitude);
                float normalizedSpeed = _rigidBody.velocity.magnitude / _originVelocityMagnitude;
                Animation.SetCurrentAnimationSpeed(normalizedSpeed);
            }
        }

        public override void Exit()
        {
            _rigidBody.velocity = Vector3.zero;
            Animation.SetCurrentAnimationSpeed(1f);
            Debug.Log($"Bird:{_transform.gameObject.name} Shoot State Exit");
        }
    }
}