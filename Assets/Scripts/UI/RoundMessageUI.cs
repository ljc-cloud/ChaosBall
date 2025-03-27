using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class RoundMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI messageText;

        private CanvasGroup _mCanvasGroup;
        
        private void Awake()
        {
            _mCanvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            GameManager.Instance.OnRoundMessaging += OnRoundMessaging;
            transform.position.Set(1200, 0, 0);
            _mCanvasGroup.alpha = 0;
            _mCanvasGroup.blocksRaycasts = false;
        }

        private void OnRoundMessaging(int currentRound, string message)
        {
            roundText.text = $"回合{currentRound}/4";
            messageText.text = message;
            DoMoveFadeIn();
            Invoke(nameof(DoMoveFadeOut), 1f);
        }

        private void DoMoveFadeIn()
        {
            transform.DOMoveX(0, .3f);
            _mCanvasGroup.DOFade(1, .3f).OnComplete(() =>
            {
                _mCanvasGroup.blocksRaycasts = true;
            });
        }

        private void DoMoveFadeOut()
        {
            transform.DOMoveX(-1200, .3f);
            _mCanvasGroup.DOFade(0, .3f).OnComplete(() =>
            {
                _mCanvasGroup.blocksRaycasts = false;
                transform.position.Set(1200, 0, 0);
            });
        }
    }
}