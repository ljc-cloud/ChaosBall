using System;
using System.Collections;
using System.Linq;
using ChaosBall.Events;
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
        Crashed
    }

    public abstract class Ball : MonoBehaviour
    {
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected Transform arrow;
        [SerializeField] protected Image arrowImage;
        [SerializeField] protected float arrowClamp;
        [SerializeField] protected float maxLaunchForce;
        [SerializeField] protected float minLaunchForce;
        [SerializeField] protected float arrowRotationLerp;
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

        protected Rigidbody _rigidbody;
        
        private Vector2 _moveVector;

        private float _launchForce;

        private Quaternion _arrowInitialRotation;
        
        private bool _rotationFlag;

        private short _waitFrames = 5;

        private IPlayerInput _playerInput;

        protected PlayerEnum _belongPlayer;
        
        private float _scoreCountTimer = 1f;
        
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
                    BallMove();
                    ArrowRotation();
                    break;
                case BallState.Launched :
                    MakeSureBallStop();
                    break;
                case BallState.Stopped:
                    if (!ScoreCounted && !HasCrashed)
                    {
                        _scoreCountTimer -= Time.deltaTime;
                        if (_scoreCountTimer <= 0)
                        {
                            ScoreCounted = true;
                            ChaosBallApp.Interface.SendEvent<OnChangeRound>();
                            _scoreCountTimer = 1f;
                        }
                    }
                    break;
                case BallState.Crashed:
                    MakeSureBallStop();
                    break;
                case BallState.Scored:
                    MakeSureBallStop();
                    break;
            }
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
                CurrentBallState = CurrentBallState == BallState.Launched ? BallState.Stopped : BallState.Scored;
            }
        }
        
        private IEnumerator ReadyToLaunchBall()
        {
            CurrentBallState = BallState.ReadyToLaunch;
            var flag = true;
            while (CurrentBallState == BallState.ReadyToLaunch)
            {
                if (Mathf.Abs(arrowImage.fillAmount - 1f) < .05f)
                {
                    flag = false;
                }
                else if (arrowImage.fillAmount < .05f)
                {
                    flag = true;
                }
                arrowImage.fillAmount = flag? Mathf.Lerp(arrowImage.fillAmount, 1f, arrowRotationLerp)
                    : Mathf.Lerp(arrowImage.fillAmount, 0f, arrowRotationLerp);
                _launchForce = flag? Mathf.Lerp(_launchForce, maxLaunchForce, arrowRotationLerp)
                    : Mathf.Lerp(_launchForce, minLaunchForce, arrowRotationLerp);
                yield return null;
            }
        }

        #region Trigger And Collider

        private void OnTriggerStay(Collider other)
        {
            if (CurrentBallState == BallState.Stopped && !ScoreCounted && !HasCrashed)
            {
                if (other.GetComponentInParent<Area>().Type == AreaData.AreaType.OutSpace)
                {
                    Debug.Log("OutSpace, ChangeRound");
                    GameManager.Instance.ReTry();
                    ScoreCounted = true;
                    CurrentBallState = BallState.Scored;
                    Destroy(gameObject);
                    return;
                }

                CalculateAndIncreaseScore();
                // 切换投球
                ChaosBallApp.Interface.SendEvent<OnChangeRound>();
            } 
            else if (CurrentBallState == BallState.Scored && IsStopped && !ScoreCounted && HasCrashed)
            {
                CalculateAndIncreaseScore();
            }
        }

        private void CalculateAndIncreaseScore()
        {
            var areas = GetAreas();
            if (areas.Length == 0)
            {
                Debug.Log("Areas Empty, ChangeRound");
                ScoreCounted = true;
                CurrentBallState = BallState.Scored;
                ChaosBallApp.Interface.SendEvent<OnChangeRound>();
                return;
            }
            Array.Sort(areas);
            // Update Player Score
            var score = areas[areas.Length - 1].Score;
            GameManager.Instance.UpdatePlayerScore(_belongPlayer, BallIndex, score);
            ScoreCounted = true;
            CurrentBallState = BallState.Scored;
        }

        private Area[] GetAreas()
        {
            var capsuleCollider = GetComponent<CapsuleCollider>();
            var areas = Physics.CapsuleCastAll(transform.position, transform.position + capsuleCollider.height * Vector3.up
                , capsuleCollider.radius, Vector3.down,1f, areaLayer).Select(item => item.transform.GetComponentInParent<Area>()).ToArray();
            // Area[] areas = Physics.RaycastAll(transform.position + capsuleCollider.radius * Vector3.up,
            //         Vector3.down, 1f, areaLayer)
            //     .Select(item => item.transform.GetComponentInParent<Area>()).ToArray();
            return areas;
        }

        private void OnTriggerExit(Collider other)
        {
            if (CurrentBallState == BallState.Crashed && HasCrashed && !ScoreCounted)
            {
                if (other.transform.parent.TryGetComponent(out Area area))
                {
                    // Decrease Score
                    GameManager.Instance.UpdatePlayerScore(_belongPlayer, BallIndex, 0);
                    CurrentBallState = BallState.Scored;
                }
            }
            
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("BallCrash"))
            {
                GameManager.Instance.CreateParticle(transform.position);
                if (other.transform.CompareTag("Ball") && CurrentBallState is BallState.Scored or BallState.Crashed)
                {
                    CurrentBallState = BallState.Crashed;
                    HasCrashed = true;
                    ScoreCounted = false;
                    _waitFrames = 5;
                }
            }
           
        }

        #endregion
        
        private void LaunchBall()
        {
            if (CurrentBallState != BallState.ReadyToLaunch) return;
            Timer.Instance.PauseTimer();
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
        
    }
}