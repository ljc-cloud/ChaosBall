using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UI
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
