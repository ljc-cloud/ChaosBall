using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdStopState : BirdState
    {
        private Rigidbody _mRigidbody;
        private IBirdStopBehaviour _mBirdStopBehaviour;
        
        public BirdStopState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity, Rigidbody rigidbody
            , IBirdStopBehaviour birdStopBehaviour) : base(birdStateManager, targetTransform, entity)
        {
            _mRigidbody = rigidbody;
            _mBirdStopBehaviour = birdStopBehaviour;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Stop State Enter");
            State = BirdStateEnum.Stop;
            _mBirdStateManager.BirdAnimation.PlayIdle();
            _mRigidbody.velocity = Vector3.zero;
            
            _mBirdStopBehaviour?.OnStop();
            
            if (FromStateEnum is BirdStateEnum.Shoot or BirdStateEnum.Effected or BirdStateEnum.Collided)
            {
                _mBirdStateManager.ChangeState(BirdStateEnum.Count);
            }
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Stop State Exit");
        }
    }
}