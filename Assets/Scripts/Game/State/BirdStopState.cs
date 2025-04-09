using ChaosBall.Event.Game;
using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdStopState : BirdState
    {
        private Rigidbody _rigidbody;
        private IBirdStopBehaviour _birdStopBehaviour;

        public BirdStopState(BirdStateMachine birdStateMachine, BirdAnimation birdAnimation, Entity entity,
            Rigidbody rigidbody
            , IBirdStopBehaviour birdStopBehaviour) : base(birdStateMachine, birdAnimation, entity)
        {
            _rigidbody = rigidbody;
            _birdStopBehaviour = birdStopBehaviour;
        }

        public override void Enter()
        {
            // Debug.Log($"Bird:{TargetTransform.gameObject.name} Stop State Enter");
            State = BirdStateEnum.Stop;
            Animation.PlayIdle();
            _rigidbody.velocity = Vector3.zero;
            
            _birdStopBehaviour?.OnStop();
            
            Debug.Log("发布停止事件");
            if (Entity.IsLocal)
            {
                GameInterface.Interface.EventSystem.Publish<BirdStopEvent>();
            }
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            // Debug.Log($"Bird:{TargetTransform.gameObject.name} Stop State Exit");
        }
    }
}