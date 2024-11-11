using UnityEngine;

namespace ChaosBall.Game
{
    public class BallVisual : MonoBehaviour
    {
        [SerializeField] private Renderer bodyRenderer;

        public void SetColor(Color color)
        {
            bodyRenderer.material.color = color;
        }
    }
}