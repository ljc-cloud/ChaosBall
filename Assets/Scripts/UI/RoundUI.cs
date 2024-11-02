using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class RoundUI : MonoBehaviour
    {
        [SerializeField] private Image circle;
        [SerializeField] private Image circleFill;


        public void SetFill()
        {
            circle.Hide();
            circleFill.Show();
        }

        public void SetEmpty()
        {
            circleFill.Hide();
            circle.Show();
        }
    }
}