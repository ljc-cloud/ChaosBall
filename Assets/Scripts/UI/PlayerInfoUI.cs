using ChaosBall.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class PlayerInfoUI : MonoBehaviour
    {
        [SerializeField] private Image crownImage;
        [SerializeField] private Text nameText;
        [SerializeField] private TextMeshProUGUI[] scorePoints;
        [SerializeField] private TextMeshProUGUI scoreText;
        

        public void SetData(bool isWinner, string playerName, int[] scores, int score)
        {
            crownImage.gameObject.SetActive(isWinner);
            nameText.text = playerName;
            for (int i = 0; i < GameManager.MAX_ROUND; i++)
            {
                scorePoints[i].text = scores[i].ToString();
            }

            scoreText.text = score.ToString();
        }
    }
}
