using System.Collections.ObjectModel;
using ChaosBall.Manager;
using ChaosBall.Model;
using ChaosBall.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Image[] points;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private PlayerEnum playerEnum;

        [SerializeField] private Sprite circleEmpty;
        [SerializeField] private Sprite circleFill;

        private void Awake()
        {
            GameManager.Instance.OnChangePlayerData += UpdatePlayerUI;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnChangePlayerData -= UpdatePlayerUI;
        }

        private void UpdatePlayerUI(ReadOnlyDictionary<PlayerEnum, PlayerModel> playerData)
        {
            Debug.Log("Update Player UI");
            print($"playerData: {playerData[playerEnum]}");
            nameText.text = playerData[playerEnum].playerName;
            scoreText.text = playerData[playerEnum].score.ToString();
            var ballLeft = playerData[playerEnum].ballLeft;
            for (int i = points.Length - 1; i >= 0; i--)
            {
                points[i].sprite = ballLeft-- > 0 ? circleFill : circleEmpty;
            }
        }
    }
}