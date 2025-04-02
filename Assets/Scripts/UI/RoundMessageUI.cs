using System;
using ChaosBall.Event.Game;
using ChaosBall.Utility;
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
            GameInterface.Interface.EventSystem.Subscribe<RoundMessageEvent>(OnRoundMessaging);
            transform.DeActive();
            transform.localPosition.Set(1200, 0, 0);
            _mCanvasGroup.alpha = 0;
            _mCanvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<RoundMessageEvent>(OnRoundMessaging);
        }

        private void OnRoundMessaging(RoundMessageEvent e)
        {
            roundText.text = $"回合{e.currentRound}/4";
            messageText.text = e.message;
            transform.Active();
            DoMoveFadeIn();
            Invoke(nameof(DoMoveFadeOut), 1f);
        }

        private void DoMoveFadeIn()
        {
            transform.DOLocalMoveX(0, .3f);
            _mCanvasGroup.DOFade(1, .3f).OnComplete(() =>
            {
                _mCanvasGroup.blocksRaycasts = true;
            });
        }

        private void DoMoveFadeOut()
        {
            transform.DOLocalMoveX(-1200, .3f);
            _mCanvasGroup.DOFade(0, .3f).OnComplete(() =>
            {
                _mCanvasGroup.blocksRaycasts = false;
                transform.position.Set(1200, 0, 0);
            });
        }
    }
}