using System;
using System.Collections.Generic;
using ChaosBall.Model;
using ChaosBall.Net;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class RoundUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private PlayerRoundScoreUI localPlayerRoundScoreUI;
        [SerializeField] private PlayerRoundScoreUI remotePlayerRoundScoreUI;

        private void Start()
        {
            GameManager.Instance.OnRoundChanged += OnRoundChanged;
        }

        private void OnRoundChanged(int currentRound)
        {
            // TODO: ui animation display
            Dictionary<Entity.PlayerType,PlayerScoreBoard> playerTypeToPlayerScoreBoardDict 
                = GameManager.Instance.GetPlayerScoreBoard();
            localPlayerRoundScoreUI.SetPlayerRoundScoreUI(playerTypeToPlayerScoreBoardDict[Entity.PlayerType.Local]);
            remotePlayerRoundScoreUI.SetPlayerRoundScoreUI(playerTypeToPlayerScoreBoardDict[Entity.PlayerType.Remote]);
        }
    }
}