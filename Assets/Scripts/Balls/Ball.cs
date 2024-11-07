using System;
using System.Collections;
using System.Linq;
using ChaosBall.Game;
using ChaosBall.Manager;
using ChaosBall.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ChaosBall.Balls
{
    public enum BallState
    {
        UnLaunched,
        ReadyToLaunch,
        Launched,
        Stopped,
        Scored,
        Effected,
    }

    /// <summary>
    /// 一般的小球，似乎没什么特点
    /// </summary>
    public abstract class Ball : MonoBehaviour
    {
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected Transform arrow;
        [SerializeField] protected Image arrowImage;
        [SerializeField] protected float arrowClamp;
        [SerializeField] protected float maxLaunchForce;
        [SerializeField] protected float minLaunchForce;
        [SerializeField] protected float ballBounceRatio;
        [SerializeField] protected float launchToMaxForceDuration;
        [SerializeField] protected LayerMask areaLayer;
        
        [field:SerializeField]
        public bool ScoreCounted { get; protected set; }
        
        [field: SerializeField]
        public BallState CurrentBallState { get; protected set; }

        [field: SerializeField]
        public bool HasCrashed { get; protected set; } = false;

        [field:SerializeField]
        public int BallIndex { get; protected set; }

        public bool IsStopped => _rigidbody.velocity == Vector3.zero;

        public event Action OnBallOutSpace;
        
        // belongPlayer, BallIndex, Score 
        public event Action<PlayerEnum, int, int> OnBallIncreaseScore;
        // belongPlayer, BallIndex
        public event Action<PlayerEnum, int> OnBallScoreReset;

        public event Action OnBallScoreCounted;

        protected Rigidbody _rigidbody;
        
        private Vector2 _moveVector;

        private float _launchForce;

        private Quaternion _arrowInitialRotation;
        
        private bool _rotationFlag;

        private short _waitFrames = 10;

        private IPlayerInput _playerInput;

        protected PlayerEnum _belongPlayer;
        
        private float _scoreCountTimer = 1f;

        private Vector3 _lastDir;
        
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
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
            
        }

        protected virtual void OnBallScored()
        {
            MakeSureBallStop();
        }

        protected virtual void OnBallEffected()
        {
            MakeSureBallStop();
        }

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
                CurrentBallState = CurrentBallState == BallState.Launched ? BallState.Stopped : CurrentBallState;
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
            if (CurrentBallState == BallState.Stopped && !ScoreCounted) // && !HasCrashed
            {
                if (other.GetComponentInParent<Area>().Type == AreaData.AreaType.OutSpace)
                {
                    OnBallOutSpace?.Invoke();
                    // ScoreCounted = true;
                    // CurrentBallState = BallState.Scored;
                    BallManager.Instance.RemoveBall(this);
                    Destroy(gameObject);
                    return;
                }

                CalculateAndIncreaseScore();
                // 切换投球
                // ChaosBallApp.Interface.SendEvent<OnChangeRound>();
            } 
            else if (CurrentBallState == BallState.Effected && IsStopped && !ScoreCounted)
            {
                CalculateAndIncreaseScore();
            }
        }

        private void CalculateAndIncreaseScore()
        {
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
                    // Decrease Score
                    OnBallScoreReset?.Invoke(_belongPlayer, BallIndex);
                    // CurrentBallState = BallState.Scored;
                }
            }
        }

        private void LateUpdate()
        {
            _lastDir = _rigidbody.velocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("BallCrash"))
            {
                GameManager.Instance.CreateParticle(transform.position);
                if (other.transform.CompareTag("Ball") && CurrentBallState is BallState.Scored)
                {
                    OnCollideOtherBall(other);
                }
                if (other.transform.CompareTag("Wall"))
                {
                    // 修改球的角度
                    Vector3 reflectAngle = Vector3.Reflect(_lastDir, other.GetContact(0).normal);
                    _rigidbody.velocity = reflectAngle.normalized * _lastDir.magnitude * ballBounceRatio;
                }
            }
        }

        protected virtual void OnCollideOtherBall(Collision other)
        {
            if (other.transform.GetComponent<Ball>() is NormalBall)
            {
                CurrentBallState = BallState.Effected;
                ScoreCounted = false;
                _waitFrames = 10;
            }
            else if (other.transform.GetComponent<Ball>() is AttachBall)
            {
                transform.SetParent(other.transform);
                CurrentBallState = BallState.Effected;
                ScoreCounted = false;
                _waitFrames = 10;
            }
        }

        #endregion
        
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
            arrow.Rotate((_rotationFlag ? Vector3.up : -Vector3.up) * .5f, Space.Self);
        }

        public void SetPlayerBelong(PlayerEnum player)
        {
            _belongPlayer = player;
        }

        public PlayerEnum GetPlayerBelong() => _belongPlayer;

        public void SetBallIndex(int index) => BallIndex = index;

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