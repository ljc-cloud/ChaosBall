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
            transform.DOLocalMoveX(0, .3f);
            _mCanvasGroup.DOFade(1, .3f);
        }

        private void DoMoveFadeOut()
        {
            transform.DOLocalMoveX(-1200, .3f);
            _mCanvasGroup.DOFade(0, .3f);
        }
    }
}