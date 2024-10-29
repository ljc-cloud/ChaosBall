using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public enum SceneEnum
    {
        MainMenuScene,
        LoadScene,
        GameScene
    }

    public static class SceneLoader
    {
        public static void LoadScene(SceneEnum sceneEnum)
        {
            SceneManager.LoadScene(sceneEnum.ToString());
        }

        public static void LoadSceneAsync(SceneEnum sceneEnum, Action<AsyncOperation> action)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneEnum.ToString());
            action?.Invoke(asyncOperation);
        }
    }
}