using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChaosBall
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        GameOverScene,
        RoomScene,
    }
    public class SceneLoader
    {
        
        public event Action OnSceneLoad;
        public event Action OnSceneLoadComplete;

        public void LoadScene(Scene scene)
        {
            OnSceneLoad?.Invoke();
            SceneManager.LoadScene(scene.ToString());

            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            loadSceneAsync.completed += _ => OnSceneLoadComplete?.Invoke();
        }

        public void LoadSceneAsync(Scene scene, Action onComplete = null)
        {
            OnSceneLoad?.Invoke();

            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);

            loadSceneAsync.completed += _ =>
            {
                OnSceneLoadComplete?.Invoke();
                onComplete?.Invoke();
            };
        }
    }
}