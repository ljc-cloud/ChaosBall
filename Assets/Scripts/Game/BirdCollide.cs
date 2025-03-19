using System;
using System.Linq;
using ChaosBall.Game.State;
using Unity.VisualScripting;
using UnityEngine;

namespace ChaosBall.Game
{
    public class BirdCollide : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float collideCostParam;
        [SerializeField] private float collideRotationSpeed = 5f;
        [SerializeField] private BirdType birdType;
        
        private Rigidbody _mRigidBody;
        private BirdStateManager _mBirdStateManager;

        private Vector3 _mCollideDirection;
        private float _mBirdRadius;
        private Collider[] _mAreaTriggerArray;
        private IBirdCollideBehaviour _mBirdCollideBehaviour;

        public Vector3 LastVelocity { get; private set; }
        public event Action<Area> OnBirdStayInArea;
        public event Action<Collision> OnCollideOtherBird;
        
        private void Awake()
        {
            _mRigidBody = GetComponent<Rigidbody>();
            _mBirdStateManager = GetComponent<BirdStateManager>();
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
            if (!_mBirdStateManager.Initialized) return;
            if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Shoot)
            {
                LastVelocity = _mRigidBody.velocity;
            }
            else if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Stop)
            {
                // LastVelocity = Vector3.zero;
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
                    if (_mBirdStateManager.CurrentState.State is BirdState.BirdStateEnum.Shoot)
                    {
                        other.transform.GetComponent<BirdStateManager>().ChangeState(BirdState.BirdStateEnum.Collided);
                        _mBirdCollideBehaviour.OnCollideOtherBird(transform, other);
                    }
                    break;
            }
            
        }

        private void OnTriggerStay(Collider other)
        {
            if (_mBirdStateManager.Initialized && _mBirdStateManager.CurrentState.State is 
                    (BirdState.BirdStateEnum.Stop or BirdState.BirdStateEnum.Count) && 
                other.gameObject.layer == GameAssets.AREA_LAYER)
            {
                float areaLayerDetectOffset = .5f;
                int size = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * _mBirdRadius
                    , _mBirdRadius + areaLayerDetectOffset, _mAreaTriggerArray
                    , 1 << GameAssets.AREA_LAYER, QueryTriggerInteraction.Collide);
                if (size == 0)
                {
                    Debug.LogError("Not Collide With Area!");
                    return;
                }
                Area[] areaArray = _mAreaTriggerArray.Select(item => item?.transform.GetComponent<Area>()).ToArray();
                
                Array.Sort(areaArray);
                Area correctArea = areaArray[^1];
                OnBirdStayInArea?.Invoke(correctArea);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 16f, 16f);
        }
    }
}