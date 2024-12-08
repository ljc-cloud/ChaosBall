using System;
using System.Collections;
using System.Linq;
using ChaosBall.Game;
using ChaosBall.Manager;
using ChaosBall.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.Balls
{
    public abstract class Ball : MonoBehaviour, IBall
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Transform arrow;
        [SerializeField] private Image arrowImage;
        [SerializeField] private float arrowClamp;
        [SerializeField] private float arrowRotationRatio;
        [SerializeField] private float maxLaunchForce;
        [SerializeField] private float minLaunchForce;
        [SerializeField] private float ballBounceRatio = .3f;
        [SerializeField] private float wallBounceRatio = .8f;
        [SerializeField] private float launchToMaxForceDuration;
        [SerializeField] private LayerMask areaLayer;
        
        [field:SerializeField]
        public bool ScoreCounted { get; protected set; }
        
        [field: SerializeField]
        public BallState CurrentBallState { get; protected set; }

        [field: SerializeField] public bool HasCollide { get; protected set; } = false;

        [field:SerializeField]
        public int BallIndex { get; private set; }

        public bool IsAttached => transform.parent != null;

        public bool IsStopped => _rigidbody.velocity == Vector3.zero;
        
        public Ball EffectBall { get; set; }
        
        public event Action<int> OnBallOutSpace;
        // belongPlayer, BallIndex, Score 
        public event Action<PlayerEnum, int, int> OnBallIncreaseScore;
        // belongPlayer, BallIndex
        public event Action<PlayerEnum, int> OnBallScoreReset;
        public event Action OnBallScoreCounted;

        protected Rigidbody _rigidbody;
        protected CapsuleCollider _collider;
        protected short _waitFrames = 10;
        protected PlayerEnum _belongPlayer;
        protected Vector3 _lastVelocity;
        protected Vector3 _beforeBallCollideLastVelocity;
        // protected Ball _effectBall;
        
        private Vector2 _moveVector;
        private float _launchForce;
        private Quaternion _arrowInitialRotation;
        private bool _rotationFlag;
        private IPlayerInput _playerInput;
        private float _scoreCountTimer = 1f;
        
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
        }

        protected virtual void Start()
        {
            _rotationFlag = true;
            CurrentBallState = BallState.UnLaunched;
            _arrowInitialRotation = arrow.localRotation;
        }

        protected virtual void Update()
        {
            switch (CurrentBallState)
            {
                case BallState.UnLaunched :
                    OnBallUnLaunched();
                    break;
                case BallState.Launched :
                    OnBallLaunched();
                    break;
                case BallState.Stopped:
                    OnBallStopped();
                    break;
                case BallState.Scored:
                    OnBallScored();
                    break;
                case BallState.Effected:
                    OnBallEffected();
                    break;
            }
        }

        #region BallLifeTime

        protected virtual void OnBallUnLaunched()
        {
            BallMove();
            ArrowRotation();
        }

        protected virtual void OnBallLaunched()
        {
            MakeSureBallStop();
        }
        protected virtual void OnBallStopped()
        {
            print("Ball Stopped");
        }

        protected virtual void OnBallScored()
        {
            // MakeSureBallStop();
        }

        protected virtual void OnBallEffected()
        {
            if (EffectBall != null)
            {
                EffectBall.ProcessEffect(this);
            }
            MakeSureBallStop();
        }

        #endregion
        

        public void SetPlayerInput(IPlayerInput playerInput)
        {
            _playerInput = playerInput;
            _playerInput.OnLRMove += vector2 => { _moveVector = vector2; };
            _playerInput.OnReadyToLaunch += () => { StartCoroutine(ReadyToLaunchBall()); };
            _playerInput.OnLaunch += LaunchBall;
            _playerInput.OnUnLaunch += CancelLaunchBall;
        }
        
        private void MakeSureBallStop()
        {
            if (_waitFrames > 0)
            {
                _waitFrames--;
                return;
            }
            if (_rigidbody.velocity.sqrMagnitude < 1f)
            {
                _rigidbody.velocity = Vector3.zero;
                if (CurrentBallState is BallState.Launched)
                {
                    CurrentBallState = BallState.Stopped;
                }
                // CurrentBallState = CurrentBallState is BallState.Launched ? BallState.Stopped : CurrentBallState;
                print($"{CurrentBallState}");
            }
        }
        
        private IEnumerator ReadyToLaunchBall()
        {
            Timer.Instance.PauseTimer();
            CurrentBallState = BallState.ReadyToLaunch;
            var flag = true;
            var currentTime = 0f;
            arrowImage.fillAmount = 0f;
            while (CurrentBallState == BallState.ReadyToLaunch)
            {
                if (currentTime >= launchToMaxForceDuration)
                {
                    flag = !flag;
                }
                currentTime = currentTime >= launchToMaxForceDuration ? 0 : currentTime + Time.deltaTime;
                arrowImage.fillAmount = flag? Mathf.Lerp(0f, 1f, currentTime / launchToMaxForceDuration)
                    : Mathf.Lerp(1f, 0f, currentTime / launchToMaxForceDuration);
                _launchForce = flag? Mathf.Lerp(minLaunchForce, maxLaunchForce, currentTime / launchToMaxForceDuration)
                    : Mathf.Lerp(maxLaunchForce, minLaunchForce, currentTime / launchToMaxForceDuration);
                yield return null;
            }
        }

        #region Trigger And Collider

        private void OnTriggerStay(Collider other)
        {
            if (CurrentBallState is BallState.Stopped && !ScoreCounted && !IsAttached)
            {
                if (other.GetComponentInParent<Area>().Type == AreaData.AreaType.OutSpace)
                {
                    var index = BallManager.Instance.IndexAt(this);
                    OnBallOutSpace?.Invoke(index);
                    return;
                }

                CalculateAndIncreaseScore();
            } 
            else if (CurrentBallState == BallState.Effected && IsStopped && !ScoreCounted && !IsAttached)
            {
                CalculateAndIncreaseScore();
            }
        }

        private void CalculateAndIncreaseScore()
        {
            print("OnTriggerStay");
            var areas = GetAreas();
            Array.Sort(areas);
            // Update Player Score
            var score = areas[areas.Length - 1].Score;
            OnBallIncreaseScore?.Invoke(_belongPlayer, BallIndex, score);
            ScoreCounted = true;
            CurrentBallState = BallState.Scored;
            OnBallScoreCounted?.Invoke();
        }

        private Area[] GetAreas()
        {
            var capsuleCollider = GetComponent<CapsuleCollider>();
            // var areas = Physics.CapsuleCastAll(transform.position, transform.position + capsuleCollider.height * Vector3.up
            //     , capsuleCollider.radius, Vector3.down,1f, areaLayer).Select(item => item.transform.GetComponentInParent<Area>()).ToArray();
            Area[] areas = Physics.RaycastAll(transform.position + capsuleCollider.radius * Vector3.up,
                    Vector3.down, 2f, areaLayer)
                .Select(item => item.transform.GetComponentInParent<Area>()).ToArray();
            return areas;
        }

        private void OnTriggerExit(Collider other)
        {
            if (CurrentBallState == BallState.Effected && !ScoreCounted)
            {
                if (other.transform.parent.TryGetComponent(out Area _))
                {
                    OnBallScoreReset?.Invoke(_belongPlayer, BallIndex);
                }
            }
        }

        private void LateUpdate()
        {
            _lastVelocity = _rigidbody.velocity;
        }

        private void FixedUpdate()
        {
            if (!HasCollide)
            {
                _beforeBallCollideLastVelocity = _rigidbody.velocity;
            }
            Debug.DrawRay(transform.position, _beforeBallCollideLastVelocity.normalized * 100f, Color.red);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("BallCrash"))
            {
                GameManager.Instance.CreateParticle(transform.position);
                if (other.transform.CompareTag("Wall"))
                {
                    // 修改球的角度
                    Vector3 reflectAngle = Vector3.Reflect(_lastVelocity, other.GetContact(0).normal);
                    _rigidbody.velocity = reflectAngle.normalized * _lastVelocity.magnitude * wallBounceRatio;
                }
                if (other.transform.CompareTag("Ball"))
                {
                    HasCollide = true;
                    var otherBall = other.transform.GetComponent<Ball>();
                    if (CurrentBallState is BallState.Scored && otherBall.CurrentBallState is BallState.Launched)
                    {
                        SetBallEffected();
                        if (otherBall is AttachBall)
                        {
                            otherBall.EffectOtherBall(this);
                        }
                    }
                    else if (CurrentBallState is BallState.Scored && otherBall.CurrentBallState is BallState.Effected)
                    {
                        SetBallEffected();
                    }
                }
            }
        }

        // protected abstract void EffectOtherBall(Ball other);
        public virtual void EffectOtherBall(Ball other)
        {
            throw new NotImplementedException();
        }

        public virtual void ProcessEffect(Ball other)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void SetBallEffected()
        {
            CurrentBallState = BallState.Effected;
            ScoreCounted = false;
            _waitFrames = 10;
        }

        private void LaunchBall()
        {
            if (CurrentBallState != BallState.ReadyToLaunch) return;
            arrow.gameObject.SetActive(false);
            StopCoroutine(nameof(ReadyToLaunchBall));
            _playerInput.DisableInput();
            _rigidbody.AddForce(arrow.forward * _launchForce, ForceMode.Impulse);
            CurrentBallState = BallState.Launched;
        }
        
        private void CancelLaunchBall()
        {
            if (CurrentBallState != BallState.ReadyToLaunch) return;
            Timer.Instance.StartTimer();
            arrow.gameObject.SetActive(true);
            arrowImage.fillAmount = 1;
            _playerInput.EnableInput();
            CurrentBallState = BallState.UnLaunched;
        }
        
        private void BallMove()
        {
            if (CurrentBallState != BallState.UnLaunched)
            {
                return;
            }
            transform.Translate(new Vector3(_moveVector.x, 0, _moveVector.y) * (moveSpeed * Time.deltaTime));
        }

        private void ArrowRotation()
        {
            if (CurrentBallState != BallState.UnLaunched)
            {
                return;
            }
            if (Quaternion.Angle(arrow.localRotation, _arrowInitialRotation) >= arrowClamp)
            {
                _rotationFlag = !_rotationFlag;
            }
            arrow.Rotate((_rotationFlag ? Vector3.up : -Vector3.up) * arrowRotationRatio, Space.Self);
        }

        public void SetPlayerBelong(PlayerEnum player)
        {
            _belongPlayer = player;
        }

        public PlayerEnum GetPlayerBelong() => _belongPlayer;

        public void SetBallIndex(int index) => BallIndex = index;

        public void MakeBallStop()
        {
            CurrentBallState = BallState.Stopped;
            _rigidbody.velocity = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            var capsuleCollider = GetComponent<CapsuleCollider>();
            Gizmos.DrawLine(transform.position, transform.position +capsuleCollider.height * Vector3.up);
        }

        public void EnableInput()
        {
            _playerInput.EnableInput();
        }

        public void DisableInput()
        {
            _playerInput.DisableInput();
        }

    }
}