using UnityEngine;

namespace ChaosBall
{
    public class ChaosBallGameManager : MonoBehaviour
    {
        public static ChaosBallGameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        
    }
}