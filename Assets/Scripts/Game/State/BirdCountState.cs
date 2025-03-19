using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdCountState : BirdState
    {
        private BirdCollide _mBirdCollide;

        private bool _mCounted;
        public BirdCountState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity, BirdCollide birdCollide) 
            : base(birdStateManager, targetTransform, entity)
        {
            _mBirdCollide = birdCollide;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Count State Enter");
            State = BirdStateEnum.Count;
            if (Entity.IsLocal)
            {
                // GameInterface.Interface.GameManager.FinishOperation(Entity.playerId);
            }
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Count State Exit");
        }
    }
}