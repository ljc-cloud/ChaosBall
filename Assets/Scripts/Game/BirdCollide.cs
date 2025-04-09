using System;
using System.Linq;
using ChaosBall.Event.Game;
using ChaosBall.Game.State;
using UnityEngine;

namespace ChaosBall.Game
{
    public class BirdCollide : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float collideCostParam;
        [SerializeField] private float collideRotationSpeed = 5f;
        [SerializeField] private BirdType birdType;
        
        private Rigidbody _mRigidBody;
        private BirdStateMachine _mBirdStateMachine;

        private Vector3 _mCollideDirection;
        private float _mBirdRadius;
        private Collider[] _mAreaTriggerArray;
        private IBirdCollideBehaviour _mBirdCollideBehaviour;
        
        public class OnBirdEnterAreaEventArgs : EventArgs
        {
            public Area area;
        }
        public event EventHandler<OnBirdEnterAreaEventArgs> OnBirdEnterArea;
        public event EventHandler OnBirdExitArea;

        public Vector3 LastVelocity { get; private set; }
        
        private void Awake()
        {
            _mRigidBody = GetComponent<Rigidbody>();
            _mBirdStateMachine = GetComponent<BirdStateMachine>();
        }

        private void Start()
        {
            _mBirdRadius = GetComponent<SphereCollider>().radius;
            int size = Enum.GetValues(typeof(Area.AreaType)).Length;
            _mAreaTriggerArray = new Collider[size];

            switch (birdType)
            {
                case BirdType.NormalBird:
                    _mBirdCollideBehaviour = GetComponent<NormalBirdCollideBehaviour>();
                    break;
                case BirdType.AttachBird:
                    _mBirdCollideBehaviour = GetComponent<AttachBirdCollideBehaviour>();
                    break;
            }
        }

        private void Update()
        {
            if (Vector3.Angle(transform.forward, _mCollideDirection) > 15f)
            {
                transform.forward = Vector3.Lerp(transform.forward, _mCollideDirection, Time.deltaTime * collideRotationSpeed);
            }
        }

        private void LateUpdate()
        {
            if (!_mBirdStateMachine.Initialized) return;
            if (_mBirdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                LastVelocity = _mRigidBody.velocity;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            switch (other.gameObject.layer)
            {
                case GameAssets.WALL_LAYER:
                    // 反弹
                    ContactPoint contactPoint = other.GetContact(0);
                    Vector3 reflect = Vector3.Reflect(LastVelocity.normalized, contactPoint.normal);
                    var direction = reflect.normalized;
                    _mCollideDirection = direction;
                    _mRigidBody.velocity = direction * LastVelocity.magnitude * collideCostParam;
                    break;
                case GameAssets.BIRD_LAYER:
                    if (_mBirdStateMachine.CurrentState.State is BirdState.BirdStateEnum.Shoot)
                    {
                        other.transform.GetComponent<BirdStateMachine>().ChangeState(BirdState.BirdStateEnum.Collided);
                        _mBirdCollideBehaviour.OnCollideOtherBird(transform, other);
                    }
                    break;
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GameAssets.AREA_LAYER)
            {
                Area area = other.transform.GetComponent<Area>();
                // GameInterface.Interface.EventSystem.Publish(new BirdEnterAreaEvent
                // {
                //     area = area,
                // });
                OnBirdEnterArea?.Invoke(this, new OnBirdEnterAreaEventArgs { area = area });
            }
        }

        // private void OnTriggerStay(Collider other)
        // {
        //     if (_mBirdStateManager.Initialized && _mBirdStateManager.CurrentState.State is 
        //             (BirdState.BirdStateEnum.Stop or BirdState.BirdStateEnum.Count) && 
        //         other.gameObject.layer == GameAssets.AREA_LAYER)
        //     {
        //         float areaLayerDetectOffset = .5f;
        //         int size = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * _mBirdRadius
        //             , _mBirdRadius + areaLayerDetectOffset, _mAreaTriggerArray
        //             , 1 << GameAssets.AREA_LAYER, QueryTriggerInteraction.Collide);
        //         if (size == 0)
        //         {
        //             Debug.LogError("Not Collide With Area!");
        //             return;
        //         }
        //         Area[] areaArray = _mAreaTriggerArray.Select(item => item?.transform.GetComponent<Area>()).ToArray();
        //         
        //         Array.Sort(areaArray);
        //         Area correctArea = areaArray[^1];
        //         GameInterface.Interface.EventSystem.Publish(new BirdStayInAreaEvent { area = correctArea });
        //         // OnBirdStayInArea?.Invoke(correctArea);
        //     }
        // }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == GameAssets.AREA_LAYER)
            {
                // Area area = other.transform.GetComponent<Area>();
                // GameInterface.Interface.EventSystem.Publish(new BirdExitAreaEvent());
                OnBirdExitArea?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 16f, 16f);
        }
    }
}