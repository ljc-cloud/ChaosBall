using System;
using UnityEngine;

namespace ChaosBall.UI
{
    public class GameOverTitleUI : MonoBehaviour
    {
        public event Action OnTitleAnimOver;
        
        public void TitleAnimOver()
        {
            OnTitleAnimOver?.Invoke();
        }
    }
}