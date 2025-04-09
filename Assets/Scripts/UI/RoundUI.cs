using System.Collections.Generic;
using ChaosBall.Event.Game;
using ChaosBall.Model;
using ChaosBall.Net;
using ChaosBall.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class RoundUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private PlayerRoundScoreUI localPlayerRoundScoreUI;
        [SerializeField] private PlayerRoundScoreUI remotePlayerRoundScoreUI;

        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        private void Start()
        {
            GameInterface.Interface.EventSystem.Subscribe<RoundChangeEvent>(OnRoundChanged);
            transform.DeActive();
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<RoundChangeEvent>(OnRoundChanged);
        }

        private void OnRoundChanged(RoundChangeEvent e)
        {
            roundText.text = $"第{e.currentRound}回合得分";
            Dictionary<Entity.PlayerType,PlayerScoreBoard> playerTypeToPlayerScoreBoardDict 
                = GameManager.Instance.GetPlayerScoreBoard();
            localPlayerRoundScoreUI.SetPlayerRoundScoreUI(playerTypeToPlayerScoreBoardDict[Entity.PlayerType.Local]);
            remotePlayerRoundScoreUI.SetPlayerRoundScoreUI(playerTypeToPlayerScoreBoardDict[Entity.PlayerType.Remote]);
            transform.Active();
            DoMoveFadeIn();
            Invoke(nameof(DoMoveFadeOut), 1f);
        }
        private void DoMoveFadeIn()
        {
            transform.DOMoveX(0, .3f);
            _canvasGroup.DOFade(1, .3f).OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = true;
            });
        }

        private void DoMoveFadeOut()
        {
            transform.DOMoveX(-1200, .3f);
            _canvasGroup.DOFade(0, .3f).OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = false;
                transform.position.Set(1200, 0, 0);
            });
        }
    }
}