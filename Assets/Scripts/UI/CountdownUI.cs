using ChaosBall.Event.Game;
using ChaosBall.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class CountdownUI : MonoBehaviour
    {
        private TextMeshProUGUI _mCountdownText;

        private CanvasGroup _mCanvasGroup;
        
        private void Awake()
        {
            _mCountdownText = GetComponent<TextMeshProUGUI>();
            _mCanvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            GameInterface.Interface.EventSystem.Subscribe<CountdownChangeEvent>(OnCountdown);
            // GameManager.Instance.OnCountdownChanged += OnCountdown;
            GameInterface.Interface.EventSystem.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            // GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            // _mCanvasGroup.alpha = 0;
            // _mCanvasGroup.blocksRaycasts = false;
            transform.DeActive();
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<CountdownChangeEvent>(OnCountdown);
            GameInterface.Interface.EventSystem.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            if (e.state is GameState.CountdownToStart)
            {
                Debug.Log("Countdown active");
                Invoker.Instance.DelegateList.Add(() =>
                {
                    transform.Active();
                });
                // transform.gameObject.SetActive(true);
                // transform.Active();
            }
        }

        private void OnCountdown(CountdownChangeEvent e)
        {
            Debug.Log($"Countdown:{e.countdown}");
            _mCountdownText.text = e.countdown.ToString();
            transform.DOShakeRotation(0.4f);
            if (e.countdown <= 1)
            {
                _mCanvasGroup.alpha = 0;
                _mCanvasGroup.blocksRaycasts = false;
            }
        }
    }
}