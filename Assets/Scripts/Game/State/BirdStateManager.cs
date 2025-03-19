using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdStateManager : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float maxShootForce;
        [SerializeField] private float minShootForce;
        [SerializeField] private float readyShootDuration;
        [SerializeField] private ArrowForceUI arrowForceUI;
        [SerializeField] private BirdAnimation birdAnimation;
        [SerializeField] private BirdType birdType;
        
        private Rigidbody _mRigidBody;
        private BirdCollide _mBirdCollide;
        private IBirdStopBehaviour _mBirdStopBehaviour;
        
        private BirdUnReadyState _mUnReadyState;
        private BirdReadyState _mReadyState;
        private BirdShootState _mShootState;
        private BirdStopState _mStopState;
        private BirdCountState _mCountState;
        private BirdCollideState _mCollideState;
        private Entity _mEntity;

        private float _mShootForce;
        
        public bool Initialized { get; private set; }
        public BirdState CurrentState { get; private set; }
        public BirdAnimation BirdAnimation => birdAnimation;
        public ArrowForceUI ArrowForceUI => arrowForceUI;
        public Entity Entity { get; private set; }
        public bool IsLocal => Entity.playerType is Entity.PlayerType.Local;

        private void Awake()
        {
            _mRigidBody = GetComponent<Rigidbody>();
            _mBirdCollide = GetComponent<BirdCollide>();
            Entity = GetComponent<Entity>();
            _mEntity = GetComponent<Entity>();
        }

        private void Start()
        {
            _mUnReadyState = new BirdUnReadyState(this, transform, _mEntity, speed);
            _mReadyState = new BirdReadyState(this, transform, _mEntity, minShootForce, maxShootForce,
                readyShootDuration);
            _mShootState = new BirdShootState(this, transform, _mEntity, _mRigidBody, GetShootForce, GetDirection);
            _mStopState = new BirdStopState(this, transform, _mEntity, _mRigidBody, _mBirdStopBehaviour);
            _mCountState = new BirdCountState(this, transform, _mEntity, _mBirdCollide);
            _mCollideState = new BirdCollideState(this, transform, _mEntity, _mRigidBody);

            // switch (birdType)
            // {
            //     case BirdType.AttractBird:
            //         break;
            // }
            
            ChangeState(BirdState.BirdStateEnum.UnReady);
            Initialized = true;
        }

        private void Update()
        {
            CurrentState?.Update();
        }

        public void ChangeState(BirdState.BirdStateEnum newState)
        {
            CurrentState?.Exit();
            
            BirdState.BirdStateEnum? oldState = CurrentState?.State;
            
            switch (newState)
            {
                case BirdState.BirdStateEnum.UnReady:
                    CurrentState = _mUnReadyState;
                    break;
                case BirdState.BirdStateEnum.Ready:
                    CurrentState = _mReadyState;
                    break;
                case BirdState.BirdStateEnum.Shoot:
                    CurrentState = _mShootState;
                    break;
                case BirdState.BirdStateEnum.Stop:
                    CurrentState = _mStopState;
                    break;
                case BirdState.BirdStateEnum.Count:
                    CurrentState = _mCountState;
                    break;
                case BirdState.BirdStateEnum.Collided:
                    CurrentState = _mCollideState;
                    break;
                default: CurrentState = _mUnReadyState;
                    break;
            }

            CurrentState.FromStateEnum = oldState ?? BirdState.BirdStateEnum.UnReady;
            
            CurrentState.Enter();
        }

        public Vector3 GetDirection() => -arrowForceUI.transform.TransformDirection(arrowForceUI.transform.forward);
        private float GetShootForce() => _mShootForce;

        public void SetShootForce(float newForce)
        {
            _mShootForce = newForce;
        }
    }
}