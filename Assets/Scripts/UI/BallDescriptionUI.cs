using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class BallDescriptionUI : MonoBehaviour
    {
        [SerializeField] private Image ballImage;
        [SerializeField] private Text ballDescription;

        public void SetData(Sprite sprite, string text)
        {
            ballImage.sprite = sprite;
            ballDescription.text = text;
        }
    }
}