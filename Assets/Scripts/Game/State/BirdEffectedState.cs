using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdEffectedState : BirdState
    {
        private Rigidbody _mRigidbody;
        private int _mWaitFrames;
        
        public BirdEffectedState(BirdStateManager birdStateManager, Transform targetTransform, Rigidbody rigidbody) : base(birdStateManager, targetTransform)
        {
            _mRigidbody = rigidbody;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Effected State Enter");
            State = BirdStateEnum.Effected;
            _mWaitFrames = GameAssets.WAIT_FRAMES;
        }

        public override void Update()
        {
            if (_mWaitFrames-- < 0)
            {
                if (_mRigidbody.velocity.magnitude < 30f)
                {
                    _mRigidbody.velocity = Vector3.zero;
                    _mBirdStateManager.ChangeState(BirdStateEnum.Stop);
                }
            }
        }

        public override void Exit()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Effected State Exit");
        }
    }
}