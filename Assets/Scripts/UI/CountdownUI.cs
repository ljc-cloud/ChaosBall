using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class CountdownUI : MonoBehaviour
    {
        private TextMeshProUGUI _mCountdownText;
        
        private void Awake()
        {
            _mCountdownText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            GameManager.Instance.OnCountdownChanged += OnCountdown;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            gameObject.SetActive(false);
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state is GameState.CountdownToStart) gameObject.SetActive(true);
        }

        private void OnCountdown(int countdown)
        {
            _mCountdownText.text = countdown.ToString();
            transform.DOShakeRotation(0.4f);
        }
    }
}