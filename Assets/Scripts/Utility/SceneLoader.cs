using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChaosBall.Utility
{
    public enum SceneEnum
    {
        MainMenuScene,
        LevelSelectScene,
        LoadScene,
        GameScene,
        GameOverScene
    }

    public enum LevelEnum
    {
        Level1,
        Level2
    }

    public static class SceneLoader
    {
        public static SceneEnum NEXT_SCENE;
        public static LevelEnum NEXT_LEVEL;

        public static event Action<LevelEnum> OnLevelLoadComplete;
        public static void LoadScene(SceneEnum sceneEnum)
        {
            SceneManager.LoadScene(sceneEnum.ToString());
        }

        public static void LoadLevel(LevelEnum levelEnum)
        {
            SceneManager.LoadScene(levelEnum.ToString());
        }
        
        public static void LoadLevelAsync(LevelEnum levelEnum, Action<AsyncOperation> action)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(levelEnum.ToString());
            asyncOperation.completed += _ =>
            {
                OnLevelLoadComplete?.Invoke(levelEnum);
            };
            action?.Invoke(asyncOperation);
        }

        public static void LoadSceneAsync(SceneEnum sceneEnum, Action<AsyncOperation> action)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneEnum.ToString());
            action?.Invoke(asyncOperation);
        }
    }
}