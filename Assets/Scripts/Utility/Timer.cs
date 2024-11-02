using System;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.Utility
{
    public class Timer : MonoSingleton<Timer>
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Slider timeSlider;
        [SerializeField] private float timeLimit;
        
        public event Action OnTimeComplete;
        
        private bool _isStart;
        private bool _isDone;
    
        private float _currentTime;
        
        private void Start()
        {
            _currentTime = timeLimit;
            _isStart = false;
            _isDone = false;
        }
    
        private void Update()
        {
            if (!_isStart) return;
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                timeText.text = Mathf.CeilToInt(_currentTime).ToString();
                var normalize = _currentTime / timeLimit;
                timeSlider.value = normalize;
            }
            else
            {
                if (!_isDone)
                {
                    _isDone = true;
                    _isStart = false;
                    OnTimeComplete?.Invoke();
                }
            }
        }
    
        public void StartTimer() => _isStart = true;
    
        public void PauseTimer() => _isStart = false;
    
        public void ReStartTimer()
        {
            _currentTime = timeLimit;
            _isStart = true;
            _isDone = false;
        }
    
        public void ResetTimer()
        {
            _currentTime = timeLimit;
            _isStart = false;
            _isDone = false;
        }
    
    }

}
