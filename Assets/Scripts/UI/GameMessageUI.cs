using ChaosBall.Event.Game;
using ChaosBall.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class GameMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;

        private CanvasGroup _mCanvasGroup;
        
        private void Awake()
        {
            _mCanvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            GameInterface.Interface.EventSystem.Subscribe<GameMessageEvent>(OnGameMessage);
            transform.DeActive();
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<GameMessageEvent>(OnGameMessage);
        }
        
        private void OnGameMessage(GameMessageEvent e)
        {
            messageText.text = e.message;
            transform.Active();
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