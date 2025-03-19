using System;
using ChaosBall.Inputs;
using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall.Game
{
    // TODO: 使用有限状态机实现Bird的各个状态
    [Obsolete]
    public class BirdShoot : MonoBehaviour
    {
        [SerializeField] private BirdAnimation birdAnimation;
        [SerializeField] private float maxShootForce;
        [SerializeField] private float minShootForce;
        [SerializeField] private float readyShootDuration;
        [SerializeField] private ArrowForceUI arrowForceUI;

        private Rigidbody _mRigidBody;

        private bool _mReady;
        private float _mReadyTimer;
        private bool _mReverse;

        public event Action<float> OnReadyToShoot;
        public event Action OnShoot;
        public event Action OnQuitShoot;

        private void Awake()
        {
            _mRigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ChaosBallInputRegister.Instance.OnPlayerReadyToShoot += PlayerReadyToShoot;
            ChaosBallInputRegister.Instance.OnPlayerShoot += PlayerShoot;
            ChaosBallInputRegister.Instance.OnPlayerQuitShoot += PlayerQuitShoot;
        }

        private void Update()
        {
            CalculateShootForce();
        }

        private void OnDestroy()
        {
            ChaosBallInputRegister.Instance.OnPlayerReadyToShoot -= PlayerReadyToShoot;
            ChaosBallInputRegister.Instance.OnPlayerShoot -= PlayerShoot;
            ChaosBallInputRegister.Instance.OnPlayerQuitShoot -= PlayerQuitShoot;
        }

        private void PlayerReadyToShoot()
        {
            OnReadyToShoot?.Invoke(readyShootDuration);
            birdAnimation.PlayReady();
            _mReady = true;
            Debug.Log("Player ReadyShoot!");
        }
        private void PlayerShoot()
        {
            OnShoot?.Invoke();
            birdAnimation.PlayShoot();
            _mReady = false;
            ShootBird();
            Debug.Log("Player Shoot!");
        }
        private void CalculateShootForce()
        {
            if (!_mReady) return;
            _mReadyTimer += Time.deltaTime;

            if (_mReadyTimer >= readyShootDuration)
            {
                _mReverse = !_mReverse;
                _mReadyTimer = 0f;
            }
            
            float force = _mReverse? Mathf.Lerp(maxShootForce, minShootForce, _mReadyTimer / readyShootDuration)
                : Mathf.Lerp(minShootForce, maxShootForce, _mReadyTimer / readyShootDuration);
        }
        private void ShootBird()
        {
            if (_mReady) return;
            // 1.获取方向
            Vector3 direction = arrowForceUI.transform.TransformDirection(arrowForceUI.transform.forward);
            // 2.获取已经计算的力
            Debug.Log("direction:" + direction);
            // 3.发射
        }

        private void PlayerQuitShoot()
        {
            OnQuitShoot?.Invoke();
            birdAnimation.PlayQuitShoot();
            _mReady = false;
            Debug.Log("Player Quit Shoot!");
        }
    }
}