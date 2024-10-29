using System;
using Events;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Image[] points;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private PlayerEnum playerEnum;

        [SerializeField] private Sprite circleEmpty;
        [SerializeField] private Sprite circleFill;

        private void Start()
        {
            ChaosBallApp.Interface.RegisterEvent<OnChangePlayerData>(e =>
            {
                UpdatePlayerUI(e.playerData[playerEnum].playerName, e.playerData[playerEnum].score, e.playerData[playerEnum].ballLeft);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void UpdatePlayerUI(string playerName, int score, int ballLeft)
        {
            nameText.text = playerName;
            scoreText.text = score.ToString();
            for (int i = points.Length - 1; i >= 0; i--)
            {
                points[i].sprite = ballLeft-- > 0 ? circleFill : circleEmpty;
            }
        }
    }
}