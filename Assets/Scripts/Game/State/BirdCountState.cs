using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdCountState : BirdState
    {
        private BirdCollide _mBirdCollide;

        private bool _mCounted;
        public BirdCountState(BirdStateManager birdStateManager, Transform targetTransform, BirdCollide birdCollide) 
            : base(birdStateManager, targetTransform)
        {
            _mBirdCollide = birdCollide;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Count State Enter");
            State = BirdStateEnum.Count;
            _mBirdCollide.OnBirdStayInArea += CountScore;
        }
        private void CountScore(Area area)
        {
            if (_mCounted) return;
            _mCounted = true;
            GameManager.Instance.FinishRound(area.Score);
            _mBirdStateManager.ChangeState(BirdStateEnum.Stop);
            Debug.Log($"计算分数: {area.Score}");
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Count State Exit");
            _mBirdCollide.OnBirdStayInArea -= CountScore;
        }
    }
}