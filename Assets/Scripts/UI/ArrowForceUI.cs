using ChaosBall.Game;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class ArrowForceUI : MonoBehaviour
    {
        [SerializeField] private Image forceArrowImage;
        [SerializeField] private Color forceMaxColor;
        [SerializeField] private Color forceMinColor;
        [SerializeField] private float minForceArrowRotation;
        [SerializeField] private float maxForceArrowRotation;
        [SerializeField] private float forceArrowRotationDuration;
        [SerializeField] private float forceArrowReadyDuration;

        private bool _mReady;

        private float _mReadyTimer;
        private float _mRotationTimer;
        private bool _mForceArrowReadyReverse;
        private bool _mForceArrowRotationReverse;
        private Quaternion _mMinRotation;
        private Quaternion _mMaxRotation;
        
        private void Start()
        {
            forceArrowImage.fillAmount = 1f;
            _mForceArrowReadyReverse = false;
            _mForceArrowRotationReverse = false;
            
            Vector3 minEulerAngle = new Vector3(0, 0, minForceArrowRotation);
            Vector3 maxEulerAngle = new Vector3(0, 0, maxForceArrowRotation);
            _mMinRotation = Quaternion.Euler(minEulerAngle);
            _mMaxRotation = Quaternion.Euler(maxEulerAngle);
        }
        
        private void Update()
        {
            if (_mReady)
            {
                ForceArrowReady();
            }
            else
            {
                LerpForceArrowDirection();
            }
        }

        private void ForceArrowReady()
        {
            // 更新计时器
            _mReadyTimer += Time.deltaTime;

            if (_mReadyTimer >= forceArrowReadyDuration)
            {
                _mForceArrowReadyReverse = !_mForceArrowReadyReverse;
                _mReadyTimer = 0f;
            }
            
            forceArrowImage.fillAmount = _mForceArrowReadyReverse
                ? Mathf.Lerp(1f, 0f, _mReadyTimer / forceArrowReadyDuration)
                : Mathf.Lerp(0f, 1f, _mReadyTimer / forceArrowReadyDuration);
            forceArrowImage.color = _mForceArrowReadyReverse
                ? Color.Lerp(forceMaxColor, forceMinColor, _mReadyTimer / forceArrowReadyDuration)
                : Color.Lerp(forceMinColor, forceMaxColor, _mReadyTimer / forceArrowReadyDuration);
        }
        private void LerpForceArrowDirection()
        {
            _mRotationTimer += Time.deltaTime;
            if (_mRotationTimer >= forceArrowRotationDuration)
            {
                _mForceArrowRotationReverse = !_mForceArrowRotationReverse;
                _mRotationTimer = 0f;
            }
            transform.localRotation = _mForceArrowRotationReverse
                ? Quaternion.Slerp(_mMaxRotation, _mMinRotation, _mRotationTimer / forceArrowRotationDuration)
                : Quaternion.Slerp(_mMinRotation, _mMaxRotation, _mRotationTimer / forceArrowRotationDuration);
        }

        public void SetReady(bool ready)
        {
            _mReady = ready;
            if (!ready)
            {
                _mForceArrowReadyReverse = false;
                _mReadyTimer = 0f;
                forceArrowImage.fillAmount = 1f;
                forceArrowImage.color = forceMinColor;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}