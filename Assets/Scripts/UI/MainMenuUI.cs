using UnityEngine;
using UnityEngine.UI;
using ChaosBall.Utility;

namespace ChaosBall.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            startGameButton.onClick.AddListener(() =>
            {
                SceneLoader.LoadScene(SceneEnum.LoadScene);
            });
            quitButton.onClick.AddListener(Application.Quit);
        }
    }

}
