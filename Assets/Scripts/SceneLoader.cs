using System;
using ChaosBall.Event.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChaosBall
{
    public enum Scene 
    {
        MainMenuScene,
        RoomScene,
        LoadingScene,
        GameScene,
        GameOverScene,
    }

    public class SceneLoader
    {
        public static event Action<Scene> OnLoadScene;
        public static event Action<Scene> OnSceneLoadComplete;
    
        public void LoadScene(Scene scene)
        {
            OnLoadScene?.Invoke(scene);
            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            loadSceneAsync.completed += _ => OnSceneLoadComplete?.Invoke(scene); 
        }

        public AsyncOperation LoadGameSceneAsync()
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(Scene.GameScene.ToString(), LoadSceneMode.Single);
            loadSceneAsync.completed += _ => OnSceneLoadComplete?.Invoke(Scene.GameScene);
            return loadSceneAsync;
        }

        public void LoadSceneAsync(Scene scene, Action onComplete = null)
        {
            OnLoadScene?.Invoke(scene);
            // GameInterface.Interface.EventSystem.Publish<SceneLoadEvent>();
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);

            loadSceneAsync.completed += _ =>
            {
                OnSceneLoadComplete?.Invoke(scene);
                onComplete?.Invoke();
            };
        }
    }
}