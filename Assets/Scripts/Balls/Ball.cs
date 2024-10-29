using System;
using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Balls
{
    public enum BallState
    {
        UnLaunched,
        ReadyToLaunch,
        Launched,
        Stopped,
        Scored
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
        

        protected Rigidbody _rigidbody;
        
        private Vector2 _moveVector;

        private float _launchForce;

        private Quaternion _arrowInitialRotation;
        
        private bool _rotationFlag;

        private short _waitFrames = 5;

        private IPlayerInput _playerInput;

        protected PlayerEnum _belongPlayer;
        
        private bool _scoreCounted;
        
        private float _scoreCountTimer = 1f;
        
        public BallState CurrentBallState { get; protected set; }
        
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
                    print($"Velocity: {Math.Round(_rigidbody.velocity.sqrMagnitude, 2)}");
                    break;
                case BallState.Stopped:
                    if (!_scoreCounted)
                    {
                        _scoreCountTimer -= Time.deltaTime;
                        if (_scoreCountTimer <= 0)
                        {
                            _scoreCounted = true;
                            ChaosBallApp.Interface.SendEvent<OnChangeRound>();
                            _scoreCountTimer = 1f;
                        }
                    }
                    break;
            }
        }

        public void SetPlayerInput(IPlayerInput playerInput)
        {
            _playerInput = playerInput;
            _playerInput.OnLRMove += vector2 => { _moveVector = vector2; };
            _playerInput.OnReadyToLaunch += () => { StartCoroutine(ReadyToLaunchBall()); };
            _playerInput.OnLaunch += LaunchBall;
        }

        private void MakeSureBallStop()
        {
            if (_waitFrames > 0)
            {
                _waitFrames--;
                return;
            }
            if (_rigidbody.velocity.sqrMagnitude < .5f)
            {
                _rigidbody.velocity = Vector3.zero;
                CurrentBallState = BallState.Stopped;
                print("Ball Stopped!");
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
        
        private void OnTriggerStay(Collider other)
        {
            if (CurrentBallState != BallState.Stopped || _scoreCounted) return;
            print($"Score, {other.GetComponentInParent<Area>().gameObject.name}!");

            if (other.GetComponentInParent<Area>().Type == AreaData.AreaType.OutSpace)
            {
                GameManager.Instance.ReTry();
                _scoreCounted = true;
                CurrentBallState = BallState.Scored;
                Destroy(gameObject);
                return;
            }

            // Update Player Score
            var score = other.GetComponentInParent<Area>().Score;
            GameManager.Instance.UpdatePlayerScore(_belongPlayer, score + GameManager.Instance.GetPlayerScore(_belongPlayer));
            _scoreCounted = true;
            CurrentBallState = BallState.Scored;
            // 切换回合
            ChaosBallApp.Interface.SendEvent(new OnChangeRound { keepRound = false});
        }

        private void LaunchBall()
        {
            arrow.gameObject.SetActive(false);
            StopCoroutine(nameof(ReadyToLaunchBall));
            _playerInput.DisableInput();
            _rigidbody.AddForce(arrow.forward * _launchForce, ForceMode.Impulse);
            CurrentBallState = BallState.Launched;
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
    }
}